using System;
using System.Collections.Generic;
using UnityEngine;

public class OnIngredientAddedEventArgs : EventArgs
{
    public KitchenObjectSO kitchenObjectSO;
}

public class PlateKitchenObject : KitchenObject
{
    #region Events

    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;

    #endregion

    #region Contents

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

    #endregion

    #region Fields

    private List<KitchenObjectSO> kitchenObjectSOList;

    #endregion

    #region Unity: Awake

    protected override void Awake()
    {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    #endregion

    
    #region Get: KitchenObjectSOList

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }

    #endregion
    
    #region TryAdd: Ingredient

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            // Not a valid ingredient
            return false;
        }

        if (kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            // Already has this type
            return false;
        }
        else
        {
            kitchenObjectSOList.Add(kitchenObjectSO);

            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
            {
                kitchenObjectSO = kitchenObjectSO
            });

            return true;
        }
    }

    #endregion
}