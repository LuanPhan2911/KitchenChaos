using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player Instance { get; private set; }
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayer;

    private Vector3 lastInteractionDir;
    private ClearCounter selectedCounter;


    public class SelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter selectedCounter;
    }
    public event EventHandler<SelectedCounterChangedEventArgs> OnSelectedCounterChanged;




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
    }

    private void GameInput_OnInteraction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact();
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();


    }


    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.getMovementVectorNormalized();
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
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, xMoveDir, moveDistance);
            if (canMove)
            {
                //can move in x direction
                moveDir = xMoveDir;
            }
            else
            {
                //get only z movement
                Vector3 zMoveDir = new Vector3(0f, 0f, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, zMoveDir, moveDistance);
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
        Vector2 inputVector = gameInput.getMovementVectorNormalized();
        Vector3 moveDir = new(inputVector.x, 0f, inputVector.y);
        if (moveDir != Vector3.zero)
        {
            lastInteractionDir = moveDir;
        }
        float interactionDistance = 2f;


        if (Physics.Raycast(transform.position, lastInteractionDir, out RaycastHit hit, interactionDistance, counterLayer))
        {
            if (hit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                if (clearCounter != selectedCounter)
                {
                    SetSelectedCounter(clearCounter);
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

    private void SetSelectedCounter(ClearCounter clearCounter)
    {
        selectedCounter = clearCounter;
        OnSelectedCounterChanged?.Invoke(this,
                    new SelectedCounterChangedEventArgs { selectedCounter = clearCounter });

    }
}
