using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    private AudioSource audioSource;
    private bool playWarningSound;
    private float playWarningSoundTimer;
    private float maxPlayWarningSoundTime = 0.2f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        stoveCounter.OnChangeState += (sender, args) =>
        {
            bool playSound = args.state == StoveCounter.State.Frying || args.state == StoveCounter.State.Fried;
            if (playSound)
            {
                audioSource.Play();
            }
            else
            {
                audioSource.Pause();
            }
        };
        stoveCounter.OnProgressChanged += (sender, args) =>
        {
            float burnShowWarningAmount = 0.5f;
            playWarningSound = stoveCounter.IsFried() && args.progressNormalized >= burnShowWarningAmount;
        };
    }

    private void Update()
    {
        if (playWarningSound)
        {
            playWarningSoundTimer -= Time.deltaTime;
            if (playWarningSoundTimer <= 0f)
            {
                playWarningSoundTimer = maxPlayWarningSoundTime;
                SoundManager.Instance.PlayStoveWarningSound(transform.position);
            }
        }
    }
}
