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
}
