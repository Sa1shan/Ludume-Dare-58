using System.Collections.Generic;
using Arseniy.MiniGame.Scripts;
using DG.Tweening;
using Source._2DInteractive;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Source.GamePlayUI
{
    [System.Serializable]
    public class DialogLine
    {
        public string name;                         // Имя персонажа
        [TextArea(5, 15)] public string text;       // Текст диалога
    }

    public class DialogueSystem : MonoBehaviour
    {
        [Header("Привязки к TextMeshProUGUI")]
        [SerializeField] private TextMeshProUGUI tmp1;      // Для обычных текстов
        [SerializeField] private TextMeshProUGUI nameTmp1;  // Для имен

        [Header("Настройки анимации")]
        [SerializeField] private float animationSpeed = 0.05f; // Задержка между буквами

        [Header("Диалоги")]
        [SerializeField] private List<DialogLine> dialogueText = new List<DialogLine>();

        [SerializeField] private GameObject dialogueBackground;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button startButtonStartMinigame;
        [SerializeField] private bool dialogue4;
        [HideInInspector] public bool dialogue4IsFinish;

        private bool _firstLineShown = false;
        private int currentIndex = 0;      // текущий индекс пары
        private bool isAnimating = false;  // флаг анимации текста

        private UIManager _uiManager;

        private List<Tween> activeTweens = new List<Tween>(); // все текущие DelayedCall
        private TextMeshProUGUI currentTMP; // текущий текстовый TMP для анимации
        private string currentFullText;     // полный текст текущей строки

        private void Start()
        {
            tmp1.text = "";
            nameTmp1.text = "";
            _uiManager = UIManager.Instance;

            // Можно повесить кнопку выхода на SkipAnimation
            if (exitButton != null)
            {
                exitButton.onClick.AddListener(SkipAnimation);
            }
        }

        private void Update()
        {
            if (dialogueBackground.activeSelf)
            {
                // Показываем первый элемент один раз
                if (!_firstLineShown && dialogueText.Count > 0)
                {
                    ShowNextPair(auto: true);
                    _firstLineShown = true;
                }

                // ЛКМ: пропуск анимации или переход к следующему элементу
                if (Input.GetMouseButtonDown(0))
                {
                    SkipAnimation();
                }
            }
        }

        private void ShowNextPair(bool auto = false)
        {
            if (currentIndex >= dialogueText.Count)
            {
                OnDialogEnd();
                return;
            }

            DialogLine line = dialogueText[currentIndex];

            nameTmp1.text = line.name;
            AnimateText(tmp1, line.text);

            if (!auto)
            {
                currentIndex++;
            }
            else
            {
                currentIndex = 1; // первый элемент уже показан
            }
        }

        private void SkipAnimation()
        {
            if (isAnimating)
            {
                // Отменяем все DelayedCall
                foreach (var tween in activeTweens)
                {
                    tween.Kill();
                }
                activeTweens.Clear();

                // Показываем весь текст сразу
                if (currentTMP != null)
                {
                    currentTMP.text = currentFullText;
                }

                isAnimating = false;
            }
            else
            {
                // Переходим к следующей строке
                ShowNextPair();
            }
        }

        private void AnimateText(TextMeshProUGUI textTMP, string text)
        {
            if (textTMP == null) return;

            isAnimating = true;
            textTMP.text = "";
            currentTMP = textTMP;
            currentFullText = text;

            float delay = 0f;
            activeTweens.Clear();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                Tween t = DOVirtual.DelayedCall(delay, () =>
                {
                    textTMP.text += c;
                });
                activeTweens.Add(t);

                if (c != ' ')
                {
                    delay += animationSpeed;
                }
            }

            Tween finalTween = DOVirtual.DelayedCall(delay, () =>
            {
                isAnimating = false;
                activeTweens.Clear();
            });
            activeTweens.Add(finalTween);
        }

        private void OnDialogEnd()
        {
            dialogueBackground.SetActive(false);

            if (!dialogue4)
                startButtonStartMinigame.gameObject.SetActive(true);

            if (dialogue4)
            {
                dialogue4IsFinish = true;
                Time.timeScale = 1;
            }

            if (!dialogue4IsFinish)
            {
                var _interactiableController = GetComponent<InteractiableController>();
                foreach (var btn in _interactiableController.buttons)
                {
                    if (btn == null) continue;
                    btn.onClick = new Button.ButtonClickedEvent(); // полностью очищаем события
                }
            }
        }

        public void RevertTimeScale()
        {
            Time.timeScale = 1;
        }
    }
}
