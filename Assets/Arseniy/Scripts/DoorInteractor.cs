using Source._2DInteractive;
using UnityEngine;
using Unity.Cinemachine;


public class DoorInteractor : MonoBehaviour
{
    [Header("References (per-door)")]
    [Tooltip("UI панель не хранится здесь — она общая у DoorRaycaster (sharedPromptPanel).")]
    public GameObject openCutscene;
    public GameObject closeCutScene;

    [Tooltip("Cinemachine virtual camera (optional) — будет обнулён priority при открытии.")]
    public CinemachineCamera cinemachineCam;
    [SerializeField] private InteractiableController interactiableController;

    [SerializeField] private int doorIndex;
    public int currentDoor = 1;
    
    // состояние
    private bool doorOpened = false;

    // Публичный геттер, чтобы менеджер знал, можно ли взаимодействовать
    public bool IsOpened => doorOpened;

    // Публичный метод открытия — вызывается менеджером (DoorRaycaster)

    public void DoorIndexAddition()
    {
        currentDoor += 1;
    }

    public bool CanOpen()
    {
        if (doorIndex == currentDoor) return true;
        return false;
    }
    
    public void OpenDoor()
    {
        if (doorOpened) return;
        doorOpened = true;
        interactiableController.playerRb.isKinematic = true;

        if (openCutscene != null)
            openCutscene.SetActive(true);
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