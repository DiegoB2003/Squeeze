using UnityEngine;

public class LoadSceneCheck : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int day = ShopManager.Instance.day;
    int raspberryUnlockDay = ShopManager.Instance.raspberryUnlockDay;
    int strawberryUnlockDay = ShopManager.Instance.strawberryUnlockDay;

    public void checkWhatSceneToLoad(){
        if (day == raspberryUnlockDay || day == strawberryUnlockDay){
            SceneManagerScript.Instance.LoadScene("NewItemsScene");
        } else {
            SceneManagerScript.Instance.LoadScene("UserPriceScene");
        }
    }
}
