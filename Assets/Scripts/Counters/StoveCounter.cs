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
        state = State.Idle;
        OnChangeState?.Invoke(this, new ChangeStateEventArgs
        {
            state = state
        });
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

                        state = State.Fried;
                        burnedRecipeSO = GetBurnedRecipeSO(GetKitchenObject().GetKitchenObjectSO());
                        burningTimer = 0f;
                        OnChangeState?.Invoke(this, new ChangeStateEventArgs
                        {
                            state = state
                        });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = burningTimer / burnedRecipeSO.burnedTimerMax,
                        });


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
                        state = State.Burned;
                        OnChangeState?.Invoke(this, new ChangeStateEventArgs
                        {
                            state = state
                        });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0,
                        });


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
                state = State.Idle;
                OnChangeState?.Invoke(this, new ChangeStateEventArgs
                {
                    state = state
                });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0,
                });

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
                    state = State.Frying;
                    OnChangeState?.Invoke(this, new ChangeStateEventArgs
                    {
                        state = state
                    });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax,
                    });
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
}
