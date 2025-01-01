using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [SerializeField] PlateKitchenObject plateKitchenObject;


    [Serializable]
    public struct KitchenObjectSO_GameObject
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSO_GameObjectList;



    private void Start()
    {
        foreach (var kitchenObjectSO_GameObject in kitchenObjectSO_GameObjectList)
        {
            kitchenObjectSO_GameObject.gameObject.SetActive(false);
        }

        plateKitchenObject.OnIngredientAdded += (sender, args) =>
        {
            foreach (var kitchenObjectSO_GameObject in kitchenObjectSO_GameObjectList)
            {
                if (kitchenObjectSO_GameObject.kitchenObjectSO == args.kitchenObjectSO)
                {
                    kitchenObjectSO_GameObject.gameObject.SetActive(true);
                }
            }
        };
    }
}
