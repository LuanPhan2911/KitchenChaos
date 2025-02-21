using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }


    private const string MAX_RECIPE_COMPLETED = "MaxRecipeCompleted";
    private int maxRecipeCompleted;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        maxRecipeCompleted = PlayerPrefs.GetInt(MAX_RECIPE_COMPLETED, 0);
        DontDestroyOnLoad(gameObject);
    }
    public void SetMaxRecipeCompleted(int recipeCompleted)
    {
        if (recipeCompleted > maxRecipeCompleted)
        {
            maxRecipeCompleted = recipeCompleted;
            PlayerPrefs.SetInt(MAX_RECIPE_COMPLETED, maxRecipeCompleted);
            PlayerPrefs.Save();
        }
    }
    public int GetMaxRecipeCompleted()
    {
        return maxRecipeCompleted;
    }
}
