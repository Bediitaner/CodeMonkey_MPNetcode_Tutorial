using System;
using KitchenChaos_Multiplayer.Game;
using Unity.Netcode;
using UnityEngine;

namespace Counters
{
    public class CuttingCounter : BaseCounter, IHasProgress
    {
        #region Events

        public static event EventHandler OnAnyCutEvent;
        public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChangedEvent;
        public event EventHandler OnCutEvent;

        #endregion

        #region Contents

        [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

        #endregion

        #region Fields

        private int cuttingProgress;

        #endregion


        #region Reset: StaticData

        new public static void ResetStaticData()
        {
            OnAnyCutEvent = null;
        }

        #endregion


        #region Override: Interact

        public override void Interact(Player player)
        {
            if (!HasKitchenObject())
            {
                // There is no KitchenObject here
                if (player.HasKitchenObject())
                {
                    // Player is carrying something
                    if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                    {
                        // Player carrying something that can be Cut
                        KitchenObject kitchenObject = player.GetKitchenObject();
                        kitchenObject.SetKitchenObjectParent(this);

                        InteractLogicPlaceObjectServerRpc();
                    }
                }
                else
                {
                    // Player not carrying anything
                }
            }
            else
            {
                // There is a KitchenObject here
                if (player.HasKitchenObject())
                {
                    // Player is carrying something
                    if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                    {
                        // Player is holding a Plate
                        if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                        {
                            GetKitchenObject().DestroySelf();
                        }
                    }
                }
                else
                {
                    // Player is not carrying anything
                    GetKitchenObject().SetKitchenObjectParent(player);
                }
            }
        }

        #endregion

        #region Override: InteractAlternate

        public override void InteractAlternate(Player player)
        {
            if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
            {
                // There is a KitchenObject here AND it can be cut
                CutObjectServerRpc();
            }
        }

        #endregion

        
        #region ServerRpc: InteractLogicPlaceObject

        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicPlaceObjectServerRpc()
        {
            InteractLogicPlaceObjectClientRpc();
        }

        #endregion

        #region Client: InteractLogicPlaceObject
        [ClientRpc]
        private void InteractLogicPlaceObjectClientRpc()
        {
            cuttingProgress = 0;

            OnProgressChangedEvent?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0f
            });
        }

        #endregion


        #region ServerRpc: Cut: Object

        [ServerRpc(RequireOwnership = false)]
        private void CutObjectServerRpc()
        {
            CutObjectClientRpc();
            CuttingProgressServerRpc();
        }

        #endregion

        #region ClientRpc: Cut: Object

        [ClientRpc]
        private void CutObjectClientRpc()
        {
            cuttingProgress++;

            OnCutEvent?.Invoke(this, EventArgs.Empty);
            OnAnyCutEvent?.Invoke(this, EventArgs.Empty);

            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChangedEvent?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
            });
        }

        #endregion

        #region ServerRpc: CuttingProgress

        [ServerRpc(RequireOwnership = false)]
        private void CuttingProgressServerRpc()
        {
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

                KitchenObject.DestroyKitchenObject(GetKitchenObject());
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }
        }

        #endregion


        #region Has: RecipeWithInput

        private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
        {
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
            return cuttingRecipeSO != null;
        }

        #endregion

        #region Get: OutputForInput

        private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
        {
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
            if (cuttingRecipeSO != null)
            {
                return cuttingRecipeSO.output;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Get: CuttingRecipeSOWithInput

        private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
        {
            foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
            {
                if (cuttingRecipeSO.input == inputKitchenObjectSO)
                {
                    return cuttingRecipeSO;
                }
            }

            return null;
        }

        #endregion
    }
}