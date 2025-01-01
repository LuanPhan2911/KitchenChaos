using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateGrabbedByPlayer))
                {
                    if (plateGrabbedByPlayer.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
                else
                {
                    //player not carrying plate but something else
                    if (GetKitchenObject().TryGetPlate(out PlateKitchenObject platePutOnCounter))
                    {
                        // kitchen object on counter is plate
                        // put kitchen object player carrying to plate on counter
                        if (platePutOnCounter.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
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
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }

    }
}
