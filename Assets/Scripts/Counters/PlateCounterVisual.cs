using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlateCounterVisual : MonoBehaviour
{
    [SerializeField] private Transform counterTopSpawn;
    [SerializeField] private Transform plateVisualPrefab;

    [SerializeField] private PlateCounter plateCounter;
    private List<Transform> plateVisualTransforms;

    private void Awake()
    {
        plateVisualTransforms = new List<Transform>();
    }

    private void Start()
    {
        plateCounter.OnPlateSpawn += (sender, args) =>
        {
            Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopSpawn);

            float offsetY = 0.1f;

            plateVisualTransform.localPosition = new Vector3(0, offsetY * plateVisualTransforms.Count, 0);
            plateVisualTransforms.Add(plateVisualTransform);
        };
        plateCounter.OnPlateGrabbed += (sender, args) =>
        {
            Transform removedPlateVisual = plateVisualTransforms.Last();
            plateVisualTransforms.Remove(removedPlateVisual);
            Destroy(removedPlateVisual.gameObject);

        };
    }
}
