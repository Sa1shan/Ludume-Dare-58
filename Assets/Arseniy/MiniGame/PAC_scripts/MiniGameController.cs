using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// Менеджер мини-игры: хранит массив кликабельных предметов, обновляет счётчик,
/// воспроизводит звук клика, показывает финальную панель.
/// </summary>
public class MiniGameController : MonoBehaviour
{
    [Header("Setup (assign in Inspector)")]
    [Tooltip("Перетащи сюда 5 ClickableItem (вручную).")]
    [SerializeField] private ClickableItem[] items = new ClickableItem[5];

    [Tooltip("TextMeshProUGUI для отображения счёта: 0/5 объектов найдено")]
    [SerializeField] private TextMeshProUGUI counterText;

    [Tooltip("Панель (GameObject) которая появится мгновенно при завершении")]
    [SerializeField] private GameObject finalPanel;

    [Header("Audio")]
    [Tooltip("Звук при клике (AudioClip).")]
    [SerializeField] private AudioClip clickClip;

    [Tooltip("Если не поставишь, AudioSource добавится автоматически на этот объект.")]
    [SerializeField] private AudioSource audioSource;

    [Header("Gameplay")]
    [Tooltip("Длительность фейда (s) для DoTween.")]
    [SerializeField] private float fadeDuration = 0.5f;

    private int foundCount = 0;
    private bool gameEnded = false;

    public bool IsGameEnded => gameEnded;
    public float FadeDuration => fadeDuration;

    private void Awake()
    {
        // Гарантируем наличие AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        // Показываем системный курсор (на всякий случай)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (finalPanel != null)
            finalPanel.SetActive(false);

        // Инициализируем items (передаём ссылку менеджера каждому)
        if (items == null)
        {
            Debug.LogWarning("[MiniGameController] Items array is null.");
            items = new ClickableItem[0];
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
                items[i].SetManager(this);
            else
                Debug.LogWarning($"[MiniGameController] Item at index {i} is null in inspector.");
        }

        UpdateCounterUI();
    }

    /// <summary>
    /// Вызывается ClickableItem при первом клике на себя.
    /// </summary>
    public void NotifyItemFound(ClickableItem item)
    {
        if (gameEnded) return;

        foundCount++;
        UpdateCounterUI();

        if (foundCount >= items.Length)
            EndGame();
    }

    private void UpdateCounterUI()
    {
        if (counterText != null)
            counterText.text = $"{foundCount}/{items.Length} объектов найдено";
        else
            Debug.LogWarning("[MiniGameController] counterText is not assigned.");
    }

    private void EndGame()
    {
        gameEnded = true;

        // Отключаем взаимодействие у оставшихся объектов (чтобы пользователь не кликал зря)
        foreach (var it in items)
            if (it != null)
                it.DisableInteraction();

        // Показываем финальную панель мгновенно
        if (finalPanel != null)
            finalPanel.SetActive(true);
    }

    /// <summary>
    /// Проиграть звук клика (если есть).
    /// </summary>
    public void PlayClickSfx()
    {
        if (clickClip == null || audioSource == null) return;
        audioSource.PlayOneShot(clickClip);
    }
}
