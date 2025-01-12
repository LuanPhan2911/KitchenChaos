using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());


            InteractServerRPC();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractServerRPC()
    {
        InteractClientRPC();
    }

    [ClientRpc]
    private void InteractClientRPC()
    {
        OnDrop?.Invoke(this, EventArgs.Empty);
    }
}
