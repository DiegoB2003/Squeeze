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
            // Randomly select an item from the array
            int randomIndex = Random.Range(0, randomTexts.Length);

            // Assign the randomly selected item to the UI Text component
            OrderFromCustomer.text = randomTexts[randomIndex];
        }
        else
        {
            Debug.LogError("OrderFromCustomer Text is not assigned in the Inspector.");
        }
    }

    // Method to get a random order
    public string GetRandomOrder()
    {
        int randomIndex = Random.Range(0, randomTexts.Length);
        return randomTexts[randomIndex];
    }
}