using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMainMenu : MonoBehaviour
{
    public void BackToMainMenuButton()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
