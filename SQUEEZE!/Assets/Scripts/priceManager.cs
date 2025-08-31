using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PriceManager : MonoBehaviour
{
    public TMP_InputField teaInput;
    public TMP_InputField grapesInput;
    public TMP_InputField lemonadeInput;
    public TMP_InputField raspberryInput;
    public TMP_InputField strawberryInput;
    public TMP_InputField raspberryLemonadeInput;

    private Dictionary<string, TMP_InputField> inputFields;

    void Start()
    {
        var sales = ShopManager.Instance.itemSales;
        int day = ShopManager.Instance.day;
        int raspberryUnlockDay = ShopManager.Instance.raspberryUnlockDay;
        int strawberryUnlockDay = ShopManager.Instance.strawberryUnlockDay;

        inputFields = new Dictionary<string, TMP_InputField>()
        {
            { "Tea", teaInput },
            { "Grapes", grapesInput },
            { "Raspberries", raspberryInput },
            { "Strawberries", strawberryInput },
            { "Lemonade", lemonadeInput },
            { "Raspberry Lemonade", raspberryLemonadeInput }
        };

        foreach (var item in inputFields)
        {
            if (sales.TryGetValue(item.Key, out int value)){
                item.Value.text = value.ToString();
            }

            item.Value.onEndEdit.AddListener((input) => OnPriceChanged(item.Key, input));//listens for changes and updates it
        }

        if (day >= raspberryUnlockDay)
        {
            GetParentOfInput(raspberryInput).SetActive(true);
            GetParentOfInput(raspberryLemonadeInput).SetActive(true);
        }
        else
        {
            GetParentOfInput(raspberryInput).SetActive(false);
            GetParentOfInput(raspberryLemonadeInput).SetActive(false);
        }

        if(day >= strawberryUnlockDay){
            GetParentOfInput(strawberryInput).SetActive(true);
            // GetParentOfInput(strawberryLemonadeInput).SetActive(true);
        }
        else{
            GetParentOfInput(strawberryInput).SetActive(false);
            // GetParentOfInput(strawberryLemonadeInput).SetActive(false);
        }
    }

    void OnPriceChanged(string itemName, string input)
    {
        if (int.TryParse(input, out int value))
        {
            ShopManager.Instance.itemSales[itemName] = value;
            Debug.Log($"Updated {itemName} to ${value}");
        }
        else
        {
            Debug.LogWarning($"Invalid input for {itemName}: '{input}'");
        }
    }

    public GameObject GetParentOfInput(TMP_InputField input)
    {
        if (input == null) return null;

        return input.gameObject.transform.parent != null ? input.gameObject.transform.parent.gameObject : null;
    }
}
