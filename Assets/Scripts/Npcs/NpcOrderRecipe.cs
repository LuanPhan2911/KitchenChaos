using TMPro;
using UnityEngine;

public class NpcOrderRecipe : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI orderText;

    private void Start()
    {
        Hide();
    }
    public void SetOrderText(string text)
    {
        Show();
        orderText.text = text;
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
