using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Source.GamePlayUI;
using UnityEngine.UI;

public class TaskBarController : MonoBehaviour
{
    public static TaskBarController Instance;
    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private List<string> texts = new List<string>();
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Image taskBar;
    private PagerController _pagerController;

    [HideInInspector] public bool taskBarWasClosed;
    private int currentIndex = 0;
    private bool lastPagerState = false; // Для отслеживания изменения состояния

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _pagerController = PagerController.Instance;
    }

    private void Update()
    {
        // Проверяем, изменилось ли состояние pagerWasOpen
        if (_pagerController.pagerWasOpen != lastPagerState)
        {
            if (_pagerController.pagerWasOpen && !taskBarWasClosed)
            {
                ShowNextText();
            }
            lastPagerState = _pagerController.pagerWasOpen;
        }
        if (taskBarWasClosed == lastPagerState)
        {
            HideText();
        }
    }

    private void ShowNextText()
    {
        if (texts.Count == 0 || textDisplay == null)
            return;

        // Берем текущий элемент
        textDisplay.text = texts[currentIndex];

        // Увеличиваем индекс (по кругу)
        currentIndex = (currentIndex + 1) % texts.Count;

        
        // Анимация появления
        taskBar.DOFade(1f, fadeDuration);
        textDisplay.DOFade(1f, fadeDuration);
    }

    private void HideText()
    {
        if (textDisplay == null)
            return;

        // Анимация исчезновения
        taskBar.DOFade(0f, fadeDuration);
        textDisplay.DOFade(0f, fadeDuration);
    }

    // Этот метод можно вызвать из другого скрипта
    public void SetPagerState(bool state)
    {
        _pagerController.pagerWasOpen = state;
    }
}