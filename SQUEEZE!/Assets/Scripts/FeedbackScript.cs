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
        string feedback = "Try to improve the following for a higher rating and more profit:\n";
        bool hasFeedback = false;

        if (ShopManager.Instance.servedLate)
        {
            feedback += "- Try serving customers in time.\n";
            hasFeedback = true;
        }
        if (ShopManager.Instance.soldWrongItem)
        {
            feedback += "- Don't serve the wrong product.\n";
            hasFeedback = true;
        }
        if (ShopManager.Instance.wastedItemsCrafting)
        {
            feedback += "- Craft with the right ingredients to avoid waste.\n";
            hasFeedback = true;
        }
        if (ShopManager.Instance.priceTooHigh)
        {
            feedback += "- Consider lowering high prices.\n";
            hasFeedback = true;
        }
        if (ShopManager.Instance.priceTooLow)
        {
            feedback += "- Consider increasing some of the lower prices from default.\n";
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
