using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Player : NetworkBehaviour, IKitchenObjectParent
{

    // public static Player Instance { get; private set; }

    public static Player LocalInstance { get; private set; }


    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayer;
    [SerializeField] private LayerMask collisionsLayer;

    private Vector3 lastInteractionDir;
    private BaseCounter selectedCounter;
    [SerializeField] private Transform kitchenObjectHoldPosition;
    [SerializeField] private Vector3[] spawnPositions;
    [SerializeField] private PlayerVisual playerVisual;
    private KitchenObject kitchenObject;

    public static event EventHandler OnAnyPlayerSpawn;


    public class SelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }
    public event EventHandler<SelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public event EventHandler OnPickupSomething;
    public static event EventHandler OnAnyPlayerPickupSomething;


    public static void ResetStaticState()
    {
        OnAnyPlayerSpawn = null;
    }

    private bool isWalking;

    private void Awake()
    {
        // if (Instance != null)
        // {
        //     Debug.LogError("There is more than one player instance");
        // }
        // Instance = this;
    }
    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteraction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractionAlternate;

        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
        transform.position = spawnPositions[
            KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)
        ];
        OnAnyPlayerSpawn?.Invoke(this, EventArgs.Empty);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
           {
               // destroy kitchen object when player holder object and disconnect
               if (clientId == OwnerClientId && HasKitchenObject())
               {
                   KitchenObject.DestroyKitchenObject(GetKitchenObject());
               }
           };
        }
    }

    private void GameInput_OnInteraction(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying())
        {
            return;
        }

        if (selectedCounter != null)
        {

            selectedCounter.Interact(this);
        }
    }
    private void GameInput_OnInteractionAlternate(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying())
        {
            return;
        }
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        HandleMovement();

        HandleInteraction();


    }



    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new(inputVector.x, 0f, inputVector.y);
        float moveSpeed = 5f;

        float playerRadius = 0.7f;
        float moveDistance = moveSpeed * Time.deltaTime;

        bool canMove =
        !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir,
         Quaternion.identity, moveDistance, collisionsLayer);

        if (!canMove)
        {

            //get only x movement
            Vector3 xMoveDir = new Vector3(moveDir.x, 0f, 0f).normalized;
            canMove = xMoveDir.x != 0 &&
            !Physics.BoxCast(transform.position, Vector3.one * playerRadius, xMoveDir,
             Quaternion.identity, moveDistance, collisionsLayer);
            if (canMove)
            {
                //can move in x direction
                moveDir = xMoveDir;
            }
            else
            {
                //get only z movement
                Vector3 zMoveDir = new Vector3(0f, 0f, moveDir.z).normalized;
                canMove = zMoveDir.z != 0 && !Physics.BoxCast(
                   transform.position, Vector3.one * playerRadius, zMoveDir,
                   Quaternion.identity, moveDistance, collisionsLayer);
                if (canMove)
                {
                    //can move in z direction
                    moveDir = zMoveDir;
                }
                else
                {
                    //can't move in any direction

                }
            }


        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }
        float rotationSpeed = 10f;
        isWalking = moveDir != Vector3.zero;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotationSpeed * Time.deltaTime);
    }

    private void HandleInteraction()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new(inputVector.x, 0f, inputVector.y);
        if (moveDir != Vector3.zero)
        {
            lastInteractionDir = moveDir;
        }
        float interactionDistance = 2f;


        if (Physics.Raycast(transform.position, lastInteractionDir, out RaycastHit hit, interactionDistance, counterLayer))
        {
            if (hit.transform.TryGetComponent(out BaseCounter counter))
            {
                if (counter != selectedCounter)
                {
                    SetSelectedCounter(counter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }

    }
    public bool IsWalking()
    {
        return isWalking;
    }

    private void SetSelectedCounter(BaseCounter counter)
    {
        selectedCounter = counter;
        OnSelectedCounterChanged?.Invoke(this,
                    new SelectedCounterChangedEventArgs { selectedCounter = counter });

    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPosition;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnPickupSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPlayerPickupSomething?.Invoke(this, EventArgs.Empty);
        }
    }
    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }


    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }
    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
