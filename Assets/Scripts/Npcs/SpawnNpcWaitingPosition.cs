using UnityEngine;

public class SpawnNpcWaitingPosition : MonoBehaviour
{
    public bool isUsaged = false;
    public int npcIndex = -1;


    public void SetNpcUsaged(int npcIndex)
    {
        if (npcIndex != -1)
        {
            this.npcIndex = npcIndex;
            isUsaged = true;
        }
        else
        {
            isUsaged = false;
            this.npcIndex = -1;

        }
    }
    private Vector3 spawnPosition;
    private void Start()
    {
        spawnPosition = transform.position + new Vector3(10f, 0, 0);
    }
    public Vector3 GetWaitingPosition()
    {
        return transform.position;
    }
    public Vector3 GetSpawnPosition()
    {
        return spawnPosition;
    }
}
