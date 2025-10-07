using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace Source.GamePlayUI
{
    public class PagerController : MonoBehaviour
    { 
        [Header("Pager и текст")]
        [SerializeField] private RectTransform pager;
        [SerializeField] private TextMeshProUGUI tmPro;
        [SerializeField] private string notification;
        [SerializeField] private TextMeshProUGUI notificationTmPro;

        [TextArea(5, 15)]
        [SerializeField] private List<string> pages = new List<string>();

        [Header("Настройки анимации текста")]
        [SerializeField] private float animationSpeed;
        [SerializeField] private float notificationAnimSpeed;

        [Header("Настройки анимации Pager")]
        [SerializeField] private Vector2 startPosition;
        [SerializeField] private Vector2 endPosition;
        [SerializeField] private float pagerAnimationDuration;
        [SerializeField] private float pagerAnimationDelay;

        private int currentPageIndex = 0;
        private bool isAnimatingText = false;

        void Start()
        {
            pager.gameObject.SetActive(false);
            notificationTmPro.gameObject.SetActive(false);

            // Показываем сразу нулевое сообщение
            if (pages.Count > 0)
            {
                tmPro.text = pages[0];
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (!pager.gameObject.activeSelf)
                {
                    // Показываем Pager
                    pager.gameObject.SetActive(true);
                    pager.anchoredPosition = startPosition;
                    AnimatePager(true); // true — вылет
                }
                else
                {
                    // Скрываем Pager
                    AnimatePager(false); // false — возврат
                }
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                Notificaion();
            }
        }

        private void AnimatePager(bool isEntering)
        {
            Vector2 targetPos = isEntering ? endPosition : startPosition;

            // Перед анимацией текста очищаем TMP, если Pager появляется
            if (isEntering && tmPro != null)
            {
                tmPro.text = "";
            }

            pager.DOAnchorPos(targetPos, pagerAnimationDuration)
                .SetDelay(pagerAnimationDelay)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    if (!isEntering)
                    {
                        pager.gameObject.SetActive(false);
                    }
                    else
                    {
                        // Анимация текста только когда Pager на конечной позиции
                        ShowMessage(currentPageIndex);
                    }
                });
        }

        private void ShowMessage(int index)
        {
            if (index < 0 || index >= pages.Count) return;

            AnimateText(tmPro, pages[index]);
        }

        private void AnimateText(TextMeshProUGUI textTMP, string text)
        {
            if (textTMP == null) return;

            isAnimatingText = true;
            textTMP.text = "";

            float delay = pagerAnimationDelay; // используем тот же delay для текста

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                DOVirtual.DelayedCall(delay, () =>
                {
                    textTMP.text += c;
                });

                if (c != ' ')
                {
                    delay += animationSpeed;
                }
            }

            DOVirtual.DelayedCall(delay, () => { isAnimatingText = false; });
        }

        private void Notificaion()
        {
            notificationTmPro.gameObject.SetActive(true);
            notificationTmPro.DOFade(1f,notificationAnimSpeed);
        }

        private void NextMessage()
        {
            // Здесь позже добавишь логику для перехода к следующему сообщению
        }
    }
}
