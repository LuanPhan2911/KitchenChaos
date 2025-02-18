using UnityEngine;

public class GameMultiplayerPauseUI : MonoBehaviour
{


    private void Start()
    {

        GameManager.Instance.OnGamePaused += (sender, args) =>
        {
            if (KitchenGameMultiplayer.isMultiplayer)
            {
                Show();
            }
        };
        GameManager.Instance.OnGameUnpaused += (sender, args) =>
        {
            Hide();
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
