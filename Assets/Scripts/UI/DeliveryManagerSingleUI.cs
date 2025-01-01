using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private Transform iconContainer;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private Transform iconImageTemplate;



    private void Awake()
    {
        iconImageTemplate.gameObject.SetActive(false);
    }
    public void SetRecipeSO(RecipeSO recipeSO)
    {
        textMeshProUGUI.text = recipeSO.recipeName;

        foreach (Transform child in iconContainer)
        {
            if (child == iconImageTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList)
        {
            Transform iconImageTransform = Instantiate(iconImageTemplate, iconContainer);
            iconImageTransform.gameObject.SetActive(true);
            iconImageTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;

        }
    }
}
