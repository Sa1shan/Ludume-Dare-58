using UnityEngine;
using UnityEngine.EventSystems;

public class ImageClickForwarder : MonoBehaviour, IPointerClickHandler
{
    public EndSequenceManager manager;

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.OnAnyImageClicked();
    }
}