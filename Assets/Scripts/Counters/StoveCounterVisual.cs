using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveOnVisual;
    [SerializeField] private GameObject particle;


    private void Start()
    {
        stoveCounter.OnChangeState += (sender, args) =>
        {
            bool showVisual = args.state == StoveCounter.State.Frying || args.state == StoveCounter.State.Fried;
            stoveOnVisual.SetActive(showVisual);
            particle.SetActive(showVisual);

        };
    }
}
