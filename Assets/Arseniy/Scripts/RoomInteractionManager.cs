using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class RoomInteractionManager : MonoBehaviour
{
    [Header("Room")]
    [Tooltip("Root объект вашей 2D комнаты / фон. Когда он activeInHierarchy — включаем интерактивность.")]
    [SerializeField] private GameObject roomRoot;

    [Header("Cursor")]
    [SerializeField] private bool useSystemCursor = true;
    [SerializeField] private Texture2D systemCursorTexture;
    [SerializeField] private Vector2 systemCursorHotspot = Vector2.zero;
    [SerializeField] private GameObject customCursor;

    [Header("Raycast / Filtering")]
    [Tooltip("Камера для расчёта позиции мыши. Если null — Camera.main.")]
    [SerializeField] private Camera raycastCamera;
    [Tooltip("Если ваши игровые объекты не на Z=0 — укажите здесь Z-плоскость, на которой лежат коллайдеры.")]
    [SerializeField] private float worldPlaneZ = 0f;
    [SerializeField] private LayerMask interactableMask = ~0;

    [Header("Behaviour")]
    [Tooltip("Если true — блокируем мирные события, когда указатель над UI (GraphicRaycaster).")]
    [SerializeField] private bool blockWhenPointerOverUI = true;
    [Tooltip("Сколько кадров подряд кандидат должен оставаться — прежде чем мы сменим hover (устранение мигания). 1 = немедленно.")]
    [Range(1, 6)]
    [SerializeField] private int stableFramesThreshold = 2;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    [SerializeField] private bool drawGizmos = true;

    private EventSystem evt;
    private PointerEventData pointerData;
    private GameObject currentHovered;
    private bool interactionsEnabled = false;

    // stabilization
    private GameObject lastCandidate;
    private int stableCounter = 0;

    // cache
    private readonly List<RaycastResult> uiRaycastResults = new List<RaycastResult>();

    void Awake()
    {
        evt = EventSystem.current;
        if (evt == null)
        {
            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            evt = go.GetComponent<EventSystem>();
            Debug.Log("[RoomInteractionManager] EventSystem was missing — created automatically.");
        }

        pointerData = new PointerEventData(evt);
        if (raycastCamera == null) raycastCamera = Camera.main;

        interactionsEnabled = roomRoot ? roomRoot.activeInHierarchy : false;
        ApplyCursorVisibility(interactionsEnabled);
    }

    void Update()
    {
        if (roomRoot != null)
        {
            bool shouldBeEnabled = roomRoot.activeInHierarchy;
            if (shouldBeEnabled != interactionsEnabled) SetRoomActive(shouldBeEnabled);
        }

        if (!interactionsEnabled)
        {
            if (customCursor && customCursor.activeSelf) customCursor.SetActive(false);
            return;
        }

        UpdateCursorPosition();
        HandlePointer();
    }

    public void SetRoomActive(bool active)
    {
        interactionsEnabled = active;
        ApplyCursorVisibility(active);

        if (!active && currentHovered != null)
        {
            var pd = CreatePointerEventData();
            ExecuteEvents.Execute(currentHovered, pd, ExecuteEvents.pointerExitHandler);
            currentHovered = null;
        }
    }

    private void ApplyCursorVisibility(bool visible)
    {
        if (useSystemCursor)
        {
            Cursor.visible = visible;
            if (systemCursorTexture != null && visible)
                Cursor.SetCursor(systemCursorTexture, systemCursorHotspot, CursorMode.Auto);
            else if (!visible)
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            if (customCursor) customCursor.SetActive(visible);
            Cursor.visible = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    private void UpdateCursorPosition()
    {
        if (!useSystemCursor && customCursor)
        {
            customCursor.transform.position = Input.mousePosition;
        }
    }

    private void HandlePointer()
{
    pointerData.position = Input.mousePosition;

    // 1. Проверка выхода за пределы экрана
    if (pointerData.position.x < 0 || pointerData.position.y < 0 ||
        pointerData.position.x > Screen.width || pointerData.position.y > Screen.height)
    {
        if (currentHovered != null)
        {
            var pdExit = CreatePointerEventData();
            ExecuteEvents.Execute(currentHovered, pdExit, ExecuteEvents.pointerExitHandler);
            currentHovered = null;
        }
        return;
    }

    // 2. Блокировка под UI
    if (blockWhenPointerOverUI && IsPointerOverUI(pointerData))
    {
        if (currentHovered != null)
        {
            var pdExit = CreatePointerEventData();
            ExecuteEvents.Execute(currentHovered, pdExit, ExecuteEvents.pointerExitHandler);
            currentHovered = null;
        }
        return;
    }

    // 3. Создаём 3D-луч от камеры в направлении курсора
    Ray ray = raycastCamera.ScreenPointToRay(pointerData.position);
    RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, interactableMask);

    GameObject top = PickTopMostInteractableFrom3DHits(hits);

    // 4. Смена наведённого объекта
    if (top != currentHovered)
    {
        if (currentHovered != null)
        {
            var pdExit = CreatePointerEventData();
            ExecuteEvents.Execute(currentHovered, pdExit, ExecuteEvents.pointerExitHandler);
        }

        if (top != null)
        {
            var pdEnter = CreatePointerEventData();
            ExecuteEvents.Execute(top, pdEnter, ExecuteEvents.pointerEnterHandler);
        }

        currentHovered = top;
    }

    // 5. Клик
    if (Input.GetMouseButtonDown(0) && currentHovered != null)
    {
        var pdClick = CreatePointerEventData();
        pdClick.button = PointerEventData.InputButton.Left;
        ExecuteEvents.Execute(currentHovered, pdClick, ExecuteEvents.pointerClickHandler);
    }
}

private GameObject PickTopMostInteractableFrom3DHits(RaycastHit[] hits)
{
    if (hits == null || hits.Length == 0)
        return null;

    // Сортируем по расстоянию (ближайший первым)
    System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

    foreach (var h in hits)
    {
        if (h.collider == null) continue;
        var inter = h.collider.GetComponentInParent<Interactable2D>();
        if (inter != null)
            return inter.gameObject;
    }

    return null;
}


    private PointerEventData CreatePointerEventData()
    {
        var pd = new PointerEventData(evt)
        {
            position = pointerData.position,
            pointerId = pointerData.pointerId
        };
        return pd;
    }

    private bool IsPointerOverUI(PointerEventData pd)
    {
        uiRaycastResults.Clear();
        evt.RaycastAll(pd, uiRaycastResults);
        return uiRaycastResults.Count > 0;
    }

    // stable selection logic: return candidate after it stays the same for stableFramesThreshold frames
    private GameObject GetStabilizedCandidate(GameObject immediateTop)
    {
        if (immediateTop == lastCandidate)
        {
            stableCounter++;
        }
        else
        {
            lastCandidate = immediateTop;
            stableCounter = 1;
        }

        if (stableCounter >= Mathf.Max(1, stableFramesThreshold))
        {
            // reset counters so next time we start fresh
            lastCandidate = null;
            stableCounter = 0;
            return immediateTop;
        }

        // not enough stability — keep current hovered
        return currentHovered;
    }

    // prefer collider's own Interactable2D before searching parents
    private GameObject PickTopMostInteractableFrom2DHits(Collider2D[] hits)
    {
        if (hits == null || hits.Length == 0) return null;

        // map interactable -> (order, z)
        var map = new Dictionary<Interactable2D, (int order, float z)>();

        foreach (var h in hits)
        {
            if (h == null) continue;

            // prefer if Interactable2D is on collider.gameObject
            Interactable2D inter = h.GetComponent<Interactable2D>();
            if (inter == null) inter = h.GetComponentInParent<Interactable2D>();
            if (inter == null) continue;

            if (map.ContainsKey(inter)) continue;

            int order = int.MinValue;
            var srOnRoot = inter.GetComponent<SpriteRenderer>();
            if (srOnRoot != null) order = srOnRoot.sortingOrder;
            else
            {
                var srChild = inter.GetComponentInChildren<SpriteRenderer>();
                if (srChild != null) order = srChild.sortingOrder;
            }

            map[inter] = (order, inter.transform.position.z);
        }

        Interactable2D best = null;
        int bestOrder = int.MinValue;
        float bestZ = float.MaxValue;

        foreach (var kv in map)
        {
            var k = kv.Key;
            var o = kv.Value.order;
            var z = kv.Value.z;

            if (best == null || o > bestOrder || (o == bestOrder && z < bestZ))
            {
                best = k; bestOrder = o; bestZ = z;
            }
        }

        return best ? best.gameObject : null;
    }

    // Robust Screen->World point at arbitrary Z plane
    private Vector2 ScreenToWorldPointAtZ(Vector2 screenPos, float targetWorldZ)
    {
        if (raycastCamera == null)
        {
            raycastCamera = Camera.main;
            if (raycastCamera == null) return Vector2.zero;
        }

        Ray ray = raycastCamera.ScreenPointToRay(screenPos);
        // if ray has z-direction (perspective) — compute intersection
        if (Mathf.Abs(ray.direction.z) > 1e-6f)
        {
            float t = (targetWorldZ - ray.origin.z) / ray.direction.z;
            Vector3 wp = ray.GetPoint(t);
            return new Vector2(wp.x, wp.y);
        }
        else
        {
            // orthographic camera
            Vector3 wp = ray.origin;
            wp.z = targetWorldZ;
            return new Vector2(wp.x, wp.y);
        }
    }

    // helpers you can call from other scripts
    public void ForceClick(GameObject go)
    {
        if (go == null) return;
        var pd = CreatePointerEventData();
        pd.button = PointerEventData.InputButton.Left;
        ExecuteEvents.Execute(go, pd, ExecuteEvents.pointerClickHandler);
    }

    public void ForceHover(GameObject go)
    {
        if (go == null) return;
        if (currentHovered == go) return;
        if (currentHovered != null)
        {
            var pd = CreatePointerEventData();
            ExecuteEvents.Execute(currentHovered, pd, ExecuteEvents.pointerExitHandler);
            currentHovered = null;
        }
        var pdNew = CreatePointerEventData();
        ExecuteEvents.Execute(go, pdNew, ExecuteEvents.pointerEnterHandler);
        currentHovered = go;
    }

    // Draw debug gizmos to visualize the mouse projection
    void OnDrawGizmos()
    {
        if (!drawGizmos || !Application.isPlaying) return;

        Vector2 screen = Input.mousePosition;
        Vector2 world = ScreenToWorldPointAtZ(screen, worldPlaneZ);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(new Vector3(world.x, world.y, worldPlaneZ), 0.1f);
    }
}
