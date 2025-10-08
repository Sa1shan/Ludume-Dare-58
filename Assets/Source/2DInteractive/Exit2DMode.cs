using System;
using Source.GamePlayUI;
using UnityEngine;
using UnityEngine.Playables;

namespace Source._2DInteractive
{
    public class Exit2DMode : MonoBehaviour
    {
        [SerializeField] private DoorInteractor doorInteractor;
        [SerializeField] private InteractiableController interactiableController;
        private PagerController _pagerController;
        

        private void Start()
        {
            _pagerController = PagerController.Instance;
        }

        public void OnClickExit()
        {
            _pagerController.NextMessage();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            
            interactiableController.gameObject.SetActive(false);
            
            Destroy(doorInteractor.openCutscene);
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
