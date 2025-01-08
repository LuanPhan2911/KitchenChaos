using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveWarningUI : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    private void Start()
    {

        stoveCounter.OnProgressChanged += (sender, args) =>
        {
            float burnShowWarningAmount = 0.5f;
            if (stoveCounter.IsFried() && args.progressNormalized >= burnShowWarningAmount)
            {
                Show();
            }
            else
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
