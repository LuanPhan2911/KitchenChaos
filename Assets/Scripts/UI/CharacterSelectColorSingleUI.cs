using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectColorSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;

    [SerializeField] private GameObject selectedColorGameObject;
    [SerializeField] private Image image;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.Instance.ChangePlayerColor(colorId);
        });
    }
    private void Start()
    {
        image.color = KitchenGameMultiplayer.Instance.GetPlayerColor(colorId);
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChange += KitchenGameMultiplayer_OnPlayerDataNetworkListChange;
        UpdateIsSelectedColor();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChange(object sender, EventArgs eventArgs)
    {
        UpdateIsSelectedColor();
    }

    private void UpdateIsSelectedColor()
    {
        if (KitchenGameMultiplayer.Instance.GetPlayerData().colorId == colorId)
        {
            selectedColorGameObject.SetActive(true);
        }
        else
        {
            selectedColorGameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChange -= KitchenGameMultiplayer_OnPlayerDataNetworkListChange;
    }
}
