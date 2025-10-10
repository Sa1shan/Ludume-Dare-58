using UnityEngine;

namespace Source.StartUI
{
    public class StartAndPause : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private FirstPersonController player;
        private bool _isPaused = false;
        private void Start()
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        private void Pause()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            _isPaused = true;
            player.cameraCanMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Resume()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            _isPaused = false;
            player.cameraCanMove = true;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
    }
}
