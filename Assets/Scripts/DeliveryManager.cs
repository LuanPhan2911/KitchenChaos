using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    [SerializeField] private RecipeListSO recipeListSO;
    [SerializeField] private float addedDeliveryCorrectTimer = 15f;

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;

    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public class OnRecipeCommpletedEventArgs : EventArgs
    {
        public RecipeSO recipeSO;
    }
    public event EventHandler<OnRecipeCommpletedEventArgs> OnRecipeCommpletedWithRecipeSO;

    private List<RecipeSO> waitingRecipeSOList;
    public static DeliveryManager Instance { get; private set; }

    //private float spawnRecipeTimer;
    //private float spawnRecipeTimerMax;

    //private int waitingRecipeMax = 4;

    private int deliveredSuccess;

    private void Awake()
    {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
        //spawnRecipeTimer = UnityEngine.Random.Range(5, 15);
        //spawnRecipeTimerMax = UnityEngine.Random.Range(15, 30);
    }
    //private void Update()
    //{
    //if (!IsServer)
    //{
    //    return;
    //}

    //spawnRecipeTimer -= Time.deltaTime;
    //if (spawnRecipeTimer <= 0f)
    //{
    //    spawnRecipeTimer = spawnRecipeTimerMax;
    //    if (GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax)
    //    {
    //        spawnRecipeTimerMax = UnityEngine.Random.Range(15, 30);
    //        int waitingRecipeIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
    //        SpawnNewWaitingRecipeClientRPC(waitingRecipeIndex);
    //    }
    //}

    //}
    [ServerRpc(RequireOwnership = false)]
    public void SpawnNewWaitingRecipeServerRPC(int recipeSOIndex)
    {
        SpawnNewWaitingRecipeClientRPC(recipeSOIndex);
    }
    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRPC(int waitingRecipeSOIndex)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];
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
        GameManager.Instance.AddGamePlayingTimer(addedDeliveryCorrectTimer);
        DeliverCorrectRecipeClientRPC(recipeIndex);
    }
    [ClientRpc]
    private void DeliverCorrectRecipeClientRPC(int recipeIndex)
    {


        OnRecipeCommpletedWithRecipeSO?.Invoke(this, new OnRecipeCommpletedEventArgs
        {
            recipeSO = waitingRecipeSOList[recipeIndex]
        });
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
    public RecipeSO GetRandomRecipeSO(out int recipeSOIndex)
    {
        recipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
        return recipeListSO.recipeSOList[recipeSOIndex];
    }

}
