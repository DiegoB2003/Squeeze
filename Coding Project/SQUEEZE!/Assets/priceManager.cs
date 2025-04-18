using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PriceManager : MonoBehaviour
{
    public TMP_InputField lemonInput;
    public TMP_InputField sugarInput;
    public TMP_InputField teaInput;
    public TMP_InputField grapesInput;
    public TMP_InputField lemonadeInput;
    public TMP_InputField raspberryInput;
    public TMP_InputField strawberryInput;
    public TMP_InputField raspberryLemonadeInput;
    
    // public GameObject raspberryObject;
    // public GameObject raspberryLemonadeObject;
    // public GameObject strawberryObject;
    // public GameObject strawberryLemonadeObject;

    private Dictionary<string, TMP_InputField> inputFields;

    void Start()
    {
        var sales = ShopManager.Instance.itemSales;
        // var day = ShopManager.Instance.day;

        inputFields = new Dictionary<string, TMP_InputField>()
        {
            { "Lemon", lemonInput },
            { "Sugar", sugarInput },
            { "Tea", teaInput },
            { "Grapes", grapesInput },
            { "Raspberry", raspberryInput },
            { "Strawberry", strawberryInput },
            { "Lemonade", lemonadeInput },
            { "RaspberryLemonade", raspberryLemonadeInput }
        };

        foreach (var item in inputFields)
        {
            if (sales.TryGetValue(item.Key, out int value)){
                item.Value.text = value.ToString();
            }

            item.Value.onEndEdit.AddListener((input) => OnPriceChanged(item.Key, input));//listens for changes and updates it
        }

        // if (day > 3)
        // {
        //     raspberryObject.SetActive(true);
        //     raspberryLemonadeObject.SetActive(true);
        // }
        // else
        // {
        //     raspberryObject.SetActive(false);
        //     raspberryLemonadeObject.SetActive(false);
        // }

        // if(day > 4){
        //      strawberryObject.SetActive(true);
        //      strawberryLemonadeObject.SetActive(true);
        // }
        // else{
        //     strawberryObject.SetActive(false);
        //     strawberryLemonadeObject.SetActive(false);
        // }
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
}
