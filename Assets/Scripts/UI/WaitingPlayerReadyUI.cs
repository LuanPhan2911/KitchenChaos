using UnityEngine;

public class WaitingPlayerReadyUI : MonoBehaviour
{

    private void Start()
    {

        GameManager.Instance.OnLocalPlayerReadyChanged += (sender, args) =>
        {
            if (GameManager.Instance.IsLocalPlayerReady())
            {

                Show();
            }
        };
        GameManager.Instance.OnStateChanged += (sender, args) =>
        {
            if (GameManager.Instance.IsCountdownGameStart())
            {
                Hide();
            }
        };

        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
