using UnityEngine;

public class SpawnNpcWaitingPositionContainer : MonoBehaviour
{
    [SerializeField] private SpawnNpcWaitingPosition[] spawnNpcWaitingPositionArray;

    public bool TryGetAvailableWaitingPosition(out SpawnNpcWaitingPosition spawnNpcWaitingPosition)
    {
        spawnNpcWaitingPosition = null;
        foreach (var item in spawnNpcWaitingPositionArray)
        {
            if (!item.isUsaged)
            {
                spawnNpcWaitingPosition = item;
                return true;
            }
        }
        return false;
    }
    public SpawnNpcWaitingPosition GetSpawnNpcWaitingPosition(int index)
    {
        return spawnNpcWaitingPositionArray[index];
    }
    public SpawnNpcWaitingPosition GetRandomNpcWaitingPosition(out int index)
    {
        int count = 0;
        while (count < spawnNpcWaitingPositionArray.Length)
        {
            index = UnityEngine.Random.Range(0, spawnNpcWaitingPositionArray.Length);
            if (spawnNpcWaitingPositionArray[index].isUsaged)
            {
                count++;
            }
            else
            {
                return spawnNpcWaitingPositionArray[index];
            }
        }
        index = -1;
        return null;

    }
}
