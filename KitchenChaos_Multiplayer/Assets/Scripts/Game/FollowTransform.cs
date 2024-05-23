using UnityEngine;

namespace KitchenChaos_Multiplayer.Game
{
    public class FollowTransform : MonoBehaviour
    {
        #region Content

        private Transform targetTransform;

        #endregion

        #region Set: TargetTransform

        public void SetTargetTransform(Transform targetTransform)
        {
            this.targetTransform = targetTransform;
        }

        #endregion

    
        #region Unity: LateUpdate

        private void LateUpdate()
        {
            if (targetTransform == null)
            {
                return;
            }
        
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }

        #endregion
    }
}