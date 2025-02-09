using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------------ Audio Sources ------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------------ Audio Clip ------------")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClick;

    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

}
