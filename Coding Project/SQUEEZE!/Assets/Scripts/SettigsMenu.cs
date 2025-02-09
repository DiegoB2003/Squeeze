using UnityEngine;
using UnityEngine.SceneManagement;

public class SettigsMenu : MonoBehaviour
{
    public void GoToSettingsMainMenu()
    {
        SceneManager.LoadSceneAsync(2);
    }
}
