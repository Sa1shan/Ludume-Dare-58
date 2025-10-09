using UnityEngine;
using System.Collections;
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

        [Header("Настройки страниц")]
        [SerializeField] private List<PageData> pages = new List<PageData>();

        [Header("Настройки анимации текста")]
        [SerializeField] private float letterAnimationSpeed = 0.05f; // Скорость появления каждой буквы
        [SerializeField] private float delayBetweenPages = 1f; // Задержка между страницами
        [SerializeField] private float notificationAnimSpeed;

        [Header("Настройки анимации Pager")]
        [SerializeField] private Vector2 startPosition;
        [SerializeField] private Vector2 endPosition;
        [SerializeField] private float pagerAnimationDuration;
        [SerializeField] private float pagerAnimationDelay;

        private int _currentPageIndex = 0;
        private bool _isAnimatingText = false;
        private Vector3 _playerStartPosition;
        private bool _initialSequenceCompleted = false; // Флаг завершения начальной последовательности
        private Coroutine _pageSequenceCoroutine; // Для управления последовательностью страниц
        private bool _isFirstOpen = true; // Флаг первого открытия пейджера
        private bool _isPagerAnimating = false; // Флаг анимации пейджера (открытие/закрытие)

        [System.Serializable]
        public class PageData
        {
            [TextArea(5, 15)]
            public string text; // Текст страницы
            public bool showInInitialSequence; // Галочка для показа в начальной последовательности
        }

        void Awake()
        {
            pager.gameObject.SetActive(false);
            notificationTmPro.gameObject.SetActive(false);

            if (pages.Count > 0)
            {
                tmPro.text = "";
            }
            _playerStartPosition = player.transform.position;
            Instance = this;
        }

        void Start()
        {
            ShowNotification();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (_isAnimatingText || _isPagerAnimating) return;

                if (!pager.gameObject.activeSelf)
                {
                    pager.gameObject.SetActive(true);
                    pager.anchoredPosition = startPosition;
                    AnimatePager(true);
                    notificationTmPro.gameObject.SetActive(false);
                }
                else
                {
                    AnimatePager(false);
                    _isFirstOpen = false;
                }
            }

            notificationTmPro.text = notification;
        }

        private void AnimatePager(bool isEntering)
        {
            _isPagerAnimating = true;
            Vector2 targetPos = isEntering ? endPosition : startPosition;

            if (isEntering && tmPro != null)
            {
                tmPro.text = "";
            }

            pager.DOAnchorPos(targetPos, pagerAnimationDuration)
                .SetDelay(pagerAnimationDelay)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    _isPagerAnimating = false;

                    if (!isEntering)
                    {
                        pager.gameObject.SetActive(false);
                    }
                    else
                    {
                        // Если это первое открытие и есть галочки для начальной последовательности
                        if (_isFirstOpen && !_initialSequenceCompleted)
                        {
                            StartInitialPageSequence();
                        }
                        else
                        {
                            // Любой текущий текст (галочка или нет) теперь анимируется по буквам
                            if (_currentPageIndex < pages.Count)
                            {
                                StartCoroutine(ShowPageWithAnimation(pages[_currentPageIndex].text));
                            }
                        }
                    }
                });
        }

        private void StartInitialPageSequence()
        {
            if (_pageSequenceCoroutine != null)
            {
                StopCoroutine(_pageSequenceCoroutine);
            }
            _pageSequenceCoroutine = StartCoroutine(PageSequenceRoutine());
        }

        private IEnumerator PageSequenceRoutine()
        {
            // Берем только страницы с галочкой
            List<PageData> initialPages = pages.FindAll(page => page.showInInitialSequence);

            if (initialPages.Count == 0)
            {
                _initialSequenceCompleted = true;
                yield break;
            }

            for (int i = 0; i < initialPages.Count; i++)
            {
                yield return StartCoroutine(ShowPageWithAnimation(initialPages[i].text));
                _currentPageIndex = pages.IndexOf(initialPages[i]);

                if (i < initialPages.Count - 1)
                {
                    yield return new WaitForSeconds(delayBetweenPages);
                }
            }

            _initialSequenceCompleted = true;
        }

        private IEnumerator ShowPageWithAnimation(string text)
        {
            _isAnimatingText = true;
            tmPro.text = "";

            for (int i = 0; i < text.Length; i++)
            {
                tmPro.text += text[i];
                if (text[i] != ' ')
                {
                    yield return new WaitForSeconds(letterAnimationSpeed);
                }
            }

            _isAnimatingText = false;
        }

        public void ShowNotification()
        {
            notificationTmPro.gameObject.SetActive(true);
            notificationTmPro.DOFade(1f, notificationAnimSpeed);
        }

        public void NextMessage()
        {
            if (_isAnimatingText || _isPagerAnimating) return;
            if (pages == null || pages.Count == 0) return;

            if (_currentPageIndex >= pages.Count - 1) return;

            _currentPageIndex++;

            if (pager.gameObject.activeSelf)
            {
                if (_isFirstOpen && !_initialSequenceCompleted && pages[_currentPageIndex].showInInitialSequence)
                {
                    StartCoroutine(ShowPageWithAnimation(pages[_currentPageIndex].text));
                }
                else
                {
                    tmPro.text = pages[_currentPageIndex].text;
                }
            }
        }

        public void PreviousMessage()
        {
            if (_isAnimatingText || _isPagerAnimating) return;
            if (pages == null || pages.Count == 0) return;

            if (_currentPageIndex <= 0) return;

            _currentPageIndex--;

            if (pager.gameObject.activeSelf)
            {
                if (_isFirstOpen && !_initialSequenceCompleted && pages[_currentPageIndex].showInInitialSequence)
                {
                    StartCoroutine(ShowPageWithAnimation(pages[_currentPageIndex].text));
                }
                else
                {
                    tmPro.text = pages[_currentPageIndex].text;
                }
            }
        }

        public void OnExitButton()
        {
            if (_isAnimatingText || _isPagerAnimating) return;

            if (pager.gameObject.activeSelf)
            {
                AnimatePager(false);
                _isFirstOpen = false;
            }
        }
    }
}
