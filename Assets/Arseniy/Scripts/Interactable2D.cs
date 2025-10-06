using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Collider2D))]
public class Interactable2D : MonoBehaviour
{
    [Header("Outline (assign a child SpriteRenderer that's a white sprite)")]
    public SpriteRenderer outlineRenderer; // assign: child white sprite used as outline
    public Vector3 outlineScale = Vector3.one;
    public bool outlineEnableOnHover = true;


    [Header("Events")]
    public UnityEvent onClick;
    public UnityEvent onHoverEnter;
    public UnityEvent onHoverExit;


    [HideInInspector]
    public bool IsHovered { get; private set; }


    void Reset()
    {
// попытка автоматически найти возможный дочерний SpriteRenderer (не гарантированно)
        if (outlineRenderer == null)
        {
            var sr = GetComponentInChildren<SpriteRenderer>(true);
            if (sr != null && sr != GetComponent<SpriteRenderer>())
                outlineRenderer = sr;
        }
    }


// Вызывается InteractionManager при наведении
    public void OnHoverEnter()
    {
        if (IsHovered) return;
        IsHovered = true;
        if (outlineRenderer != null && outlineEnableOnHover)
        {
            outlineRenderer.gameObject.SetActive(true);
            outlineRenderer.transform.localScale = outlineScale;
        }
        onHoverEnter?.Invoke();
    }


// Вызывается InteractionManager при уходе курсора
    public void OnHoverExit()
    {
        if (!IsHovered) return;
        IsHovered = false;
        if (outlineRenderer != null && outlineEnableOnHover)
            outlineRenderer.gameObject.SetActive(false);
        onHoverExit?.Invoke();
    }


// Вызывается InteractionManager при клике
    public void OnClick()
    {
        onClick?.Invoke();
    }
}