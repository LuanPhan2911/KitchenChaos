using System;
using System.Collections;
using System.Collections.Generic;
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
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0;
            if (platesSpawnedAmount < platedSpawnedAmountMax)
            {
                platesSpawnedAmount++;
                OnPlateSpawn?.Invoke(this, EventArgs.Empty);

            }
        }
    }
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (platesSpawnedAmount > 0)
            {
                KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
                OnPlateGrabbed?.Invoke(this, EventArgs.Empty);
                platesSpawnedAmount--;
            }


        }

    }

}
