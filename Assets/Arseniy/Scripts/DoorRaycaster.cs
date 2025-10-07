using UnityEngine;

[AddComponentMenu("Interaction/Door Raycaster (Manager)")]
public class DoorRaycaster : MonoBehaviour
{
    [Header("Raycast")]
    [Tooltip("Камера, от которой идёт луч. Если пусто — используется Camera.main.")]
    [SerializeField] private Camera sourceCamera;
    [SerializeField] private float maxDistance = 2f;
    [SerializeField] private LayerMask interactableMask = ~0;
    [SerializeField] private string doorTag = "Door";

    [Header("Shared UI")]
    [Tooltip("Общая панель подсказки для всех дверей (пример: 'Нажмите E').")]
    [SerializeField] private GameObject sharedPromptPanel;

    
    // внутренняя ссылка на DoorInteractor текущей в прицеле двери
    private DoorInteractor currentDoorInteractor;

    void Start()
    {
        if (sourceCamera == null)
        {
            sourceCamera = Camera.main;
            if (sourceCamera == null)
                Debug.LogWarning("[DoorRaycaster] Камера не назначена и Camera.main не найдена.");
        }

        if (sharedPromptPanel != null)
            sharedPromptPanel.SetActive(false);
    }

    void Update()
    {
        if (sourceCamera == null) return;

        // Если уже открыт активный door — просто скрываем UI и не даём его снова показать
        if (currentDoorInteractor != null && currentDoorInteractor.IsOpened)
        {
            HidePrompt();
            return;
        }

        Ray ray = sourceCamera.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        RaycastHit hit;

        DoorInteractor hitDoor = null;

        if (Physics.Raycast(ray, out hit, maxDistance, interactableMask))
        {
            if (hit.collider != null)
            {
                // ищем ближайший ancestor с тегом doorTag (или сам объект)
                Transform t = hit.collider.transform;
                Transform tagged = GetTaggedAncestor(t, doorTag);

                if (tagged != null)
                {
                    // пытаемся получить DoorInteractor у найденного объекта (на корне или в родителе)
                    hitDoor = tagged.GetComponent<DoorInteractor>();
                    if (hitDoor == null)
                    {
                        // может DoorInteractor висит в дочернем объекте — попробуем GetComponentInChildren
                        hitDoor = tagged.GetComponentInChildren<DoorInteractor>();
                    }
                }
                else
                {
                    // запасной вариант: если тег не используется, можно искать компонент DoorInteractor вверх по иерархии
                    Transform up = t;
                    while (up != null && hitDoor == null)
                    {
                        hitDoor = up.GetComponent<DoorInteractor>();
                        up = up.parent;
                    }
                }
            }
        }

        // Если нашли DoorInteractor под прицелом и дверь не открыта, показываем панель
        if (hitDoor != null && !hitDoor.IsOpened && hitDoor.CanOpen())
        {
            ShowPromptForDoor(hitDoor);
            if (Input.GetKeyDown(KeyCode.E))
            {
                hitDoor.OpenDoor();
                // после открытия — скрываем UI
                HidePrompt();
            }
        }
        else
        {
            // ничего подходящего не найдено — скрываем панель
            HidePrompt();
        }
    }

    private void ShowPromptForDoor(DoorInteractor door)
    {
        if (sharedPromptPanel != null && !sharedPromptPanel.activeSelf)
            sharedPromptPanel.SetActive(true);

        currentDoorInteractor = door;
    }

    private void HidePrompt()
    {
        if (sharedPromptPanel != null && sharedPromptPanel.activeSelf)
            sharedPromptPanel.SetActive(false);

        currentDoorInteractor = null;
    }

    // Поднимаемся вверх и ищем объект с нужным тегом
    private Transform GetTaggedAncestor(Transform t, string tag)
    {
        Transform cur = t;
        while (cur != null)
        {
            if (cur.CompareTag(tag))
                return cur;
            cur = cur.parent;
        }
        return null;
    }

    void OnDrawGizmosSelected()
    {
        if (sourceCamera == null) sourceCamera = Camera.main;
        if (sourceCamera == null) return;

        Vector3 origin = sourceCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, sourceCamera.nearClipPlane));
        Vector3 dir = sourceCamera.transform.forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, dir * maxDistance);
    }
}
