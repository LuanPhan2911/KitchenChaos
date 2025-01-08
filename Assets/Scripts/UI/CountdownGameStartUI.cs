using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownGameStartUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private const string NUMBER_POPUP = "NumberPopup";
    private Animator animator;
    private int previousCountdown;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
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
        int countdown = Mathf.CeilToInt(GameManager.Instance.GetCountdownTimer());
        countdownText.text = countdown.ToString();
        if (countdown != previousCountdown)
        {
            previousCountdown = countdown;
            animator.SetTrigger(NUMBER_POPUP);
            SoundManager.Instance.PlayCountdownSound();
        }
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
