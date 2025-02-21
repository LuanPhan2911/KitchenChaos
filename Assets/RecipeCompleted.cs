using TMPro;
using UnityEngine;

public class RecipeCompleted : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeCompletedText;

    private void Start()
    {
        recipeCompletedText.text = $"Best recipe completed: {GameData.Instance.GetMaxRecipeCompleted()}";
    }

}
