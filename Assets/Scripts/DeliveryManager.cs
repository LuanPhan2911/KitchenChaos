using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    [SerializeField] private RecipeListSO recipeListSO;

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;

    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    private List<RecipeSO> waitingRecipeSOList;
    public static DeliveryManager Instance { get; private set; }

    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4;
    private int waitingRecipeMax = 4;

    private int deliveredSuccess;

    private void Awake()
    {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }
    private void Update()
    {

        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;
            if (GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax)
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                waitingRecipeSOList.Add(waitingRecipeSO);
                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }

    }
    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        bool canDeliverPlate = false;
        RecipeSO deleteWaitingRecipe = null;
        foreach (var waitingRecipeSO in waitingRecipeSOList)
        {
            List<KitchenObjectSO> differenceRecipeList = waitingRecipeSO.kitchenObjectSOList.Except(plateKitchenObject.GetKitchenObjectSOList()).ToList();
            if (differenceRecipeList.Count == 0)
            {
                canDeliverPlate = true;
                deleteWaitingRecipe = waitingRecipeSO;
                break;

            }
        }
        if (canDeliverPlate && deleteWaitingRecipe != null)
        {
            waitingRecipeSOList.Remove(deleteWaitingRecipe);
            OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
            OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
            deliveredSuccess++;
        }
        else
        {
            OnRecipeFailed?.Invoke(this, EventArgs.Empty);
        }
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
