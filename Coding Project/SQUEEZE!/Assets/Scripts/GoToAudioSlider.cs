using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToAudioSlider : MonoBehaviour
{
    public void GoToAudioSliderMenu()
    {
        // Load the scene with the index 3
        SceneManager.LoadSceneAsync(3);
    }
}
