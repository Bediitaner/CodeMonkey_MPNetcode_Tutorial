using Counters;
using KitchenChaos_Multiplayer.Game;
using UnityEngine;

namespace KitchenChaos_Multiplayer.Managers
{
    public class ResetStaticDataManager : MonoBehaviour
    {
        private void Awake()
        {
            CuttingCounter.ResetStaticData();
            BaseCounter.ResetStaticData();
            TrashCounter.ResetStaticData();
            Player.ResetStaticData();
        }
    }
}