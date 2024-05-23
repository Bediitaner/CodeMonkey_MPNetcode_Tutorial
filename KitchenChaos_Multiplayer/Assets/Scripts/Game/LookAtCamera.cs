using UnityEngine;

namespace KitchenChaos_Multiplayer.Game
{
    public class LookAtCamera : MonoBehaviour
    {
        #region Contents

        [SerializeField] private Mode mode;

        #endregion

        #region Unity: LateUpdate

        private void LateUpdate()
        {
            switch (mode)
            {
                case Mode.LookAt:
                    transform.LookAt(Camera.main.transform);
                    break;
                case Mode.LookAtInverted:
                    Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                    transform.LookAt(transform.position + dirFromCamera);
                    break;
                case Mode.CameraForward:
                    transform.forward = Camera.main.transform.forward;
                    break;
                case Mode.CameraForwardInverted:
                    transform.forward = -Camera.main.transform.forward;
                    break;
            }
        }

        #endregion
    }
}