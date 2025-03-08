using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateOrder : MonoBehaviour
{
    public TMP_Text OrderFromCustomer; // Reference to the UI Text component

    // Array holding random items
    //TODO Add actual orders not just random items
    private string[] randomTexts = { "Tea", "Grapes", "Lemonade"};

    void Start()
    {   
        // Call the RandomizeText method
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
        return randomTexts[Random.Range(0, randomTexts.Length)];
    }
}