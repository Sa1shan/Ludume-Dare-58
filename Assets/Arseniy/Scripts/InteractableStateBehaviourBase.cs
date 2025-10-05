using UnityEngine;
using UnityEngine.EventSystems;

public abstract class InteractableStateBehaviourBase : UnityEngine.MonoBehaviour, IInteractableStateBehaviour
{
    public virtual void OnStateEnter(InteractableStateMachine.State newState) { }
    public virtual void OnStateExit(InteractableStateMachine.State oldState) { }
    public virtual void OnPointerEnter(PointerEventData eventData) { }
    public virtual void OnPointerExit(PointerEventData eventData) { }
    public virtual void OnPointerClick(PointerEventData eventData) { }
    public virtual void OnUpdate() { }
}