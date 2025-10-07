using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

namespace Source.GamePlayUI
{
    public class PagerController : MonoBehaviour
    { 
        public static PagerController Instance { get; private set; }
        
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
        private Vector3 _playerStartPosition;


        void Awake()
        {
            pager.gameObject.SetActive(false);
            notificationTmPro.gameObject.SetActive(false);

            // Показываем сразу нулевое сообщение
            if (pages.Count > 0)
            {
                tmPro.text = pages[0];
            }
            _playerStartPosition = player.transform.position;
            Instance = this;
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

        public void NextMessage()
        {
            // Если уже проигрывается анимация текста — не трогаем ничего
            if (_isAnimatingText) return;

            // Если нет страниц — выходим
            if (pages == null || pages.Count == 0) return;

            // Если есть следующая страница — двигаем индекс, иначе ничего не делаем
            if (_currentPageIndex >= pages.Count - 1)
            {
                // Можно раскомментировать следующую строку, чтобы зациклить лист:
                // _currentPageIndex = 0;
                return;
            }

            _currentPageIndex++;

            // Очистим текст сразу, чтобы не было "нахлёста" старого текста
            if (tmPro != null)
                tmPro.text = "";

            // Если Pager сейчас не показан — показываем его и дочерняя логика
            // (AnimatePager(true) после завершения вызовет ShowMessage с текущим индексом)
            if (pager.gameObject.activeSelf)
            {
                // Pager уже открыт — просто показываем сообщение с анимацией
                ShowMessage(_currentPageIndex);
            }
        }

    }
}
