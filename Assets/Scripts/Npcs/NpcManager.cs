using System;
using UnityEngine;

public class NpcManager : MonoBehaviour
{
    [HideInInspector] public NpcMovement npcMovement;
    [HideInInspector] public NpcAnimation npcAnimation;
    [SerializeField] public NpcOrderRecipe npcOrderRecipe;
    public RecipeSO recipeSO;
    public int recipeSOIndex;

    public const float minDistance = 0.5f;

    public class OnSpawnRecipeEventArgs : EventArgs
    {
        public int recipeSOIndex;
    }
    public event EventHandler<OnSpawnRecipeEventArgs> OnSpawnRecipe;

    private void Awake()
    {
        npcMovement = GetComponent<NpcMovement>();
        npcAnimation = GetComponent<NpcAnimation>();
    }

    private void Start()
    {
        npcMovement.OnReachTargetPosition += NpcMovement_OnReachTargetPosition; ;
    }

    private void NpcMovement_OnReachTargetPosition(object sender, System.EventArgs e)
    {
        if (Vector3.Distance(transform.position, SpawnNpcManager.Instance.GetWaitingPosition()) <= minDistance)
        {
            // handle order here
            OnSpawnRecipe?.Invoke(this, new OnSpawnRecipeEventArgs
            {
                recipeSOIndex = recipeSOIndex
            });
            // show order message
            npcOrderRecipe.SetOrderText(recipeSO.recipeName);



            // receive order and move to spawn position
            //npcMovement.SetTargetPosition(SpawnNpcManager.Instance.GetSpawnPosition());
        }
        else if (Vector3.Distance(transform.position, SpawnNpcManager.Instance.GetSpawnPosition()) <= minDistance)
        {
            // destroy npc
            SpawnNpcManager.Instance.RemoveNpcManager(this);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        npcMovement.OnReachTargetPosition -= NpcMovement_OnReachTargetPosition;
    }
}
