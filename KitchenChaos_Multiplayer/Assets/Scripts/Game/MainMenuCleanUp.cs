using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos_Multiplayer.Game
{
    public class MainMenuCleanUp : MonoBehaviour
    {
        private void Awake()
        {
            if (NetworkManager.Singleton != null)
            {
                Destroy(NetworkManager.Singleton.gameObject);
            }
            if (KitchenGameMultiplayer.Instance != null)
            {
                Destroy(KitchenGameMultiplayer.Instance.gameObject);
            }
        }
    }
}
