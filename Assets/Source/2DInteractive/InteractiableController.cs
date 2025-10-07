using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Playables;

namespace Source._2DInteractive
{
    public class InteractiableController : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private GameObject player;
        
        [Header("Interactiable")]
        [SerializeField] private Image blackBackground;
        [SerializeField] private List<Button> buttons = new List<Button>();
        [SerializeField] private Image background;

        [Header("Float")]
        [SerializeField] private float delay;   
        [SerializeField] private float fadeDuration;
        
        [Header("Timeline")]
        [SerializeField] private PlayableDirector cutScene;
        public PlayableDirector closeCutscene;
        
        private bool _iskeypressed = false;
        [HideInInspector] public Rigidbody playerRb;
        
        private void Start()
        {
            playerRb = player.GetComponent<Rigidbody>();
            blackBackground.gameObject.SetActive(false);
            background.gameObject.SetActive(false);

            // Устанавливаем прозрачность фона
            Color bgColor = blackBackground.color;
            bgColor.a = 1f;
            blackBackground.color = bgColor;

            // Прячем кнопки
            foreach (Button button in buttons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                }
            }
            

            // Подписка на событие завершения Timeline
                cutScene.stopped += OnCutSceneFinished;
        }

        private void OnDisable()
        {
            if (cutScene != null)
            {
                cutScene.stopped -= OnCutSceneFinished;
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

            blackBackground.gameObject.SetActive(true);
            background.gameObject.SetActive(true);

            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(true);
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            DOVirtual.DelayedCall(delay, () =>
            {
                blackBackground.DOFade(0f, fadeDuration).OnComplete(() =>
                {
                    blackBackground.gameObject.SetActive(false);
                    Time.timeScale = 0f;
                });
            });
        }
    }
}