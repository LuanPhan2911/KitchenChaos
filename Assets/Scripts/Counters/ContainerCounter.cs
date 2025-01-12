using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public event EventHandler OnPlayerGrabbedKitchenObject;


    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
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
        OnPlayerGrabbedKitchenObject?.Invoke(this, EventArgs.Empty);
    }



}
