using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace Source._2DInteractive
{
    public class InteractiableController : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private GameObject player;

        [Header("Interactiable")]
        [SerializeField] private Image blackbackground;
        [SerializeField] private List<Button> buttons = new List<Button>();

        [Header("Float")]
        [SerializeField] private float delay;   
        [SerializeField] private float fadeDuration;

        [FormerlySerializedAs("cutScene_1")]
        [FormerlySerializedAs("timeline")]
        [Header("Timelines")]
        [SerializeField] private PlayableDirector cutScene1;
        [SerializeField] private PlayableDirector cutScene2;
        [SerializeField] private PlayableDirector cutScene3;
        [SerializeField] private PlayableDirector cutScene4;
        private bool _iskeypressed = false;
        private Rigidbody _playerRb;

        private void Start()
        {
            _playerRb = player.GetComponent<Rigidbody>();
            blackbackground.gameObject.SetActive(false);

            // Устанавливаем прозрачность фона
            Color bgColor = blackbackground.color;
            bgColor.a = 1f;
            blackbackground.color = bgColor;

            // Прячем кнопки
            foreach (Button button in buttons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                    Image btnImage = button.GetComponent<Image>();
                    if (btnImage != null)
                    {
                        Color btnColor = btnImage.color;
                        btnColor.a = 0f;
                        btnImage.color = btnColor;
                    }
                }
            }

            // Подписка на событие завершения Timeline
            
                cutScene1.stopped += OnCutSceneFinished;
                cutScene2.stopped += OnCutSceneFinished;
                cutScene3.stopped += OnCutSceneFinished;
                cutScene4.stopped += OnCutSceneFinished;
        }

        private void OnDisable()
        {
            if (cutScene1 != null)
            {
                cutScene1.stopped -= OnCutSceneFinished;
            }
        }

        private void OnCutSceneFinished(PlayableDirector director)
        {
            // Вызываем ту же логику, что раньше запускалась по E
            StartInteraction();
        }

        private void StartInteraction()
        {
            if (_iskeypressed) return; // Чтобы случайно не запускать дважды
            _iskeypressed = true;

            blackbackground.gameObject.SetActive(true);
            _playerRb.constraints = RigidbodyConstraints.FreezeAll;

            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(true);
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            DOVirtual.DelayedCall(delay, () =>
            {
                blackbackground.DOFade(0f, fadeDuration).OnComplete(() =>
                {
                    foreach (var button in buttons)
                    {
                        button.image.DOFade(1f, fadeDuration).OnComplete(() =>
                        {
                            Time.timeScale = 0f;
                        });
                    }
                });
            });
        }

        // Можно оставить Update() для отладки или временно убрать
        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.E))
        //     {
        //         StartInteraction();
        //     }
        // }
    }
}