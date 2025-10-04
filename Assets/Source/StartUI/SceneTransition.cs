using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source.StartUI
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private int sceneNumberToLoad;

        public void LoadScene()
        {
            SceneManager.LoadScene(sceneNumberToLoad);
        }
    }
}
