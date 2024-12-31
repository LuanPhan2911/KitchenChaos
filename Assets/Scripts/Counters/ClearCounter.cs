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
                if (player.GetKitchenObject() is PlateKitchenObject plateKitchenObject)
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
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
