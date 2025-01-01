using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioClipRefSO audioClipRefSO;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += (sender, args) =>
        {
            PlaySound(audioClipRefSO.deliverySuccess, DeliveryCounter.Instance.transform.position);
        };
        DeliveryManager.Instance.OnRecipeFailed += (sender, args) =>
        {
            PlaySound(audioClipRefSO.deliveryFailed, DeliveryCounter.Instance.transform.position);
        };

        CuttingCounter.OnAnyCut += (sender, args) =>
        {
            CuttingCounter cuttingCounter = sender as CuttingCounter;
            PlaySound(audioClipRefSO.chop, cuttingCounter.transform.position);
        };

        Player.Instance.OnPickupSomething += (sender, args) =>
        {
            PlaySound(audioClipRefSO.objectPickUp, Player.Instance.transform.position);
        };

        BaseCounter.OnAnyObjectPlace += (sender, args) =>
        {
            BaseCounter baseCounter = sender as BaseCounter;
            PlaySound(audioClipRefSO.objectDrop, baseCounter.transform.position);
        };
        TrashCounter.OnDrop += (sender, args) =>
        {
            TrashCounter trashCounter = sender as TrashCounter;
            PlaySound(audioClipRefSO.trash, trashCounter.transform.position);
        };
    }


    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }

    public void PlayFootstepSound(Vector3 position, float volume)
    {
        PlaySound(audioClipRefSO.footstep, position, volume);
    }

}
