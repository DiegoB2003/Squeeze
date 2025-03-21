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
//ShopManager class is responsible for managing the shop, inventory, and sales records
public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance; //Instance of shop manager for other files to access

    public int totalMoney = 50; //The player starts with $50 on the game
    public int day = 1;
    public int rating = 100;     //numerical representation of the rating
    public DateTime startTime = DateTime.Now;       //Start tacing track of the time
    public GameObject raspberryItemObject; // Reference to the Raspberry item GameObject

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
         { "Grape", 3 },
         { "Raspberry", 10}
    };
    
    //Dictionary stores sales records for each day
    public Dictionary<int, List<SaleRecords>> dailySalesRecords = new Dictionary<int, List<SaleRecords>>();
    public List<int> totalMoneyForDay = new List<int>();
    public List<int> EODBalance = new List<int>();
    
    // Text fields for each item to display the quantity
    public TextMeshProUGUI lemonText;
    public TextMeshProUGUI sugarText;
    public TextMeshProUGUI teaText;
    public TextMeshProUGUI grapeText;
    public TextMeshProUGUI lemonadeText;
    public TextMeshProUGUI raspberryText;

    //Initialize the star images
    public Image starOne;
    public Image starTwo;
    public Image starThree;
    public Image starFour;
    public Image starFive;


    
    // Initializing order variable.
    private TextMeshProUGUI order;

    void Awake()
    {
        Debug.Log("SHOP MANAGER AWAKE");
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
        Debug.Log("STARTING SHOP MANAGER");
        raspberryItemObject = GameObject.Find("Ingredient5");
        moneyText = GameObject.Find("Money Text")?.GetComponent<TextMeshProUGUI>();
        dayText = GameObject.Find("Day Text")?.GetComponent<TextMeshProUGUI>();
        InitializeInventory();  // Initialize inventory to start with 0
        UpdateUI(); //Updates UI with information
        UpdateInventoryText();//updates inventory text
        totalMoneyForDay.Add(totalMoney);
        if (raspberryItemObject != null)
        {
            raspberryItemObject.SetActive(false); // Hide Raspberry item initially
        }
        else
        {
            Debug.LogError("Raspberry Item Object is not assigned.");
        }

        // Set the Raspberry inventory text to invisible at the start
        if (raspberryText != null)
        {
            raspberryText.gameObject.SetActive(false); // Hide Raspberry inventory text initially
        }
        else
        {
            Debug.LogError("Raspberry Text UI element not found.");
        }

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
            //restart the timer for the next customer (this is used in the star rating function)
            startTime = DateTime.Now;
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
        inventory["Raspberry"] = 0;
    }

    void starRating()
    {
        // Calculate the time taken to serve the customer
        DateTime endTime = DateTime.Now;
        TimeSpan timeTaken = endTime - startTime;

        Debug.Log("Time taken to serve the customer: " + timeTaken);
        
        // Determine if the customer was served quickly
        bool servedQuickly = timeTaken.Seconds < 10;
        Debug.Log(servedQuickly 
            ? "You served the customer in less than 10 seconds!" 
            : "You took more than 10 seconds to serve the customer.");

        // Update rating within bounds (0 to 100)
        int delta = servedQuickly ? 10 : -10;   // Increase rating if served quickly, decrease if not
        rating = Mathf.Clamp(rating + delta, 0, 100);   // Ensure rating is within bounds

        // Only update stars if rating is divisible by 20
        if (rating % 20 == 0)
        {
            // List of star images
            Image[] stars = { starOne, starTwo, starThree, starFour, starFive };
            int starIndex = rating / 20; // Determines the number of visible stars

            // Enable or disable stars based on rating
            for (int i = 0; i < stars.Length; i++)
                stars[i].enabled = i < starIndex;
        }
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
            
            //check if we need to remove a start or add a star 
            starRating();

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

        if (raspberryText != null)
        {
            raspberryText.text = "Raspberries: " + inventory["Raspberry"].ToString();
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
             // Conditionally show Raspberry item and text based on the current day (visible from day 3)
            if (day >= 3)
            {
                if (raspberryItemObject != null)
                    raspberryItemObject.SetActive(true);
                if (raspberryText != null)
                    raspberryText.gameObject.SetActive(true);
            }
            else
            {
                if (raspberryItemObject != null)
                    raspberryItemObject.SetActive(false);
                if (raspberryText != null)
                    raspberryText.gameObject.SetActive(false);
            }
        
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
        raspberryText = GameObject.Find("raspberryText")?.GetComponent<TextMeshProUGUI>();

        //Below we find the star images and assign them to the variables
        starOne = GameObject.Find("StarOne").GetComponent<Image>();
        starTwo = GameObject.Find("StarTwo").GetComponent<Image>();
        starThree = GameObject.Find("StarThree").GetComponent<Image>();
        starFour = GameObject.Find("StarFour").GetComponent<Image>();
        starFive = GameObject.Find("StarFive").GetComponent<Image>();


        //Find and reconnect buttons
        AssignBuyButton("BuyLemonButton", "Lemon");
        AssignBuyButton("BuySugarButton", "Sugar");
        AssignBuyButton("BuyTeaButton", "Tea");
        AssignBuyButton("BuyGrapesButton", "Grape");
        AssignBuyButton("BuyRaspberryButton", "Raspberry");

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
        EODBalance.Add(totalMoney);
        day++;//increments the day
        dayText.text = "Day: " + day; // Update the day text in the UI
        Debug.Log("It's now day " + day); 
       SceneManager.LoadScene("GraphScene");
    }
}
