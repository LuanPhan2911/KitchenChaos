using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image clock;
    [SerializeField] private TextMeshProUGUI timerText, addTimerText;

    private float addTimerMax = 1f;
    private float addTimer;


    private void Start()
    {
        Hide();
        HideAddTimerText();
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
        DeliveryManager.Instance.OnRecipeSuccess += Delivery_OnRecipeSuccess;
    }


    private void Delivery_OnRecipeSuccess(object sender, System.EventArgs e)
    {
        addTimer = addTimerMax;
        ShowAddTimerText();
    }

    private void Update()
    {
        if (addTimer > 0)
        {
            addTimer -= Time.deltaTime;
        }
        else
        {
            HideAddTimerText();
        }
        clock.fillAmount = GameManager.Instance.GetGamePlayingNormalized();
        UpdateTimerText();
    }
    private void UpdateTimerText()
    {
        int minute = (int)GameManager.Instance.GetGamePlayingTimer() / 60;
        int second = (int)GameManager.Instance.GetGamePlayingTimer() - minute * 60;
        if (second < 10)
        {
            timerText.text = $"{minute}:0{second}";
        }
        else
        {
            timerText.text = $"{minute}:{second}";
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

    private void ShowAddTimerText()
    {
        addTimerText.gameObject.SetActive(true);
    }
    private void HideAddTimerText()
    {
        addTimerText.gameObject.SetActive(false);
    }

}
