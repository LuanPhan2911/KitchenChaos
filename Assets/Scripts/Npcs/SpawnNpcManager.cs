using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnNpcManager : NetworkBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform waitingPosition;

    [SerializeField] private GameObject[] npcPrefabs;

    private float spawnNpcTimer;
    private float spawnNpcTimerMax;
    private int waitingNpcMax = 1;

    private List<NpcManager> npcManagers;





    public static SpawnNpcManager Instance { get; private set; }

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
        //spawnNpcTimer = UnityEngine.Random.Range(5, 15);
        spawnNpcTimerMax = UnityEngine.Random.Range(15, 30);
        npcManagers = new List<NpcManager>();

    }
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeCommpletedWithRecipeSO += DeliveryManager_OnRecipeCompleted;
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, DeliveryManager.OnRecipeCommpletedEventArgs e)
    {

        foreach (NpcManager npcManager in npcManagers)
        {

            if (npcManager.recipeSO == e.recipeSO)
            {
                npcManager.recipeSOIndex = -1;
                npcManager.recipeSO = null;
                npcManager.npcOrderRecipe.Hide();
                npcManager.npcMovement.SetTargetPosition(GetSpawnPosition());
                break;
            }
        }
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        spawnNpcTimer -= Time.deltaTime;
        if (spawnNpcTimer <= 0f)
        {
            spawnNpcTimer = spawnNpcTimerMax;
            if (GameManager.Instance.IsGamePlaying() && npcManagers.Count < waitingNpcMax)
            {
                spawnNpcTimerMax = UnityEngine.Random.Range(15, 30);
                int npcIndex = UnityEngine.Random.Range(0, npcPrefabs.Length);
                SpawnNpcClientRpc(npcIndex);
            }
        }

    }

    [ClientRpc]
    public void SpawnNpcClientRpc(int npcIndex)
    {
        GameObject npcPrefab = npcPrefabs[npcIndex];
        GameObject npc = Instantiate(npcPrefab, spawnPosition.position, Quaternion.identity);
        NpcManager npcManager = npc.GetComponent<NpcManager>();

        AddNpcManager(npcManager);

    }
    public void AddNpcManager(NpcManager npcManager)
    {
        npcManager.recipeSO = DeliveryManager.Instance.GetRandomRecipeSO(out int recipeSOIndex);
        npcManager.recipeSOIndex = recipeSOIndex;
        npcManager.npcMovement.SetTargetPosition(waitingPosition.position);

        npcManager.OnSpawnRecipe += NpcManager_OnSpawnRecipe;

        npcManagers.Add(npcManager);

    }

    private void NpcManager_OnSpawnRecipe(object sender, NpcManager.OnSpawnRecipeEventArgs e)
    {
        DeliveryManager.Instance.SpawnNewWaitingRecipeServerRPC(e.recipeSOIndex);
    }

    public void RemoveNpcManager(NpcManager npcManager)
    {
        npcManager.OnSpawnRecipe -= NpcManager_OnSpawnRecipe;
        npcManagers.Remove(npcManager);

    }
    public Vector3 GetSpawnPosition()
    {
        return spawnPosition.position;
    }
    public Vector3 GetWaitingPosition()
    {
        return waitingPosition.position;
    }

}
