using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    private AudioSource audioSource;

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
    }
}
