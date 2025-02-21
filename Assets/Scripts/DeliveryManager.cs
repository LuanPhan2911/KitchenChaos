using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    [SerializeField] private RecipeListSO recipeListSO;

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;

    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    private List<RecipeSO> waitingRecipeSOList;
    public static DeliveryManager Instance { get; private set; }

    private float spawnRecipeTimer = 4f;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipeMax = 4;

    private int deliveredSuccess;

    private void Awake()
    {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }
    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;
            if (GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax)
            {
                int waitingRecipeIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                SpawnNewWaitingRecipeClientRPC(waitingRecipeIndex);
            }
        }

    }
    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRPC(int waitingRecipeIndex)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];
            List<KitchenObjectSO> differenceRecipeList = waitingRecipeSO.kitchenObjectSOList.Except(plateKitchenObject.GetKitchenObjectSOList()).ToList();
            if (differenceRecipeList.Count == 0)
            {

                DeliverCorrectRecipeServerRPC(i);
                return;
            }
        }
        DeliverInCorrectRecipeServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRPC(int recipeIndex)
    {
        GameManager.Instance.AddGamePlayingTimer(5f);
        DeliverCorrectRecipeClientRPC(recipeIndex);
    }
    [ClientRpc]
    private void DeliverCorrectRecipeClientRPC(int recipeIndex)
    {
        waitingRecipeSOList.RemoveAt(recipeIndex);
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
        deliveredSuccess++;
    }
    [ServerRpc(RequireOwnership = false)]
    private void DeliverInCorrectRecipeServerRPC()
    {
        DeliverInCorrectRecipeClientRPC();
    }
    [ClientRpc]
    private void DeliverInCorrectRecipeClientRPC()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetDeliveredSuccess()
    {
        return deliveredSuccess;
    }
}
