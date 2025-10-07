using UnityEngine;
using Unity.Cinemachine;

[AddComponentMenu("Interaction/Door Interactor (Data)")]
public class DoorInteractor : MonoBehaviour
{
    [Header("References (per-door)")]
    [Tooltip("UI панель не хранится здесь — она общая у DoorRaycaster (sharedPromptPanel).")]
    [SerializeField] private GameObject targetObject;

    [Tooltip("Cinemachine virtual camera (optional) — будет обнулён priority при открытии.")]
    [SerializeField] private CinemachineCamera cinemachineCam;

    // состояние
    private bool doorOpened = false;

    // Публичный геттер, чтобы менеджер знал, можно ли взаимодействовать
    public bool IsOpened => doorOpened;

    // Публичный метод открытия — вызывается менеджером (DoorRaycaster)
    public void OpenDoor()
    {
        if (doorOpened) return;
        doorOpened = true;

        if (targetObject != null)
            targetObject.SetActive(true);
        else
            Debug.LogWarning($"[DoorInteractor] targetObject не назначен у двери '{gameObject.name}'");

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
}