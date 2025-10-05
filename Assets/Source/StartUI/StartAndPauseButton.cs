using UnityEngine;

namespace Source.StartUI
{
    public class StartAndPauseButton : MonoBehaviour
    {
        public void OnClick()
        {
            Time.timeScale = 1;
        }
    }
}
