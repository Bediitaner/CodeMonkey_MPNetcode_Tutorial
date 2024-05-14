using System;
using Unity.Netcode;
using UnityEngine;

namespace Counters
{
    public class StoveCounter : BaseCounter, IHasProgress
    {
        #region Events

        public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
        public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

        public class OnStateChangedEventArgs : EventArgs
        {
            public State state;
        }

        #endregion

        #region Contents

        [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
        [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

        #endregion

        #region Fields

        private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
        private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
        private FryingRecipeSO fryingRecipeSO;
        private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
        private BurningRecipeSO burningRecipeSO;

        #endregion


        #region Unity: Start | Update

        private void Start()
        {
            state.Value = State.Idle;
        }

        private void Update()
        {
            if (!IsServer)
            {
                return;
            }

            if (HasKitchenObject())
            {
                switch (state.Value)
                {
                    case State.Idle:
                        break;
                    case State.Frying:
                        fryingTimer.Value += Time.deltaTime;

                        if (fryingTimer.Value > fryingRecipeSO.fryingTimerMax)
                        {
                            // Fried
                            KitchenObject.DestroyKitchenObject(GetKitchenObject());

                            KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                            state.Value = State.Fried;
                            burningTimer.Value = 0f;
                            SetBurningRecipeSOClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO()));
                        }

                        break;
                    case State.Fried:
                        burningTimer.Value += Time.deltaTime;

                        if (burningTimer.Value > burningRecipeSO.burningTimerMax)
                        {
                            // Fried
                            KitchenObject.DestroyKitchenObject(GetKitchenObject());

                            KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                            state.Value = State.Burned;
                        }

                        break;
                    case State.Burned:
                        break;
                }
            }
        }

        #endregion

        #region Netcode: OnNetworkSpawn

        public override void OnNetworkSpawn()
        {
            AddEvents();
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
                        // Player carrying something that can be Fried
                        KitchenObject kitchenObject = player.GetKitchenObject();
                        kitchenObject.SetKitchenObjectParent(this);

                        InteractLogicPlaceObjectOnCounterServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));
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

                            state.Value = State.Idle;
                        }
                    }
                }
                else
                {
                    // Player is not carrying anything
                    GetKitchenObject().SetKitchenObjectParent(player);

                    SetStateIdleServerRpc();
                }
            }
        }

        #endregion

        #region ServerRpc: InteractLogic: PlaceObjectOnCounter

        [ServerRpc(RequireOwnership = false)]
        private void SetStateIdleServerRpc()
        {
            state.Value = State.Idle;
        }
        #endregion

        #region ServerRpc: InteractLogic: PlaceObjectOnCounter

        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
        {
            fryingTimer.Value = 0f;

            state.Value = State.Frying;

            SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
        }

        #endregion

        #region ClientRpc: Set: FryingRecipeSO

        [ClientRpc]
        private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
        {
            KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

            fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
        }

        #endregion

        #region ClientRpc: Set: BurningRecipeSO

        [ClientRpc]
        private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
        {
            KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

            burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);
        }

        #endregion


        #region Has: RecipeWithInput

        private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
        {
            FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
            return fryingRecipeSO != null;
        }

        #endregion


        #region Get: OutputForInput

        private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
        {
            FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
            if (fryingRecipeSO != null)
            {
                return fryingRecipeSO.output;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Get: Frying: RecipeSOWithInput

        private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
        {
            foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
            {
                if (fryingRecipeSO.input == inputKitchenObjectSO)
                {
                    return fryingRecipeSO;
                }
            }

            return null;
        }

        #endregion

        #region Get: Burning: RecipeSOWithInput

        private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
        {
            foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
            {
                if (burningRecipeSO.input == inputKitchenObjectSO)
                {
                    return burningRecipeSO;
                }
            }

            return null;
        }

        #endregion


        #region Is: Fried

        public bool IsFried()
        {
            return state.Value == State.Fried;
        }

        #endregion


        #region Event: OnFryingValueChanged

        private void OnFryingValueChanged(float previousvalue, float newvalue)
        {
            float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;
            
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = fryingTimer.Value / fryingTimerMax
            });
        }

        #endregion

        #region Event: OnBurningValueChanged

        private void OnBurningValueChanged(float previousvalue, float newvalue)
        {
            float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.burningTimerMax : 1f;

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = fryingTimer.Value / burningTimerMax
            });
        }

        #endregion

        #region Event: OnStateValueChanged

        private void OnStateValueChanged(State previousvalue, State newvalue)
        {
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
            {
                state = state.Value
            });

            if (state.Value == State.Burned || state.Value == State.Idle)
            {
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            fryingTimer.OnValueChanged += OnFryingValueChanged;
            burningTimer.OnValueChanged += OnBurningValueChanged;
            state.OnValueChanged += OnStateValueChanged;
        }


        private void RemoveEvents()
        {
            fryingTimer.OnValueChanged -= OnFryingValueChanged;
            burningTimer.OnValueChanged -= OnBurningValueChanged;
            state.OnValueChanged -= OnStateValueChanged;
        }

        #endregion
    }
}