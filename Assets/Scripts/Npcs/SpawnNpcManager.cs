using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]


public class SpawnNpcManager : NetworkBehaviour
{


    [SerializeField] private SpawnNpcWaitingPositionContainer spawnNpcWaitingPositionContainer;



    [SerializeField] private GameObject[] npcPrefabs;

    private float spawnNpcTimer;
    private float spawnNpcTimerMax;
    private int waitingNpcMax = 4;

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
        spawnNpcTimerMax = UnityEngine.Random.Range(5, 15);
        spawnNpcTimer = UnityEngine.Random.Range(5, 15);
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
                npcManager.npcMovement.SetTargetPosition(
                    npcManager.spawnNpcWaitingPosition.GetSpawnPosition());
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
                //spawnNpcTimerMax = UnityEngine.Random.Range(15, 30);
                spawnNpcTimerMax = UnityEngine.Random.Range(5, 10);

                int npcIndex = UnityEngine.Random.Range(0, npcPrefabs.Length);

                SpawnNpcWaitingPosition waitingPosition = spawnNpcWaitingPositionContainer.GetRandomNpcWaitingPosition(out int waitingPositionIndex);
                if (waitingPositionIndex != -1)
                {
                    waitingPosition.SetNpcUsaged(npcIndex);
                    DeliveryManager.Instance.GetRandomRecipeSO(out int recipeSOIndex);
                    SpawnNpcClientRpc(npcIndex, recipeSOIndex, waitingPositionIndex);
                }

            }
        }

    }

    [ClientRpc]
    public void SpawnNpcClientRpc(int npcIndex, int recipeSOIndex, int waitingPositionIndex)
    {
        GameObject npcPrefab = npcPrefabs[npcIndex];
        SpawnNpcWaitingPosition waitingPosition = spawnNpcWaitingPositionContainer.
            GetSpawnNpcWaitingPosition(waitingPositionIndex);

        GameObject npc = Instantiate(npcPrefab, waitingPosition.GetSpawnPosition(), Quaternion.identity);
        NpcManager npcManager = npc.GetComponent<NpcManager>();

        npcManager.recipeSO = DeliveryManager.Instance.GetRecipeSO(recipeSOIndex);
        npcManager.recipeSOIndex = recipeSOIndex;
        npcManager.spawnNpcWaitingPosition = waitingPosition;
        npcManager.npcMovement.SetTargetPosition(waitingPosition.GetWaitingPosition());
        npcManager.OnSpawnRecipe += NpcManager_OnSpawnRecipe;

        npcManagers.Add(npcManager);

    }


    private void NpcManager_OnSpawnRecipe(object sender, NpcManager.OnSpawnRecipeEventArgs e)
    {
        if (IsHost)
        {
            DeliveryManager.Instance.SpawnNewWaitingRecipeServerRPC(e.recipeSOIndex);
        }
    }

    public void RemoveNpcManager(NpcManager npcManager)
    {
        npcManager.spawnNpcWaitingPosition.SetNpcUsaged(-1);
        npcManager.spawnNpcWaitingPosition = null;
        npcManager.OnSpawnRecipe -= NpcManager_OnSpawnRecipe;
        npcManagers.Remove(npcManager);

    }



}
