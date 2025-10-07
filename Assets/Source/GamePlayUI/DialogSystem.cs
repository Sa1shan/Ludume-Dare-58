using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Source.GamePlayUI
{
    [System.Serializable]
    public class DialogLine
    {
        public string name;                         // Имя персонажа
        [TextArea(5, 15)] public string text;       // Текст диалога
    }

    public class DialogSystem : MonoBehaviour
    {
        [Header("Привязки к TextMeshProUGUI")]
        [SerializeField] private TextMeshProUGUI tmp1;      // Для обычных текстов
        [SerializeField] private TextMeshProUGUI nameTmp1;  // Для имен

        [Header("Настройки анимации")]
        [SerializeField] private float animationSpeed = 0.05f; // Задержка между буквами

        [Header("Диалоги")]
        [SerializeField] private List<DialogLine> dialogLines = new List<DialogLine>();

        [SerializeField] private GameObject dialogue;

        [SerializeField] private GameObject exitButton;
        
        private bool _firstLineShown = false;

        private int currentIndex = 0;      // текущий индекс пары
        private bool isAnimating = false;  // блокировка ЛКМ

        private void Start()
        {
            tmp1.text = "";
            nameTmp1.text = "";
        }

        private void Update()
        {
            if (dialogue.gameObject.activeSelf)
            {
                // Показываем первый элемент один раз, когда объект активен
                if (!_firstLineShown)
                {
                    if (dialogLines.Count > 0)
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
            if (currentIndex >= dialogLines.Count)
            {
                OnDialogEnd();
                return;
            }

            DialogLine line = dialogLines[currentIndex];

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
            dialogue.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(true);
        }
    }
}
