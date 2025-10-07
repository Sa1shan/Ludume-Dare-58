using UnityEngine;
using UnityEngine.Playables;

namespace Source._2DInteractive
{
    public class Exit2DMode : MonoBehaviour
    {
        [SerializeField] private DoorInteractor doorInteractor;
        [SerializeField] private InteractiableController interactiableController;
        public void OnClickExit()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            
            interactiableController.gameObject.SetActive(false);
            
            doorInteractor.openCutscene.gameObject.SetActive(false);
            doorInteractor.closeCutScene.gameObject.SetActive(true);
            doorInteractor.cinemachineCam.Priority = 10;
            
            Time.timeScale = 1;

            interactiableController.closeCutscene.stopped += OnCloseCutsceneStopped;
        }

        private void OnCloseCutsceneStopped(PlayableDirector director)
        {
            interactiableController.playerRb.isKinematic = false;
            
            doorInteractor.closeCutScene.gameObject.SetActive(false);
        }
    }
}
