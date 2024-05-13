using System;

namespace Counters
{
    public class TrashCounter : BaseCounter
    {
        #region Events

        public static event EventHandler OnAnyObjectTrashed;

        #endregion

        #region Reset: StaticData

        new public static void ResetStaticData()
        {
            OnAnyObjectTrashed = null;
        }

        #endregion


        #region Override: Interact

        public override void Interact(Player player)
        {
            if (player.HasKitchenObject())
            {
                player.GetKitchenObject().DestroySelf();

                OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}