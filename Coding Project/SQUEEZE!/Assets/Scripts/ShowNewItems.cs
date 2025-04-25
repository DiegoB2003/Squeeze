using UnityEngine;

public class ShowNewItems : MonoBehaviour
{
    [SerializeField] private GameObject strawberryPanel;
    // [SerializeField] private GameObject strawberryLemonadePanel;
    [SerializeField] private GameObject raspberryPanel;
    [SerializeField] private GameObject raspberryLemonadePanel;
    int day = ShopManager.Instance.day;
    int raspberryUnlockDay = ShopManager.Instance.raspberryUnlockDay;
    int strawberryUnlockDay = ShopManager.Instance.strawberryUnlockDay;

    void Start()
    {
        showRaspberryItems();
        showStrawberryItems();

    }   

    private void showRaspberryItems(){
        if (day == raspberryUnlockDay) {
            raspberryPanel.SetActive(true);
            raspberryLemonadePanel.SetActive(true);
        } else {
            raspberryPanel.SetActive(false);
            raspberryLemonadePanel.SetActive(false);
        }
    }

    private void showStrawberryItems(){
        if (day == strawberryUnlockDay) {
            strawberryPanel.SetActive(true);
            // strawberryLemonadePanel.SetActive(true);
        } else {
            strawberryPanel.SetActive(false);
            // strawberryLemonadePanel.SetActive(false);
        }
    }
}
