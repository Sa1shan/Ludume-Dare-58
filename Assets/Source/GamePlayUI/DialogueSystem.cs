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

        private InteractiableController _interactiableController;
        
        private bool _firstLineShown = false;

        private int currentIndex = 0;      // текущий индекс пары
        private bool isAnimating = false;  // блокировка ЛКМ

        private UIManager _uiManager;
        
        private void Start()
        {
            _interactiableController = GetComponent<InteractiableController>();
            tmp1.text = "";
            nameTmp1.text = "";
            _uiManager = UIManager.Instance;
        }

        private void Update()
        {
            if (dialogueBackground.gameObject.activeSelf)
            {
                // Показываем первый элемент один раз, когда объект активен
                if (!_firstLineShown)
                {
                    if (dialogueText.Count > 0)
                    {
                        ShowNextPair(auto: true);
                        _firstLineShown = true;
                    }
                }

                // Все последующие строки по ЛКМ
                if (Input.GetMouseButtonDown(0) && !isAnimating && _firstLineShown)
                {
                    ShowNextPair();
                }
            }
        }

        // Немного изменяем ShowNextPair
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

        private void AnimateText(TextMeshProUGUI textTMP, string text)
        {
            if (textTMP == null) return;

            isAnimating = true;
            textTMP.text = "";

            float delay = 0f;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                DOVirtual.DelayedCall(delay, () =>
                {
                    textTMP.text += c;
                });

                // задержка только если символ не пробел
                if (c != ' ')
                {
                    delay += animationSpeed;
                }
            }

            // После окончания анимации разрешаем ЛКМ
            DOVirtual.DelayedCall(delay, () =>
            {
                isAnimating = false;
            });
        }


        private void OnDialogEnd()
        {
            dialogueBackground.gameObject.SetActive(false); 
            if(!dialogue4) startButtonStartMinigame.gameObject.SetActive(true);

            if (dialogue4)
            {
                dialogue4IsFinish = true;
                Time.timeScale = 1;
                
                //
            }
            if(!dialogue4IsFinish)
            {
                foreach (var btn in _interactiableController.buttons)
                {
                    if (btn == null) continue;
                    // Полностью переинициализируем событие, чтобы удалить ВСЕ ссылки (включая добавленные в инспекторе)
                    btn.onClick = new Button.ButtonClickedEvent();
                }
            }
            
        }

        public void RevertTimeScale()
        {
            Time.timeScale = 1;
        }
    }
}
