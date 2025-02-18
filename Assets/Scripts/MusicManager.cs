using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;

    public static MusicManager Instance { get; private set; }
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    float volume = 0.5f;
    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 0.5f);
        audioSource.volume = volume;
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
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }
}
