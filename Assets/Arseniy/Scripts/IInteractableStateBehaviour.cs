using UnityEngine.EventSystems;

public interface IInteractableStateBehaviour
{
    void OnStateEnter(InteractableStateMachine.State newState);
    void OnStateExit(InteractableStateMachine.State oldState);
    void OnPointerEnter(PointerEventData eventData);
    void OnPointerExit(PointerEventData eventData);
    void OnPointerClick(PointerEventData eventData);
    void OnUpdate(); // called from StateMachine.Update()
}