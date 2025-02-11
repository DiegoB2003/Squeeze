using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------------ Audio Sources ------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------------ Audio Clip ------------")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClick;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }   

    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.clip = clip;
    }

}
