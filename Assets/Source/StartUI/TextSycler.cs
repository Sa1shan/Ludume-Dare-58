using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Source.StartUI
{
    public class TextSycler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textField; // Сюда перетаскиваешь TMP Text
        [SerializeField] private Button continueButton;
        [TextArea(5, 15)]
        [SerializeField] private List<string> texts = new List<string>(); // Сюда записываешь фразы
        private int currentIndex = 0;

        private void Start()
        {
            if (texts.Count > 0)
            {
                textField.text = texts[currentIndex];
            }
            continueButton.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) // ЛКМ
            {
                NextText();
            }
        }

        private void NextText()
        {
            currentIndex++;

            if (currentIndex < texts.Count)
            {
                textField.text = texts[currentIndex];
            }
            else
            {
                textField.text = string.Empty;
                OnTextEnd();
            }
        }

        private void OnTextEnd()
        {
            continueButton.gameObject.SetActive(true);
        }
    }
}
