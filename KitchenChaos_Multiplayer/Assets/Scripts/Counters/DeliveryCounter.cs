namespace Counters
{
    public class DeliveryCounter : BaseCounter
    {
        #region Singleton

        public static DeliveryCounter Instance { get; private set; }

        #endregion


        #region Unity: Awake

        private void Awake()
        {
            Instance = this;
        }

        #endregion

        #region Override: Interact

        public override void Interact(Player player)
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Only accepts Plates

                    DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

                    KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                }
            }
        }

        #endregion
    }
}