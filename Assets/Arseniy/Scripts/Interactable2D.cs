using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class Interactable2D : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Visual")]
    [Tooltip("Child GameObject with white sprite used as outline/highlight (SetActive true/false)")]
    [SerializeField] private GameObject outline;

    [Header("State machine (optional - will be added automatically if absent)")]
    [SerializeField] private InteractableStateMachine stateMachine;

    void Reset()
    {
        var t = transform.Find("Outline");
        if (t != null) outline = t.gameObject;

        stateMachine = GetComponent<InteractableStateMachine>();
        if (stateMachine == null) stateMachine = gameObject.AddComponent<InteractableStateMachine>();
    }

    void Awake()
    {
        if (outline) outline.SetActive(false);
        if (stateMachine == null) stateMachine = GetComponent<InteractableStateMachine>();
    }

    void OnEnable()
    {
        if (outline) outline.SetActive(false);
        if (stateMachine == null) stateMachine = GetComponent<InteractableStateMachine>();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // in editor, keep outline off by default to avoid confusion
        if (outline != null && !Application.isPlaying)
        {
            outline.SetActive(false);
        }
    }
#endif

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (outline) outline.SetActive(true);
        stateMachine?.NotifyPointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (outline) outline.SetActive(false);
        stateMachine?.NotifyPointerExit(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        stateMachine?.NotifyPointerClick(eventData);
    }
}