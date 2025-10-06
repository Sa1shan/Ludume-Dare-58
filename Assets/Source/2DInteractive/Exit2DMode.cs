using UnityEngine;

namespace Source._2DInteractive
{
    public class Exit2DMode : MonoBehaviour
    {
        public void OnClickExit()
        {
            Debug.Log("Exit");
            Time.timeScale = 1;
        }
    }
}
