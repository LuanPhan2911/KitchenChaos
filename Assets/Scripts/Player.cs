using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IKitchenObjectParent
{

    public static Player Instance { get; private set; }
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayer;

    private Vector3 lastInteractionDir;
    private BaseCounter selectedCounter;
    [SerializeField] private Transform kitchenObjectHoldPosition;
    private KitchenObject kitchenObject;


    public class SelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }
    public event EventHandler<SelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public event EventHandler OnPickupSomething;



    private bool isWalking;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one player instance");
        }
        Instance = this;
    }
    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteraction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractionAlternate;
    }

    private void GameInput_OnInteraction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }
    private void GameInput_OnInteractionAlternate(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();


    }


    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new(inputVector.x, 0f, inputVector.y);

        float playerRadius = 0.7f;
        float playerHeight = 2f;
        float moveDistance = moveSpeed * Time.deltaTime;

        bool canMove =
        !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {

            //get only x movement
            Vector3 xMoveDir = new Vector3(moveDir.x, 0f, 0f).normalized;
            canMove = xMoveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, xMoveDir, moveDistance);
            if (canMove)
            {
                //can move in x direction
                moveDir = xMoveDir;
            }
            else
            {
                //get only z movement
                Vector3 zMoveDir = new Vector3(0f, 0f, moveDir.z).normalized;
                canMove = zMoveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, zMoveDir, moveDistance);
                if (canMove)
                {
                    //can move in z direction
                    moveDir = zMoveDir;
                }
                else
                {
                    //can't move in any direction
                    moveDir = Vector3.zero;
                }
            }


        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }
        isWalking = moveDir != Vector3.zero;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotationSpeed * Time.deltaTime);
    }

    private void HandleInteraction()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
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
}
