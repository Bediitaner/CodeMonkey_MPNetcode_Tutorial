using KitchenChaos_Multiplayer.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GamePlayingClockUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private Image timerImage;
        
        #endregion

        #region Unity: Update

        private void Update()
        {
            timerImage.fillAmount = KitchenGameManager.Instance.GetGamePlayingTimerNormalized();
        }

        #endregion
    }
}