using UnityEngine;

public class ResetDailyTransactionScript : MonoBehaviour
{
    //Function called when button clicked to reset day transaction array and adds current money to the list
    public void ResetTotalMoneyForDay()
    {
        ShopManager.Instance.DaysTransactionsBalance.Clear();
        ShopManager.Instance.DaysTransactionsBalance.Add(ShopManager.Instance.totalMoney);
        Debug.Log("Daily transactions have been reset and current money added.");
    }

    //Function to reset all booleans to false at the start of day from shop manager
    public void ResetAllBooleans()
    {
        ShopManager.Instance.servedLate = false;
        ShopManager.Instance.soldWrongItem = false;
        ShopManager.Instance.wastedItemsCrafting = false;
        ShopManager.Instance.priceTooHigh = false;
        ShopManager.Instance.priceTooLow = false;
        Debug.Log("All boolean values have been reset to false.");
    }
}
