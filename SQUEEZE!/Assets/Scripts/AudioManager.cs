using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("------------ Audio Sources ------------")]
    [SerializeField] AudioSource musicSource;
    // [SerializeField] AudioSource SFXSource;
    [SerializeField] private AudioMixer myMixer;

    [Header("------------ Audio Clip ------------")]
    public AudioClip backgroundMusic;
    // public AudioClip buttonClick;

    private static AudioManager instance;

    private void Awake()
    {
        // If an audio instance already exists, destroy this one
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set this audio instance as the main instance
        instance = this;
        DontDestroyOnLoad(gameObject);
    }   

    private void Start()
    {
        // Play music only if it's not already playing
        if (!musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
            if (PlayerPrefs.HasKey("musicVolume"))
            {
                myMixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("musicVolume")) * 20);
            }
        }

    }

    // public void PlaySFX(AudioClip clip)
    // {
    //     SFXSource.clip = clip;
    // }

}
