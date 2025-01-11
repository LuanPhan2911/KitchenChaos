using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{

    [SerializeField] private BaseCounter counter;
    [SerializeField] private GameObject[] counterVisuals;
    private void Start()
    {
        // Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.SelectedCounterChangedEventArgs e)
    {
        if (e.selectedCounter == counter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (GameObject counterVisual in counterVisuals)
        {
            counterVisual.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach (GameObject counterVisual in counterVisuals)
        {
            counterVisual.SetActive(false);
        }
    }
}
