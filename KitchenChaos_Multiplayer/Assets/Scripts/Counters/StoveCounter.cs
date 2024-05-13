using System;
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

        private State state;
        private float fryingTimer;
        private FryingRecipeSO fryingRecipeSO;
        private float burningTimer;
        private BurningRecipeSO burningRecipeSO;

        #endregion
        
        
        #region Unity: Start | Update

        private void Start()
        {
            state = State.Idle;
        }

        private void Update()
        {
            if (HasKitchenObject())
            {
                switch (state)
                {
                    case State.Idle:
                        break;
                    case State.Frying:
                        fryingTimer += Time.deltaTime;

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                        });

                        if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                        {
                            // Fried
                            GetKitchenObject().DestroySelf();

                            KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                            state = State.Fried;
                            burningTimer = 0f;
                            burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                            {
                                state = state
                            });
                        }

                        break;
                    case State.Fried:
                        burningTimer += Time.deltaTime;

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
                        });

                        if (burningTimer > burningRecipeSO.burningTimerMax)
                        {
                            // Fried
                            GetKitchenObject().DestroySelf();

                            KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                            state = State.Burned;

                            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                            {
                                state = state
                            });

                            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                            {
                                progressNormalized = 0f
                            });
                        }

                        break;
                    case State.Burned:
                        break;
                }
            }
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
                        player.GetKitchenObject().SetKitchenObjectParent(this);

                        fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        state = State.Frying;
                        fryingTimer = 0f;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                        });
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

                            state = State.Idle;

                            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                            {
                                state = state
                            });

                            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                            {
                                progressNormalized = 0f
                            });
                        }
                    }
                }
                else
                {
                    // Player is not carrying anything
                    GetKitchenObject().SetKitchenObjectParent(player);

                    state = State.Idle;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
                }
            }
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
            return state == State.Fried;
        }

        #endregion
    }
}