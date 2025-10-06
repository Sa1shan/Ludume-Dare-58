using UnityEngine;


public class InteractionManager2D : MonoBehaviour
{
    public static InteractionManager2D Instance;


    [Header("References")]
    public Camera mainCamera; // установите либо вручную, либо будет взято Camera.main
    public LayerMask interactableLayer = ~0; // слой(и) для интерактивных объектов


    [Header("Raycast / Input settings")]
    public bool use2DPhysics = true; // если false — используется Physics.Raycast (3D)
    public float maxRayDistance = 100f;


    [Header("Interaction mode")]
    public bool cursorVisibleWhenActive = true;
    public Behaviour playerControllerToDisable; // например скрипт FPS контроллера


    Interactable2D currentHover;
    bool interactionActive = true;


    void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
        if (mainCamera == null) mainCamera = Camera.main;
    }


    void Update()
    {
        if (!interactionActive) return;
        HandleHover();


        if (Input.GetMouseButtonDown(0))
        {
            if (currentHover != null)
            {
                currentHover.OnClick();
            }
        }
    }
    void HandleHover()
    {
        if (mainCamera == null) return;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Interactable2D hitInteract = null;


        if (use2DPhysics)
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, maxRayDistance, interactableLayer);
            if (hit.collider != null)
                hitInteract = hit.collider.GetComponent<Interactable2D>();
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxRayDistance, interactableLayer))
                hitInteract = hit.collider.GetComponent<Interactable2D>();
        }


        if (hitInteract != currentHover)
        {
            if (currentHover != null) currentHover.OnHoverExit();
            currentHover = hitInteract;
            if (currentHover != null) currentHover.OnHoverEnter();
        }
    }


// Включить/выключить режим взаимодействия (включает/скрывает курсор и отключает контроллер)
    public void SetInteractionActive(bool active)
    {
        interactionActive = active;
        if (cursorVisibleWhenActive)
        {
            Cursor.visible = active;
            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
        }
        if (playerControllerToDisable != null)
            playerControllerToDisable.enabled = !active;


        if (!active)
            ForceClearHover();
    }


// Принудительно убрать hover (например при закрытии UI)
    public void ForceClearHover()
    {
        if (currentHover != null) { currentHover.OnHoverExit(); currentHover = null; }
    }
}