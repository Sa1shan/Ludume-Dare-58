using UnityEngine;
using Unity.Cinemachine;

[AddComponentMenu("Interaction/Door Interactor")]
public class DoorInteractor : MonoBehaviour
{
    [Header("Raycast / Interaction")]
    [Tooltip("Камера, из которой идёт луч. Если пусто — используется Camera.main.")]
    [SerializeField] private Camera sourceCamera;

    [Tooltip("Максимальная дистанция взаимодействия.")]
    [SerializeField] private float maxDistance = 2f;

    [Tooltip("Слои, по которым производится проверка Raycast.")]
    [SerializeField] private LayerMask interactableMask = ~0;

    [Header("Door Settings")]
    [Tooltip("Тэг объекта двери (по умолчанию Door).")]
    [SerializeField] private string doorTag = "Door";

    [Header("References")]
    [Tooltip("UI панель с текстом 'Нажмите E'.")]
    [SerializeField] private GameObject promptPanel;

    [Tooltip("Объект, который активируется при взаимодействии.")]
    [SerializeField] private GameObject targetObject;

    [Tooltip("Камера Cinemachine, у которой нужно сбросить приоритет.")]
    [SerializeField] private CinemachineCamera cinemachineCam;

    private bool doorOpened = false;

    void Start()
    {
        if (sourceCamera == null)
        {
            sourceCamera = Camera.main;
            if (sourceCamera == null)
                Debug.LogWarning("[DoorInteractor] Камера не назначена и Camera.main не найдена.");
        }

        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    void Update()
    {
        // если дверь уже открыта — гарантированно выключаем UI и ничего не делаем
        if (doorOpened)
        {
            if (promptPanel != null && promptPanel.activeSelf)
                promptPanel.SetActive(false);
            return;
        }

        if (sourceCamera == null) return;

        Ray ray = sourceCamera.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, interactableMask))
        {
            // Проверяем, есть ли у объекта тег Door
            if (hit.collider.CompareTag(doorTag))
            {
                ShowPrompt(true);

                // Если нажата клавиша E
                if (Input.GetKeyDown(KeyCode.E))
                {
                    OpenDoor();
                }

                return; // выходим, чтобы не скрывать подсказку
            }
        }

        // Если луч не попал в дверь
        ShowPrompt(false);
    }

    private void ShowPrompt(bool show)
    {
        if (promptPanel != null && promptPanel.activeSelf != show)
            promptPanel.SetActive(show);
    }

    private void OpenDoor()
    {
        if (doorOpened) return;
        doorOpened = true;

        // Отключаем UI
        if (promptPanel != null)
            promptPanel.SetActive(false);

        // Включаем целевой объект
        if (targetObject != null)
            targetObject.SetActive(true);
        else
            Debug.LogWarning($"[DoorInteractor] targetObject не назначен у двери {gameObject.name}");

        // Сбрасываем приоритет камеры
        if (cinemachineCam != null)
        {
            try
            {
                cinemachineCam.Priority = 0;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[DoorInteractor] Ошибка при изменении приоритета камеры: {e.Message}");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (sourceCamera == null)
            sourceCamera = Camera.main;

        if (sourceCamera == null) return;

        Vector3 origin = sourceCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, sourceCamera.nearClipPlane));
        Vector3 direction = sourceCamera.transform.forward;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(origin, direction * maxDistance);
    }
}
