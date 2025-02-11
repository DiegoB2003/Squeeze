using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToSettings : MonoBehaviour
{
    public void BackToSettingsMenu()
    {
        SceneManager.LoadScene(2);
    }
}
