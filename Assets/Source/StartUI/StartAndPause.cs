using System;
using UnityEngine;

namespace Source.StartUI
{
    public class StartAndPause : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        private void Start()
        {
            Time.timeScale = 1;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 0;
            }
        }

        public void OnClick()
        {
            Time.timeScale = 1;
        }
    }
}
