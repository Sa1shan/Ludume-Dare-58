using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Скрипт для каждого кликабельного Image-объекта.
/// При клике: звуковой эффект -> плавный fade (DoTween) -> Deactivate (SetActive(false)).
/// Защищён от повторных кликов.
/// </summary>
[RequireComponent(typeof(Image))]
public class ClickableItem : MonoBehaviour, IPointerClickHandler
{
    private Image image;
    private MiniGameController manager;
    private bool isFound = false;

    private void Awake()
    {
        image = GetComponent<Image>();
        if (image == null)
            Debug.LogError("[ClickableItem] Требуется компонент Image.");

        // Гарантируем начальную видимость
        var col = image.color;
        col.a = 1f;
        image.color = col;

        // Image должен принимать клики
        image.raycastTarget = true;
    }

    /// <summary>
    /// Менеджер установится автоматически, когда ты перетащишь предметы в массив в MiniGameController.
    /// Можно также назначить менеджера вручную через инспектор (через метод SetManager).
    /// </summary>
    public void SetManager(MiniGameController m)
    {
        manager = m;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Защита от повторных кликов и от кликов после окончания игры
        if (isFound) return;
        if (manager != null && manager.IsGameEnded) return;

        isFound = true;

        // Немедленно отключаем принимающие клики, чтобы второй клик не сработал.
        image.raycastTarget = false;

        // Уведомляем менеджер
        manager?.NotifyItemFound(this);

        // Проигрываем sfx
        manager?.PlayClickSfx();

        // Плавно исчезаем и деактивируем объект после анимации
        float duration = (manager != null) ? manager.FadeDuration : 0.5f;
        image.DOFade(0f, duration).OnComplete(() =>
        {
            // Отключаем объект (чтобы он не мешал в иерархии)
            gameObject.SetActive(false);
        });
    }

    /// <summary>
    /// Отключает только взаимодействие (не трогает визуал).
    /// Вызывается менеджером при завершении игры, чтобы предотвратить дальнейшие клики.
    /// </summary>
    public void DisableInteraction()
    {
        if (!isFound && image != null)
            image.raycastTarget = false;
    }
}
