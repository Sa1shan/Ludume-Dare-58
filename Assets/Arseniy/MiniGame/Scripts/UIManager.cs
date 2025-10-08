using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arseniy.MiniGame.Scripts
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("UI Elements")]
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI timerText;
        public Button startButton;
        public Button restartButton;      // кнопка на EndPanel
        public GameObject endPanel;
        public TextMeshProUGUI endText;

        [Header("Gameplay UI Group (все элементы, которые должны появиться после старта)")]
        public GameObject gameplayUIGroup; // можно сделать пустой GameObject, собрать туда score/timer и др.

        [Header("Penalty feedback")]
        public TextMeshProUGUI penaltyFeedbackText; // текст рядом с таймером/поинтами
        public float feedbackDuration = 1f;

        private Coroutine feedbackCoroutine;
        [SerializeField] private GameObject exitButton;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            // подписки
            if (startButton) startButton.onClick.AddListener(OnStartClicked);
            if (restartButton) restartButton.onClick.AddListener(OnRestartClicked);

            // В начале видна только StartButton, всё остальное скрываем
            if (gameplayUIGroup != null) gameplayUIGroup.SetActive(false);
            if (endPanel != null) endPanel.SetActive(false);

            // score/timer можно инициализировать нулями
            UpdateScore(0);
            UpdateTimer(0);

            // прячу feedback сразу
            if (penaltyFeedbackText != null) penaltyFeedbackText.alpha = 0f;
        }

        public void OnStartClicked()
        {
            // спрячем StartButton и покажем остальные элементы UI
            if (startButton) startButton.gameObject.SetActive(false);
            ShowGameplayUI(true);
            endPanel.SetActive(false);
            var g = FindObjectOfType<QWEGame>();
            if (g != null) g.StartGame();
        }

        public void OnRestartClicked()
        {
            // скрываем EndPanel и перезапускаем игру
            if (endPanel) endPanel.SetActive(false);
            if (startButton) startButton.gameObject.SetActive(false);
            ShowGameplayUI(true);

            var g = FindObjectOfType<QWEGame>();
            if (g != null) g.RestartGame();
        }

        // показать/скрыть игровые элементы (кроме StartButton)
        public void ShowGameplayUI(bool show)
        {
            if (gameplayUIGroup != null) gameplayUIGroup.SetActive(show);
        }

        public void UpdateScore(int score)
        {
            if (scoreText) scoreText.text = $"Очки: {score}";
        }

        public void UpdateTimer(float seconds)
        {
            if (!timerText) return;
            int s = Mathf.CeilToInt(seconds);
            timerText.text = $"Время: {s}s";
        }

        public void ShowEnd(bool win)
        {
            if (endPanel) endPanel.SetActive(true);

            if (endText) endText.text = win ? "Победа!" : "Проигрыш";

            if (win)
            {
                exitButton.gameObject.SetActive(true);
            }
            
            if (!win)
            {
                startButton.gameObject.SetActive(true);
            }
            
            // скрываем игровую UI (чтобы не мешала)
            ShowGameplayUI(false);
        }

        // feedback: pointsDelta обычно отрицательное (напр. -1), timeDelta отрицательное в секундах (напр. -3)
        public void ShowPenaltyFeedback(int pointsDelta, float timeDelta)
        {
            if (penaltyFeedbackText == null) return;

            string pts = pointsDelta < 0 ? $"{pointsDelta} очк." : $"+{pointsDelta} очк.";
            string t = timeDelta < 0 ? $"{timeDelta}s" : $"+{timeDelta}s";
            penaltyFeedbackText.text = $"{pts}  {t}";

            if (feedbackCoroutine != null) StopCoroutine(feedbackCoroutine);
            feedbackCoroutine = StartCoroutine(DoFadeFeedback(penaltyFeedbackText, feedbackDuration));
        }

        private IEnumerator DoFadeFeedback(TextMeshProUGUI txt, float dur)
        {
            // плавно показываем и скрываем за dur сек (половина на появление, половина на исчезание)
            float half = Mathf.Max(0.05f, dur / 2f);
            // появление
            for (float t = 0f; t < half; t += Time.deltaTime)
            {
                float a = Mathf.Clamp01(t / half);
                SetAlpha(txt, a);
                yield return null;
            }
            SetAlpha(txt, 1f);

            // пауза (если есть оставшееся время)
            float pause = Mathf.Max(0f, dur - 2f * half);
            if (pause > 0f) yield return new WaitForSeconds(pause);

            // исчезание
            for (float t = 0f; t < half; t += Time.deltaTime)
            {
                float a = 1f - Mathf.Clamp01(t / half);
                SetAlpha(txt, a);
                yield return null;
            }
            SetAlpha(txt, 0f);
        }

        private void SetAlpha(TextMeshProUGUI txt, float alpha)
        {
            if (txt == null) return;
            var c = txt.color;
            c.a = alpha;
            txt.color = c;
        }
    }
}
