using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public event EventHandler OnCut;
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticState()
    {
        OnAnyCut = null;
    }

    public override void Interact(Player player)
    {
        if (HasKitchenObject())
        {
            //counter has kitchen object
            if (!player.HasKitchenObject())
            {
                //player not carrying kitchen object
                //player grab kitchen object from counter
                GetKitchenObject().SetKitchenObjectParent(player);
            }
            else
            {
                //player is carrying kitchen object
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        // GetKitchenObject().DestroySelf();
                    }
                }
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
                    InteractPlaceObjectOnCounterServerRPC();

                    // kitchen object can cut

                }

            }
        }

    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasOutputKitchenObjectSO(GetKitchenObject().GetKitchenObjectSO()))
        {
            CutObjectServerRPC();
            TestCuttingProgressDoneServerRPC();
        }

    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractPlaceObjectOnCounterServerRPC()
    {
        InteractPlaceObjectOnCounterClientRPC();
    }
    [ClientRpc]
    private void InteractPlaceObjectOnCounterClientRPC()
    {
        cuttingProgress = 0;
        //counter have kitchen object and kitchen object can cut


        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f,
        });
    }
    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRPC()
    {
        CutObjectClientRPC();
    }
    [ClientRpc]
    private void CutObjectClientRPC()
    {
        cuttingProgress++;
        //counter have kitchen object and kitchen object can cut

        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.maxCuttingProgress,
        });

    }
    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRPC()
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
        if (cuttingProgress >= cuttingRecipeSO.maxCuttingProgress)
        {

            KitchenObject.DestroyKitchenObject(GetKitchenObject());
            // GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(cuttingRecipeSO.output, this);
            cuttingProgress = 0;
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
