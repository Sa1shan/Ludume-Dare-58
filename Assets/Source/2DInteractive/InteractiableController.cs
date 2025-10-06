using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

            // Проходим по каждой кнопке
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
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _iskeypressed = true;
                blackbackground.gameObject.SetActive(true);
                
                _playerRb.constraints = RigidbodyConstraints.FreezeAll;
                
                foreach (Button button in buttons)
                {
                    button.gameObject.SetActive(true);
                }
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                
                // Задержка перед анимацией в 1 секунду
                DOVirtual.DelayedCall(delay, () =>
                {
                    // После задержки запускаем анимацию альфа-канала
                    blackbackground.DOFade(0f, fadeDuration).OnComplete(() =>
                    {
                        foreach (var button in buttons)
                        {
                            button.image.DOFade(1f, fadeDuration).OnComplete(() => Time.timeScale = 0f);
                        }
                    });
                });
            }
        }
    }
}
