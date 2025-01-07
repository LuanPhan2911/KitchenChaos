using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownGameStartUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Start()
    {
        Hide();
        GameManager.Instance.OnStateChanged += (sender, args) =>
        {
            if (GameManager.Instance.IsCountdownGameStart())
            {
                Show();
            }
            else
            {
                Hide();
            }
        };
    }
    private void Update()
    {
        countdownText.text = Mathf.Ceil(GameManager.Instance.GetCountdownTimer()).ToString();
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
