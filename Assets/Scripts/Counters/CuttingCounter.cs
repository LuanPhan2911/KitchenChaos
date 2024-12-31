using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public event EventHandler OnCut;

    public override void Interact(Player player)
    {
        if (HasKitchenObject())
        {
            //counter has kitchen object
            if (!player.HasKitchenObject() && !HasCuttingProgress())
            {
                //player not carrying kitchen object
                //player grab kitchen object from counter
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
        else
        {
            //counter has no kitchen object
            if (player.HasKitchenObject())
            {
                //player carrying kitchen object
                //player put kitchen object on counter
                if (HasOutputKitchenObjectSO(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    cuttingProgress = 0;
                    //counter have kitchen object and kitchen object can cut
                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = (float)cuttingProgress / cuttingRecipeSO.maxCuttingProgress,
                    });
                    // kitchen object can cut

                }

            }
        }

    }
    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasOutputKitchenObjectSO(GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;
            //counter have kitchen object and kitchen object can cut

            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnCut?.Invoke(this, EventArgs.Empty);
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.maxCuttingProgress,
            });


            if (cuttingProgress >= cuttingRecipeSO.maxCuttingProgress)
            {

                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(cuttingRecipeSO.output, this);
                cuttingProgress = 0;
            }
        }

    }


    private KitchenObjectSO GetOutputKitchenObjectSO(KitchenObjectSO inputKitchenObjectSO)
    {

        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO?.output;
    }

    private bool HasCuttingProgress()
    {
        return cuttingProgress != 0;
    }
    private bool HasOutputKitchenObjectSO(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }
    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var cutKitchenObjectSO in cuttingRecipeSOArray)
        {
            if (cutKitchenObjectSO.input == inputKitchenObjectSO)
            {
                return cutKitchenObjectSO;
            }
        }
        return null;
    }
}
