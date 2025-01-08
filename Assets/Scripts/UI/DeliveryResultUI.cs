using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Image iconImage;

    [SerializeField] private Color successColor;
    [SerializeField] private Color failedColor;

    [SerializeField] private Sprite successIcon;
    [SerializeField] private Sprite failedIcon;

    private Animator animator;
    private const string POPUP = "Popup";
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {


        DeliveryManager.Instance.OnRecipeSuccess += (sender, args) =>
        {
            gameObject.SetActive(true);
            animator.SetTrigger(POPUP);
            background.color = successColor;
            resultText.text = "DELIVERY\nSUCCESS";
            iconImage.sprite = successIcon;


        };
        DeliveryManager.Instance.OnRecipeFailed += (sender, args) =>
        {
            gameObject.SetActive(true);
            animator.SetTrigger(POPUP);
            background.color = failedColor;
            resultText.text = "DELIVERY\nFAILED";
            iconImage.sprite = failedIcon;

        };
        gameObject.SetActive(false);
    }
}
