using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }

    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);



    private FryingRecipeSO fryingRecipeSO;
    private BurnedRecipeSO burnedRecipeSO;

    public override void OnNetworkSpawn()
    {

        fryingTimer.OnValueChanged += (previousValue, newValue) =>
        {
            float fryingTimerMax = fryingRecipeSO == null ? 1f : fryingRecipeSO.fryingTimerMax;
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = fryingTimer.Value / fryingTimerMax,
            });
        };
        burningTimer.OnValueChanged += (previousValue, newValue) =>
        {
            float burnedTimerMax = burnedRecipeSO == null ? 1f : burnedRecipeSO.burnedTimerMax;
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = burningTimer.Value / burnedTimerMax,
            });
        };
        state.OnValueChanged += (previousValue, newValue) =>
        {
            OnChangeState?.Invoke(this, new ChangeStateEventArgs
            {
                state = state.Value
            });
            if (state.Value == State.Burned || state.Value == State.Idle)
            {
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f,
                });
            }
        };
    }
    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        if (HasKitchenObject())
        {
            switch (state.Value)
            {

                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer.Value += Time.deltaTime;


                    if (fryingTimer.Value >= fryingRecipeSO.fryingTimerMax)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);


                        burningTimer.Value = 0f;
                        SetState(State.Fried);
                        SetBurningRecipeClientRPC(
                            KitchenObjectsMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO())
                        );


                    }
                    break;
                case State.Fried:
                    burningTimer.Value += Time.deltaTime;

                    if (burningTimer.Value >= burnedRecipeSO.burnedTimerMax)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

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

                SetStateIdleServerRPC();


            }
            else
            {
                //player is carrying kitchen object
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        SetStateIdleServerRPC();
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

                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);

                    InteractPlaceObjectOnCounterServerRPC(
                        KitchenObjectsMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));

                }

            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractPlaceObjectOnCounterServerRPC(int kitchenObjectSOIndex)
    {
        //network value only modify on server rpc
        fryingTimer.Value = 0;
        SetState(State.Frying);
        SetFryingRecipeClientRPC(kitchenObjectSOIndex);
    }
    [ClientRpc]
    private void SetFryingRecipeClientRPC(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenObjectsMultiplayer.Instance.GetKitchenObjectSoFromIndex(kitchenObjectSOIndex);
        fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
    }
    [ClientRpc]
    private void SetBurningRecipeClientRPC(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenObjectsMultiplayer.Instance.GetKitchenObjectSoFromIndex(kitchenObjectSOIndex);
        burnedRecipeSO = GetBurnedRecipeSOWithInput(kitchenObjectSO);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRPC()
    {
        state.Value = State.Idle;
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
    private BurnedRecipeSO GetBurnedRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
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

    private void SetState(State state)
    {
        this.state.Value = state;

    }
    public bool IsFried()
    {
        return state.Value == State.Fried;
    }
}
