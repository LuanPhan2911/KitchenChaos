using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestNetworkUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Start()
    {
        hostButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.Instance.StartHost();
            Debug.Log("HOST");
            Hide();
        });
        clientButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.Instance.StartClient();
            Debug.Log("CLIENT");
            Hide();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
