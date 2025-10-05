using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InteractableStateMachine : MonoBehaviour
{
    public enum State { Idle = 0, Hovered = 1, Pressed = 2, Activated = 3, Disabled = 4 }

    [Header("State")]
    [SerializeField] private State startingState = State.Idle;
    public State CurrentState => currentState;

    [Header("Inspector hooks (per-object)")]
    public UnityEvent onEnterHovered;
    public UnityEvent onExitHovered;
    public UnityEvent onPressed;
    public UnityEvent onActivated;
    public UnityEvent onDisabled;
    public UnityEvent onClicked; // called immediately on pointer click

    private State currentState;
    private IInteractableStateBehaviour[] behaviours;

    void Awake()
    {
        // кешируем все компоненты, реализующие интерфейс
        behaviours = GetComponents<IInteractableStateBehaviour>();
        currentState = startingState;
        // notify initial state (optional)
        foreach (var b in behaviours) b.OnStateEnter(currentState);
    }

    public void SetState(State newState)
    {
        if (currentState == newState) return;

        var prev = currentState;
        // notify exit
        foreach (var b in behaviours) b.OnStateExit(prev);

        currentState = newState;

        // notify enter
        foreach (var b in behaviours) b.OnStateEnter(newState);

        // inspector events
        switch (newState)
        {
            case State.Hovered: onEnterHovered?.Invoke(); break;
            case State.Pressed: onPressed?.Invoke(); break;
            case State.Activated: onActivated?.Invoke(); break;
            case State.Disabled: onDisabled?.Invoke(); break;
        }
    }

    // Called by Interactable2D when pointer enters
    public void NotifyPointerEnter(PointerEventData eventData)
    {
        foreach (var b in behaviours) b.OnPointerEnter(eventData);
        SetState(State.Hovered);
    }

    // Called by Interactable2D when pointer exits
    public void NotifyPointerExit(PointerEventData eventData)
    {
        foreach (var b in behaviours) b.OnPointerExit(eventData);
        SetState(State.Idle);
    }

    // Called by Interactable2D when clicked
    public void NotifyPointerClick(PointerEventData eventData)
    {
        foreach (var b in behaviours) b.OnPointerClick(eventData);

        // immediate inspector event for click
        onClicked?.Invoke();

        // small state progression - Pressed -> Activated (you can change this)
        SetState(State.Pressed);
        SetState(State.Activated);
    }

    void Update()
    {
        // forward per-frame update to behaviours (if they need it)
        foreach (var b in behaviours) b.OnUpdate();
    }
}
