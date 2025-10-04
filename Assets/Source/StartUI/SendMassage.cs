using UnityEngine;

namespace Source.StartUI
{
    public class SendMassge : MonoBehaviour
    {
        private bool _isclicked = false;
        public void OnClick()
        {
            _isclicked = true;
            Debug.Log(_isclicked);
        }
    }
}
