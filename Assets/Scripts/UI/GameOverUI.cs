using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deliveredText;
    private void Start()
    {
        Hide();
        GameManager.Instance.OnStateChanged += (sender, args) =>
        {
            if (GameManager.Instance.IsGameOver())
            {
                Show();
                deliveredText.text = DeliveryManager.Instance.GetDeliveredSuccess().ToString();
            }
            else
            {
                Hide();
            }
        };
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
}
