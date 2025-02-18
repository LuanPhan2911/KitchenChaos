using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance { get; private set; }

    private const string PLAYER_PREFS_SOUND_EFFECT_VOLUME = "SoundEffectVolume";
    [SerializeField] private AudioClipRefSO audioClipRefSO;

    float volume = 0.5f;
    private void Awake()
    {
        Instance = this;
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECT_VOLUME, 0.5f);
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

        Player.OnAnyPlayerPickupSomething += (sender, args) =>
        {
            Player player = sender as Player;
            PlaySound(audioClipRefSO.objectPickUp, player.transform.position);
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


    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(
            audioClipArray[Random.Range(0, audioClipArray.Length)], position, volumeMultiplier * volume);
    }

    public void PlayFootstepSound(Vector3 position)
    {
        PlaySound(audioClipRefSO.footstep, position);
    }
    public void PlayCountdownSound()
    {
        PlaySound(audioClipRefSO.warning, Vector3.zero);
    }
    public void PlayStoveWarningSound(Vector3 position)
    {
        PlaySound(audioClipRefSO.warning, position);
    }

    public float GetSoundEffectVolume()
    {
        return volume;
    }
    public void UpdateSoundEffectVolume()
    {

        if (volume >= 1f)
        {
            volume = 0f;
        }
        else
        {
            volume += 0.1f;
        }

        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECT_VOLUME, volume);
        PlayerPrefs.Save();
    }

}
