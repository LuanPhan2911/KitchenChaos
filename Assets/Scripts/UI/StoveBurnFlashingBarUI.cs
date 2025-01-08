using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBurnFlashingBarUI : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    private const string IS_FLASHING = "IsFlashing";
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {

        stoveCounter.OnProgressChanged += (sender, args) =>
        {
            float burnShowWarningAmount = 0.5f;
            bool isFlashing = stoveCounter.IsFried() && args.progressNormalized >= burnShowWarningAmount;
            animator.SetBool(IS_FLASHING, isFlashing);
        };

    }

}
