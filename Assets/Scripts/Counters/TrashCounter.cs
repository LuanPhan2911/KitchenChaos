using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnDrop;

    new public static void ResetStaticState()
    {
        OnDrop = null;
    }
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            player.GetKitchenObject().DestroySelf();

            OnDrop?.Invoke(this, EventArgs.Empty);
        }
    }
}
