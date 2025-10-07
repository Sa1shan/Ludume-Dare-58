using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace Source.GamePlayUI
{
    public class PagerController : MonoBehaviour
    { 
        [Header("Player")]
        [SerializeField] private GameObject player;
        
        [Header("Pager и текст")]
        [SerializeField] private RectTransform pager;
        [SerializeField] private TextMeshProUGUI tmPro;
        [TextArea(5, 15)]
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

        private int _currentPageIndex = 0;
        private bool _isAnimatingText = false;
        private int _currentIndex = 0;
        private Vector3 _playerStartPosition;


        void Start()
        {
            pager.gameObject.SetActive(false);
            notificationTmPro.gameObject.SetActive(false);

            // Показываем сразу нулевое сообщение
            if (pages.Count > 0)
            {
                tmPro.text = pages[0];
            }
            _playerStartPosition = player.transform.position;
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
                    // В будущем здесь будет вызов метода перехода к следующему сообщению:
                    // NextMessage();
                }
                notificationTmPro.gameObject.SetActive(false);
            }

            // if (Input.GetKeyDown(KeyCode.N))
            // {
            //     Notificaion();
            // }
            notificationTmPro.text = notification;
        }

        private void PlayerMoving()
        {
            if (player.transform.position != _playerStartPosition)
            {
                AnimatePager(false);
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
                        ShowMessage(_currentPageIndex);
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

            _isAnimatingText = true;
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

            DOVirtual.DelayedCall(delay, () => { _isAnimatingText = false; });
        }

        public void Notificaion()
        {
            notificationTmPro.gameObject.SetActive(true);
            notificationTmPro.DOFade(1f,notificationAnimSpeed);
        }

        private void NextMessage()
        {
            // 🔹 Логика для перехода к следующему уведомлению

            // 1️⃣ Проверяем, есть ли ещё страницы (уведомления) после текущей
            // Если следующая страница есть — увеличиваем индекс
            if (_currentPageIndex < pages.Count - 1)
            {
                _currentPageIndex++;

                // 2️⃣ Очищаем текст перед новой анимацией
                tmPro.text = "";

                // 3️⃣ Запускаем анимацию текста для следующей страницы
                ShowMessage(_currentPageIndex);
            }
        }
    }
}
