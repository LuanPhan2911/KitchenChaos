using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image imageBar;
    [SerializeField] private CuttingCounter cuttingCounter;


    private void Start()
    {

        cuttingCounter.OnProgressChanged += (sender, args) =>
        {
            imageBar.fillAmount = args.progressNormalized;
            if (args.progressNormalized == 0f || args.progressNormalized == 1f)
            {
                Hide();
            }
            else
            {
                Show();
            }
        };
        imageBar.fillAmount = 0;
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
