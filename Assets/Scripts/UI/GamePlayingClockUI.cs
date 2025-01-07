using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image clock;

    private void Start()
    {
        Hide();
        GameManager.Instance.OnStateChanged += (sender, args) =>
        {
            if (GameManager.Instance.IsGamePlaying())
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
        clock.fillAmount = GameManager.Instance.GetGamePlayingNormalized();
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
