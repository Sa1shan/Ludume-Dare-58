using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Source.GamePlayUI;
using UnityEngine.Playables;

namespace Source._2DInteractive
{
    public class InteractiableController : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private GameObject player;
        
        [Header("Interactiable")]
        [SerializeField] private Image blackBackground;
        public List<Button> buttons = new List<Button>();
        [SerializeField] private Image background;

        [Header("Float")]
        [SerializeField] private float delay;   
        [SerializeField] private float fadeDuration;
        
        [Header("Timeline")]
        [SerializeField] private PlayableDirector cutScene;
        public PlayableDirector closeCutscene;
        
        private bool _iskeypressed = false;
        [HideInInspector] public Rigidbody playerRb;

        [SerializeField] private DoorInteractor doorInteractor;
        private MusicFader _musicFader;
        private DialogueSystem _dialogueSystem;
        
        private void Start()
        {
            _musicFader = GetComponent<MusicFader>();
            playerRb = player.GetComponent<Rigidbody>();
            blackBackground.gameObject.SetActive(false);
            background.gameObject.SetActive(false);
            _dialogueSystem = GetComponent<DialogueSystem>();
            
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
            StartInteraction();
        }

        private void Update()
        {
            if (background.gameObject.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void StartInteraction()
        {
            if (_dialogueSystem.dialogue4)
            {
                _musicFader.ChangeMusic();
            }
            doorInteractor.DoorIndexAddition();
            
            if (_iskeypressed) return; // Чтобы случайно не запускать дважды
            _iskeypressed = true;

            blackBackground.gameObject.SetActive(true);
            background.gameObject.SetActive(true);

            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(true);
            }
            

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