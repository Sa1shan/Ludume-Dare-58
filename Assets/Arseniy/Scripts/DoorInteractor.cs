using UnityEngine;
using Unity.Cinemachine; // если у вас другая версия пакета, замените на нужный namespace

[AddComponentMenu("Interaction/Door Interactor")]
public class DoorInteractor : MonoBehaviour
{
    [Header("Raycast / interaction")]
    [Tooltip("Камера, из которой идёт луч. Если пусто — используется Camera.main")]
    public Camera sourceCamera;
    [Tooltip("Максимальная дистанция взаимодействия")]
    public float maxDistance = 2f;
    [Tooltip("LayerMask для взаимодействия (опционально)")]
    public LayerMask interactableMask = ~0;

    [Header("Door references")]
    [Tooltip("Объект двери, с которым связан этот скрипт. Если пусто — будет использован сам GameObject.")]
    public GameObject doorObject;

    [Header("UI")]
    [Tooltip("Панель с текстом 'Нажмите E' (будет показываться/скрываться)")]
    public GameObject promptPanel;

    [Header("Object to Activate (this door's cutscene / target)")]
    [Tooltip("Объект, который нужно включить при взаимодействии (каждая дверь может иметь свой)")]
    public GameObject targetObject;

    [Header("Cinemachine (optional)")]
    [Tooltip("Виртуальная камера Cinemachine, у которой нужно сбросить Priority (опционально)")]
    public CinemachineCamera cinemachineCam;

    // внутренние
    private Collider doorCollider;
    private bool promptVisible = false;
    private bool doorOpened = false;

    void Reset()
    {
        // по умолчанию привяжем камеру, если скрипт повесили на объект с камерой
        sourceCamera = GetComponent<Camera>();
    }

    void Start()
    {
        if (sourceCamera == null)
        {
            sourceCamera = Camera.main;
            if (sourceCamera == null)
                Debug.LogWarning("[DoorInteractor] sourceCamera не назначена и Camera.main равна null.");
        }

        if (doorObject == null)
            doorObject = this.gameObject;

        doorCollider = doorObject.GetComponent<Collider>();
        if (doorCollider == null)
            Debug.LogWarning($"[DoorInteractor] У объекта двери '{doorObject.name}' нет Collider'а — луч не сможет её распознать.");

        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    void Update()
    {
        if (doorOpened) return; // если дверь уже открыта — ничего не делать

        if (sourceCamera == null || doorCollider == null) return;

        Ray ray = sourceCamera.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        RaycastHit hit;

        // Если попали в пределах maxDistance по слоям
        if (Physics.Raycast(ray, out hit, maxDistance, interactableMask))
        {
            // Проверяем, что попали именно в этот объект двери (или в его детей)
            if (IsHitThisDoor(hit.collider))
            {
                ShowPrompt(true);

                // Нажатие E
                if (Input.GetKeyDown(KeyCode.E))
                {
                    OpenDoor();
                }

                return; // ранний выход — подсказка показана, не выключаем её ниже
            }
        }

        // Если сюда дошли — ничего не попало или это не эта дверь
        ShowPrompt(false);
    }

    bool IsHitThisDoor(Collider hitCollider)
    {
        // Попали прямо в тот же Коллайдер
        if (hitCollider == doorCollider) return true;

        // Либо попали в дочерний объект двери — проверим родителей
        Transform t = hitCollider.transform;
        while (t != null)
        {
            if (t.gameObject == doorObject) return true;
            t = t.parent;
        }

        return false;
    }

    void ShowPrompt(bool show)
    {
        if (promptPanel != null && promptPanel.activeSelf != show)
            promptPanel.SetActive(show);

        promptVisible = show;
    }

    void OpenDoor()
    {
        // защита от повторных вызовов
        if (doorOpened) return;
        doorOpened = true;

        // скрыть подсказку
        if (promptPanel != null)
            promptPanel.SetActive(false);

        // включить целевой объект
        if (targetObject != null)
            targetObject.SetActive(true);
        else
            Debug.LogWarning($"[DoorInteractor] targetObject не назначен на двери '{doorObject.name}'.");

        // сбрасываем вес (priority) виртуальной камеры, если назначена
        if (cinemachineCam != null)
        {
            try
            {
                cinemachineCam.Priority = 0;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[DoorInteractor] Не удалось изменить Priority у Cinemachine камеры: {e.Message}");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Camera c = sourceCamera != null ? sourceCamera : Camera.main;
        if (c == null) return;

        Vector3 origin = c.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, c.nearClipPlane));
        Vector3 dir = c.transform.forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, dir * maxDistance);
    }
}
