using UnityEngine;

public class NpcAnimation : MonoBehaviour
{
    private const string IS_WALK = "IsWalk";

    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void SetWalk(bool isWalk)
    {
        animator.SetBool(IS_WALK, isWalk);
    }

}
