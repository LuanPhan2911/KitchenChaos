using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateCounter : BaseCounter
{


    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;

    private int platesSpawnedAmount;
    private int platedSpawnedAmountMax = 4;

    public event EventHandler OnPlateSpawn;
    public event EventHandler OnPlateGrabbed;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;
            if (GameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platedSpawnedAmountMax)
            {
                SpawnPlateServerRPC();

            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlateServerRPC()
    {
        SpawnPlateClientRPC();
    }
    [ClientRpc]
    private void SpawnPlateClientRPC()
    {
        platesSpawnedAmount++;
        OnPlateSpawn?.Invoke(this, EventArgs.Empty);
    }
    public override void Interact(Player player)
    {

        if (!player.HasKitchenObject())
        {

            if (platesSpawnedAmount > 0)
            {

                KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
                InteractServerRPC();
            }


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
        OnPlateGrabbed?.Invoke(this, EventArgs.Empty);
        platesSpawnedAmount--;
    }

}
