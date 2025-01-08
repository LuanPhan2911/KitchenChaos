using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurnedRecipeSO[] burnedRecipeSOArray;



    public event EventHandler<ChangeStateEventArgs> OnChangeState;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public class ChangeStateEventArgs : EventArgs
    {
        public State state;
    }

    private float fryingTimer;
    private float burningTimer;
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }

    private State state;


    private void Start()
    {
        SetState(State.Idle);
    }
    private FryingRecipeSO fryingRecipeSO;
    private BurnedRecipeSO burnedRecipeSO;

    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {

                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax,
                    });

                    if (fryingTimer >= fryingRecipeSO.fryingTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);


                        burnedRecipeSO = GetBurnedRecipeSO(GetKitchenObject().GetKitchenObjectSO());
                        burningTimer = 0f;

                        float progressNormalized = burningTimer / burnedRecipeSO.burnedTimerMax;
                        SetState(State.Fried, progressNormalized);


                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer / burnedRecipeSO.burnedTimerMax,
                    });

                    if (burningTimer >= burnedRecipeSO.burnedTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burnedRecipeSO.output, this);
                        SetState(State.Burned);


                    }


                    break;
                case State.Burned:
                    break;
            }
        }
    }
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

                SetState(State.Idle);


            }
            else
            {
                //player is carrying kitchen object
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                        SetState(State.Idle);
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
                if (HasOutputKitchenObjectSO(player.GetKitchenObject().GetKitchenObjectSO()))
                {

                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    fryingTimer = 0;

                    float progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax;
                    SetState(State.Frying, progressNormalized);


                }

            }
        }
    }

    private KitchenObjectSO GetOutputKitchenObjectSO(KitchenObjectSO inputKitchenObjectSO)
    {

        FryingRecipeSO kitchenObjectSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return kitchenObjectSO?.output;
    }

    private bool HasOutputKitchenObjectSO(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO kitchenObjectSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return kitchenObjectSO != null;
    }
    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var kitchenObjectSO in fryingRecipeSOArray)
        {
            if (kitchenObjectSO.input == inputKitchenObjectSO)
            {
                return kitchenObjectSO;
            }
        }
        return null;
    }
    private BurnedRecipeSO GetBurnedRecipeSO(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var kitchenObjectSO in burnedRecipeSOArray)
        {
            if (kitchenObjectSO.input == inputKitchenObjectSO)
            {
                return kitchenObjectSO;
            }
        }
        return null;
    }

    private void SetState(State state, float progressNormalized = 0)
    {
        this.state = state;
        OnChangeState?.Invoke(this, new ChangeStateEventArgs
        {
            state = state
        });
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = progressNormalized,
        });
    }
    public bool IsFried()
    {
        return state == State.Fried;
    }
}
