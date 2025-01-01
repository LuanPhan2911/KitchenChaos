using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plateKitchenObject;

    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }
    private void Start()
    {


        plateKitchenObject.OnIngredientAdded += (sender, args) =>
        {
            foreach (Transform child in transform)
            {
                if (child == iconTemplate) continue;
                Destroy(child.gameObject);
            }

            foreach (KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
            {

                Transform iconTemplateTransform = Instantiate(iconTemplate, transform);
                iconTemplateTransform.gameObject.SetActive(true);
                iconTemplateTransform.GetComponent<PlateIconSingleUI>().SetKitchenObjectSO(kitchenObjectSO);

            }
        };
    }


}
