using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Source.TextUI
{
    public class DialogSystem : MonoBehaviour
    {
        public string[] texts;
        [Header("Привязки к TextMeshProUGUI")]
        [SerializeField] private TextMeshProUGUI tmp1;       // Для обычных текстов
        [SerializeField] private TextMeshProUGUI nameTmp1;   // Для имен

        [Header("Имена")]
        [SerializeField] private string name1;
        [SerializeField] private string name2;
        [SerializeField] private string name3;
        [SerializeField] private string name4;
        [SerializeField] private string name5;
        [SerializeField] private string name6;
        [SerializeField] private string name7;
        [SerializeField] private string name8;
        [SerializeField] private string name9;
        [SerializeField] private string name10;
        
        [Header("Обычные строки")]
        [SerializeField] [TextArea(5, 15)] private string string1;
        [SerializeField] [TextArea(5, 15)] private string string2;
        [SerializeField] [TextArea(5, 15)] private string string3;
        [SerializeField] [TextArea(5, 15)] private string string4;
        [SerializeField] [TextArea(5, 15)] private string string5;
        [SerializeField] [TextArea(5, 15)] private string string6;
        [SerializeField] [TextArea(5, 15)] private string string7;
        [SerializeField] [TextArea(5, 15)] private string string8;
        [SerializeField] [TextArea(5, 15)] private string string9;
        [SerializeField] [TextArea(5, 15)] private string string10;
        
        private int currentIndex = 1;      // текущая пара
        private bool isAnimating = false;  // блокировка ЛКМ

        private void Start()
        {
            tmp1.text = "";
            nameTmp1.text = "";
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !isAnimating)
            {
                ShowNextPair();
            }
        }

        private void ShowNextPair()
        {
            string text = GetString(currentIndex);
            string name = GetName(currentIndex);

            if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(name))
            {
                AnimatePair(tmp1, text, nameTmp1, name);
                currentIndex++;
            }
        }

        private void AnimatePair(TextMeshProUGUI textTMP, string text, TextMeshProUGUI nameTMP, string name)
        {
            if (textTMP == null || nameTMP == null) return;

            isAnimating = true;

            textTMP.text = "";
            nameTMP.text = "";

            string[] textWords = text.Split(' ');
            string[] nameWords = name.Split(' ');

            float interval = 0.3f;
            float delay = 0f;

            int maxWords = Mathf.Max(textWords.Length, nameWords.Length);

            for (int i = 0; i < maxWords; i++)
            {
                int textIndex = i;
                int nameIndex = i;
                DOVirtual.DelayedCall(delay, () =>
                {
                    if (textIndex < textWords.Length)
                    {
                        textTMP.text += (textTMP.text.Length > 0 ? " " : "") + textWords[textIndex];
                    }
                    if (nameIndex < nameWords.Length)
                    {
                        nameTMP.text += (nameTMP.text.Length > 0 ? " " : "") + nameWords[nameIndex];
                    }
                });

                delay += interval;
            }

            // После окончания анимации пары разрешаем ЛКМ для следующей
            DOVirtual.DelayedCall(delay, () =>
            {
                isAnimating = false;
            });
        }

        private string GetString(int index)
        {
            switch (index)
            {
                case 1: return string1;
                case 2: return string2;
                case 3: return string3;
                case 4: return string4;
                case 5: return string5;
                case 6: return string6;
                case 7: return string7;
                case 8: return string8;
                case 9: return string9;
                case 10: return string10;
                default: return "";
            }
        }

        private string GetName(int index)
        {
            switch (index)
            {
                case 1: return name1;
                case 2: return name2;
                case 3: return name3;
                case 4: return name4;
                case 5: return name5;
                case 6: return name6;
                case 7: return name7;
                case 8: return name8;
                case 9: return name9;
                case 10: return name10;
                default: return "";
            }
        }
    }
}