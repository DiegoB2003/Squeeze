using UnityEngine;

public class FeedbackScript : MonoBehaviour
{
    public TMPro.TextMeshProUGUI feedbackText;

    private void Start()
    {
        ShowFeedback(); //Call the method to show feedback when the script starts
    }

    public void ShowFeedback() //Method to show feedback based on the ShopManager's state
    {
        string feedback = "Try to improve the following:\n";
        bool hasFeedback = false;

        if (ShopManager.Instance.servedLate)
        {
            feedback += "- You served a customer late.\n";
            hasFeedback = true;
        }
        if (ShopManager.Instance.soldWrongItem)
        {
            feedback += "- You sold a wrong item.\n";
            hasFeedback = true;
        }
        if (ShopManager.Instance.wastedItemsCrafting)
        {
            feedback += "- You wasted items while crafting.\n";
            hasFeedback = true;
        }
        if (ShopManager.Instance.priceTooHigh)
        {
            feedback += "- Atleast one price was too high.\n";
            hasFeedback = true;
        }
        if (ShopManager.Instance.priceTooLow)
        {
            feedback += "- Atleast one price was too low.\n";
            hasFeedback = true;
        }

        if (hasFeedback)
        {
            if (feedbackText != null)
            {
                feedbackText.text = feedback;
            }
            else
            {
                Debug.LogWarning("FeedbackText is not assigned.");
            }
        }
    }
}
