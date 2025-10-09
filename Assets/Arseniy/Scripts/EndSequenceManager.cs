using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using DG.Tweening;
using Source.GamePlayUI;

public class EndSequenceManager : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private Button[] imageButtons; // Три кнопки (Image)
    [SerializeField] private Image vignetteImage;   // Виньетка
    [SerializeField] private Image blackBackground; // Черный фон
    [SerializeField] private TextMeshProUGUI endText; // Текст "Конец игры"

    [Header("New Sprites")]
    [SerializeField] private Sprite[] newSprites; // Новые спрайты (3 штуки)

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private string nextSceneName = "MainMenu";
    [SerializeField] private DialogueSystem dialogueSystem;

    private int clickCount = 0;
    private bool isEndSequenceStarted = false;

    private void Start()
    {
        // Начальные значения: если объекты заданы — подстраиваем их цвет, не меняя активность.
        if (vignetteImage != null)
        {
            // если объект выключен, мы не включаем его здесь — включим позже при PrepareForEndSequence()
            vignetteImage.color = new Color(vignetteImage.color.r, vignetteImage.color.g, vignetteImage.color.b, 0f);
        }

        if (blackBackground != null)
        {
            // черный фон оставим выключенным до нужного момента, но подготовим цвет
            blackBackground.color = new Color(blackBackground.color.r, blackBackground.color.g, blackBackground.color.b, 0f);
            blackBackground.gameObject.SetActive(false); // гарантируем, что фон изначально выключен
        }

        if (endText != null)
        {
            endText.text = "";
            endText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Проверяем момент, когда dialogue4IsFinish = true
        if (dialogueSystem != null && dialogueSystem.dialogue4IsFinish && !isEndSequenceStarted)
        {
            isEndSequenceStarted = true;
            PrepareForEndSequence();
        }
    }

    private void PrepareForEndSequence()
    {
        foreach (var btn in imageButtons)
        {
            if (btn == null) continue;
            // Полностью переинициализируем событие, чтобы удалить ВСЕ ссылки (включая добавленные в инспекторе)
            btn.onClick = new Button.ButtonClickedEvent();
        }

        // Включаем виньетку и чёрный фон (если они были выключены), и делаем виньетку не блокирующей клики
        if (vignetteImage != null)
        {
            if (!vignetteImage.gameObject.activeSelf)
                vignetteImage.gameObject.SetActive(true);

            // убеждаемся, что начальная альфа = 0 (чтобы DOFade корректно работал)
            var vc = vignetteImage.color;
            vc.a = 0f;
            vignetteImage.color = vc;

            var canvasGroup = vignetteImage.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = vignetteImage.gameObject.AddComponent<CanvasGroup>();

            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        if (blackBackground != null)
        {
            if (!blackBackground.gameObject.activeSelf)
                blackBackground.gameObject.SetActive(true);

            // убедимся, что фон прозрачный и не перехватывает клики до затемнения
            var bc = blackBackground.color;
            bc.a = 0f;
            blackBackground.color = bc;
            blackBackground.raycastTarget = false;
        }
    }

    public void OnAnyImageClicked()
    {
        if (dialogueSystem == null || !dialogueSystem.dialogue4IsFinish) return;

        clickCount++;

        // 1, 2, 3 клики управляют виньеткой
        if (clickCount == 1)
            FadeVignetteTo(0.3f);
        else if (clickCount == 2)
            FadeVignetteTo(0.6f);
        else if (clickCount == 3)
        {
            FadeVignetteTo(1);
            ChangeImagesSprites();
        }
        else if (clickCount == 4)
        {
            StartCoroutine(FadeToBlackAndShowText());
        }
    }

    private void FadeVignetteTo(float targetAlpha)
    {
        if (vignetteImage == null) return;

        // Убеждаемся, что объект активен перед анимацией
        if (!vignetteImage.gameObject.activeSelf)
            vignetteImage.gameObject.SetActive(true);

        vignetteImage.DOFade(targetAlpha, fadeDuration);
    }

    private void ChangeImagesSprites()
    {
        for (int i = 0; i < imageButtons.Length && i < newSprites.Length; i++)
        {
            SpriteState sprite = imageButtons[i].spriteState;
            sprite.highlightedSprite = null;
            imageButtons[i].spriteState = sprite;
            if (imageButtons[i] == null) continue;
            Image img = imageButtons[i].GetComponent<Image>();
            if (img != null && newSprites[i] != null)
                img.sprite = newSprites[i];
        }
    }

    private IEnumerator FadeToBlackAndShowText()
    {
        // затемнение экрана
        if (blackBackground != null)
        {
            // включаем перехват кликов, чтобы экран стал «модальным» во время конца
            blackBackground.raycastTarget = true;
            blackBackground.DOFade(1f, fadeDuration);
        }

        // ждём пока закончится fade (с небольшой страховкой)
        yield return new WaitForSeconds(fadeDuration + 0.05f);

        // показать текст
        if (endText != null)
        {
            endText.gameObject.SetActive(true);
            yield return StartCoroutine(PlayEndText("Was is this funny?"));
        }

        // ожидание клика
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator PlayEndText(string text)
    {
        if (endText == null) yield break;

        endText.text = "";
        foreach (char c in text)
        {
            endText.text += c;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
