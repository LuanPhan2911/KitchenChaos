using System;
using UnityEngine;

public class NpcMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    [SerializeField] private float moveSpeed = 3f;

    private NpcManager npcManager;

    private bool isMoveToTarget = false;

    public event EventHandler OnReachTargetPosition;

    private void Awake()
    {
        npcManager = GetComponent<NpcManager>();
    }


    private void Update()
    {
        MoveToTargetPosition();
    }
    public void SetTargetPosition(Vector3 targetPosition)
    {
        isMoveToTarget = false;
        this.targetPosition = targetPosition;
    }
    public Vector3 GetTargetPosition()
    {
        return targetPosition;
    }
    private void MoveToTargetPosition()
    {
        if (isMoveToTarget)
        {
            return;
        }


        if (Vector3.Distance(transform.position, targetPosition) <= NpcManager.minDistance)
        {
            npcManager.npcAnimation.SetWalk(false);
            isMoveToTarget = true;
            OnReachTargetPosition?.Invoke(this, EventArgs.Empty);
            return;
        }
        Vector3 direction = targetPosition - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
        transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        npcManager.npcAnimation.SetWalk(true);
        isMoveToTarget = false;
    }


}
