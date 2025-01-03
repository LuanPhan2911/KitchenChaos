using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{

    [SerializeField] private Transform counterTopPosition;

    public static event EventHandler OnAnyObjectPlace;
    private KitchenObject kitchenObject;

    public virtual void Interact(Player player)
    {
        // Debug.LogError("Interacting with BaseCounter");
    }
    public virtual void InteractAlternate(Player player)
    {
        // Debug.LogError("Interacting Alternate with BaseCounter");
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPosition;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnAnyObjectPlace?.Invoke(this, EventArgs.Empty);
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
