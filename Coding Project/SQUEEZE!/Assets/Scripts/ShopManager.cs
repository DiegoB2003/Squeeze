using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
using System;
using System.Collections;
using System.ComponentModel.Design;


[System.Serializable]
public class SaleRecords {
    public int day;
    public string item;
    public int price;

    public SaleRecords(int day, string item, int price) {
        this.day = day;
        this.item = item;
        this.price = price;
    }
}

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance; //Instance of shop manager for other files to access

    public int totalMoney = 50; //The player starts with $50 on the game
    public int day = 1;

    public TextMeshProUGUI moneyText; //Text box that displays total money
    public TextMeshProUGUI dayText;

    //Dictionary to store item names to the amount the player has in their inventory
    public Dictionary<string, int> inventory = new Dictionary<string, int>();

    //Dictionary mapping item names to their costs
    private Dictionary<string, int> itemCosts = new Dictionary<string, int>()
    {
         { "Lemon", 2 },
         { "Sugar", 1 },
         { "Tea", 4 },
         { "Grape", 3 }
    };
    
    //Dictionary stores sales records for each day
    public Dictionary<int, List<SaleRecords>> dailySalesRecords = new Dictionary<int, List<SaleRecords>>();
    public List<int> totalMoneyForDay = new List<int>();
    
    // Text fields for each item to display the quantity
    public TextMeshProUGUI lemonText;
    public TextMeshProUGUI sugarText;
    public TextMeshProUGUI teaText;
    public TextMeshProUGUI grapeText;
    public TextMeshProUGUI lemonadeText;
    
    // Initializing order variable.
    private TextMeshProUGUI order;

    void Awake()
    {
        //Make sure only 1 instance of the shop manager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        moneyText = GameObject.Find("Money Text")?.GetComponent<TextMeshProUGUI>();
        dayText = GameObject.Find("Day Text")?.GetComponent<TextMeshProUGUI>();
        InitializeInventory();  // Initialize inventory to start with 0
        UpdateUI(); //Updates UI with information
        UpdateInventoryText();//updates inventory text
        totalMoneyForDay.Add(totalMoney);
    }

    void MoveCustomerOffScreen()
    {
        // Find the customer object
        GameObject customer = GameObject.Find("DuckCustomer");

        //Hide the Order Panel and randomize order again.
        GameObject orderPanel = GameObject.Find("OrderPanel");
        GameObject orderUI = GameObject.Find("OrderUI");
        if (orderPanel != null)
        {
            orderPanel.SetActive(false);
            orderUI.GetComponent<CreateOrder>().RandomizeText();
        }

        //Hide the Order Button
        Button takeOrderButton = GameObject.Find("TakeOrderButton")?.GetComponent<Button>();
        if (takeOrderButton != null)
        {
            takeOrderButton.gameObject.SetActive(false);
        }

        if (customer != null)
        {
            // Start the coroutine to move the customer off the screen
            StartCoroutine(MoveCustomerCoroutine(customer));
        }
        else
        {
            Debug.LogError("Customer object not found.");
        }
    }

    IEnumerator MoveCustomerCoroutine(GameObject customer)
    {
        float speed = 5f; // Adjust the speed as needed
        Vector3 offScreenPosition = new Vector3(10, customer.transform.position.y, customer.transform.position.z);

        // Move the customer off the screen
        while (Vector3.Distance(customer.transform.position, offScreenPosition) > 0.1f)
        {   
  
            // Move the customer towards the off-screen position
            customer.transform.position = Vector3.MoveTowards(customer.transform.position, offScreenPosition, speed * Time.deltaTime);

            // Wait for the next frame
            yield return null;
        }

        // Ensure the customer is exactly at the off-screen position
        customer.transform.position = offScreenPosition;

        // Wait for a short duration before resetting the position
        yield return new WaitForSeconds(1f);

        // Instantly move the customer back to the start position
        customer.transform.position = new Vector3(-10, customer.transform.position.y, customer.transform.position.z);

        // Check if day ended when enough customers were served
        if(dailySalesRecords[day].Count >= 3+day){
            EndDay();
        }

        // Start the movement again
        SimpleMovement simpleMovement = customer.GetComponent<SimpleMovement>();
        if (simpleMovement != null)
        {
            simpleMovement.StartMovement();
        }
    }


    // Initialize the inventory with 0 items
    void InitializeInventory()
    {
        inventory["Lemon"] = 0;
        inventory["Sugar"] = 0;
        inventory["Tea"] = 0;
        inventory["Grape"] = 0;
        inventory["Lemonade"] = 0;
    }

    //Function called with button click to purchase an item
    //Takes in the name of the item to purchase, MUST BE EXACT
    public void PurchaseItem(string itemName)
    {
        //If the item is not in the dictionary, it does not exist to buy
        if (!itemCosts.ContainsKey(itemName))
        {
            Debug.LogError("Cost for item " + itemName + " is not defined!");
            return;
        }

        int cost = itemCosts[itemName];

        //Check if the player has enough money to purchase the item
        if (totalMoney >= cost)
        {
            totalMoney -= cost;
            UpdateUI();

            //Add the purchased item to player inventory
            if (inventory.ContainsKey(itemName))
            {
                inventory[itemName]++;
            }
            else
            {
                inventory[itemName] = 1;
            }

            Debug.Log("Purchased " + itemName + " for $" + cost + ". Total " + itemName + " in inventory: " + inventory[itemName]);

            // Update inventory text after purchase
            UpdateInventoryText();
            totalMoneyForDay.Add(totalMoney); //Add the total money to the list
            Debug.Log("Total money for the day: " + string.Join(", ", totalMoneyForDay));
        }
        else
        {
            Debug.Log("Not enough money to purchase " + itemName + " which costs $" + cost + "!");
        }
    }

    // Sells item. Removes item from inventory and gives money to player.
    public void SellItem(){
        order = GameObject.Find("CustomerOrder")?.GetComponent<TextMeshProUGUI>();
        string orderItem = order.text;
        if (orderItem == "Grapes") {
            orderItem = "Grape";
        }
        if (inventory[orderItem] >= 1) {
            inventory[orderItem] -= 1;
            int salePrice = 5;
            totalMoney += salePrice;
            UpdateUI();
            UpdateInventoryText();

            // Record the sale
            RecordSale(orderItem, salePrice);
            totalMoneyForDay.Add(totalMoney); //Add the total money to the list
            Debug.Log("Total money for the day: " + string.Join(", ", totalMoneyForDay));

            // Move the customer off the screen after served
            MoveCustomerOffScreen();
        } else {
            Debug.Log($"Not enough {orderItem} in inventory to sell!");
        }
    }

    public void CraftLemonade(){
        if (inventory["Lemon"] >= 2 && inventory["Sugar"] >= 1){
            inventory["Lemon"] -= 2;
            inventory["Sugar"] -= 1;
            inventory["Lemonade"] += 1;
            UpdateInventoryText();
        }
        else{
            Debug.Log("Not enough ingredients to craft Lemonade!");
        }
    }

    //Update the UI text to show the current money.
    void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "Cash: $" + totalMoney;
        }
    }

    // Update the displayed quantities of items in the inventory
    void UpdateInventoryText()
    {   
        if (lemonadeText != null)
        {
            lemonadeText.text = "Lemonade: " + inventory["Lemonade"].ToString();
        }

        if (lemonText != null)
        {
            lemonText.text = "Lemons: " + inventory["Lemon"].ToString();
        }

        if (sugarText != null)
        {
            sugarText.text = "Sugar: " + inventory["Sugar"].ToString();
        }

        if (teaText != null)
        {
            teaText.text = "Tea: " + inventory["Tea"].ToString();
        }

        if (grapeText != null)
        {
            grapeText.text = "Grapes: " + inventory["Grape"].ToString();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Only update when returning to the GameScene
        if (scene.name == "GameScene")
        {
            moneyText = GameObject.Find("Money Text")?.GetComponent<TextMeshProUGUI>();
            dayText = GameObject.Find("Day Text")?.GetComponent<TextMeshProUGUI>();
            dayText.text = "Day: " + day;

            if (moneyText == null)
            {
                Debug.LogError("MoneyText UI element not found in the scene.");
            }

            AssignUI(); //Reconnect buttons

            if (inventory.Count == 0) //If inventory is empty, initialize it
            {
                InitializeInventory();
            }

            UpdateUI(); //Refresh the displayed money amount
            UpdateInventoryText(); // Refresh inventory display
        }
    }

    void AssignUI()
    {
        moneyText = GameObject.Find("Money Text")?.GetComponent<TextMeshProUGUI>();
        dayText = GameObject.Find("Day Text")?.GetComponent<TextMeshProUGUI>();
        lemonText = GameObject.Find("lemonText")?.GetComponent<TextMeshProUGUI>();
        sugarText = GameObject.Find("sugarText")?.GetComponent<TextMeshProUGUI>();
        teaText = GameObject.Find("teaText")?.GetComponent<TextMeshProUGUI>();
        grapeText = GameObject.Find("grapeText")?.GetComponent<TextMeshProUGUI>();
        lemonadeText = GameObject.Find("lemonadeText")?.GetComponent<TextMeshProUGUI>();

        //Find and reconnect buttons
        AssignBuyButton("BuyLemonButton", "Lemon");
        AssignBuyButton("BuySugarButton", "Sugar");
        AssignBuyButton("BuyTeaButton", "Tea");
        AssignBuyButton("BuyGrapesButton", "Grape");
        AssignCraftButton("CraftLemonadeButton");
        AssignSellButton("ServeButton");
    }

    void AssignBuyButton(string buttonName, string itemName)
    {
        Button button = GameObject.Find(buttonName)?.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners(); //Prevent duplicate listeners
            button.onClick.AddListener(() => PurchaseItem(itemName)); //Reassign listener
        }
    }

    // Sets up "Serve" button aka Selling.
    void AssignSellButton(string buttonName)
    {
        Button button = GameObject.Find(buttonName)?.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners(); //Prevent duplicate listeners
            button.onClick.AddListener(() => SellItem()); //Reassign listener
        }
    }

    void AssignCraftButton(string buttonName)
    {
        Button button = GameObject.Find(buttonName)?.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners(); //Prevent duplicate listeners
            button.onClick.AddListener(() => CraftLemonade()); //Reassign listener
        }
    }

    //Records each sale by adding it to daily sales dictionary
    private void RecordSale(string item, int price)
    {
        SaleRecords sale = new SaleRecords(day, item, price);
        if (!dailySalesRecords.ContainsKey(day))
        {
            dailySalesRecords[day] = new List<SaleRecords>();
        }
        dailySalesRecords[day].Add(sale);
        Debug.Log($"Recorded sale: Day {day}, Item: {item}, Price: ${price}");
    }

    //Not used anywhere but can be used to test/debug
    //Prints all current sales records
    public void PrintSales()
    {
        foreach (KeyValuePair<int, List<SaleRecords>> daySales in dailySalesRecords)
        {
            Debug.Log("Sales for Day: " + daySales.Key);
            foreach (SaleRecords sale in daySales.Value)
            {
                Debug.Log("Item: " + sale.item + ", Price: $" + sale.price);
            }
        }
    }

    private void EndDay(){
        SceneManager.LoadScene("GraphScene");
    }
}
