using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;
using System;

public class CreateOrder : MonoBehaviour
{
    public TMP_Text OrderFromCustomer; // Reference to the UI Text component

    // Array holding random items
    List<string> items = new List<string> { "Tea", "Grapes", "Lemonade"};

    System.Random random = new();

    void Start()
    {   
        // To get day variable
        int day = (int)(GameObject.Find("GameManager")?.GetComponent<ShopManager>().day);
        // Call the RandomizeText method
        if (day >= 3) {
            Debug.Log("Adding Raspberries and Raspberry Lemonade to order pool.");
            items.Add("Raspberries");
            items.Add("Raspberry Lemonade");
        }
        if (day >= 4) {
            Debug.Log("Adding Raspberries and Raspberry Lemonade to order pool.");
            items.Add("Strawberries");
        }
        RandomizeText();
    }

    public void RandomizeText()
    {
        if (OrderFromCustomer != null)
        {   
            // Assign the randomly selected item to the UI Text component
            OrderFromCustomer.text = GetRandomOrder();
        }
        else
        {
            Debug.LogError("OrderFromCustomer Text is not assigned in the Inspector.");
        }
    }

    // Method to get a random order
    public string GetRandomOrder()
    {
        int index = random.Next(items.Count);   // Get a random index
        return items[index];
    }
}