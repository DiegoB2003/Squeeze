using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
using System;
using System.Linq;
using System.Collections;
using System.ComponentModel.Design;


[Serializable]
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
    public GameObject errorPopup;    
    public TextMeshProUGUI errorText; 
    public Button errorOkButton;    
    public static ShopManager Instance; //Instance of shop manager for other files to access

    public int totalMoney = 30; //The player starts with $50 on the game
    public int day = 1;
    public int raspberryUnlockDay = 2;
    public int strawberryUnlockDay = 3;
    
    public int rating = 60;     //numerical representation of the rating
    public DateTime startTime = DateTime.Now;       //Start tacing track of the time
    public GameObject raspberryItemObject; // Reference to the Raspberry item GameObject
    public GameObject strawberryItemObject; // Reference to the strawberry item GameObject
    public GameObject CraftRaspberryLemonadeButton;//refrences to craft button for raspberry lemonade
    public TextMeshProUGUI moneyText; //Text box that displays total money
    public TextMeshProUGUI dayText;

    public int timer = 10;

    //Dictionary to store item names to the amount the player has in their inventory
    public Dictionary<string, int> inventory = new Dictionary<string, int>();

    //Dictionary mapping item names to their costs
    private Dictionary<string, int> itemCosts = new Dictionary<string, int>()
    {
         { "Lemon", 2 },
         { "Sugar", 1 },
         { "Tea", 4 },
         { "Grapes", 3 },
         { "Raspberries", 4},
         { "Strawberries",5}
    };

    //Dictionary mapping item names to their sales amounts when they are also changed in the game
    public Dictionary<string, int> itemSales = new Dictionary<string, int>()
    {
         { "Tea", 5 },
         { "Grapes", 4 },
         { "Raspberries", 5},
         { "Strawberries",6},
         { "Lemonade",7},
         { "Raspberry Lemonade",11}
    };

    //Default Dictionary mapping item names to their sales that never changed
    public Dictionary<string, int> defaultItemSales = new Dictionary<string, int>()
    {
         { "Tea", 5 },
         { "Grapes", 4 },
         { "Raspberries", 5},
         { "Strawberries",6},
         { "Lemonade",7},
         { "Raspberry Lemonade",11}
    };
    
    //Dictionary stores sales records for each day
    public Dictionary<int, List<SaleRecords>> dailySalesRecords = new Dictionary<int, List<SaleRecords>>();
    public List<int> DaysTransactionsBalance = new List<int>();
    public List<int> EODBalance = new List<int>();
    
    // Text fields for each item to display the quantity
    public TextMeshProUGUI lemonText;
    public TextMeshProUGUI sugarText;
    public TextMeshProUGUI teaText;
    public TextMeshProUGUI grapeText;
    public TextMeshProUGUI lemonadeText;
    public TextMeshProUGUI raspberryText;
    public TextMeshProUGUI strawberryText;
    public TextMeshProUGUI raspberryLemonadeText;

    //Initialize the star images
    public Image starOne;
    public Image starTwo;
    public Image starThree;
    public Image starFour;
    public Image starFive;

    // Timer bar to hide on serve.
    private GameObject timerBar;

    // Popups for money and stars.
    private GameObject moneyPopUp;
    private GameObject starPopUp;
    
    // Initializing order variable.
    private string orderItem;

    public TMP_InputField firstIngredientInput; //Input field for the first and second ingredient
    public TMP_InputField secondIngredientInput;
    public TMP_Dropdown ingredientDropDown1; //Dropdown for the first and second ingredient
    public TMP_Dropdown ingredientDropDown2;
    public Button craftButton; //Crafts the two ingredients into a new item
    public TMP_Dropdown selectWhatToSell; //Dropdown menu that allows the player to select what to sell

    //Different booleans for each mistake a player can make
    public bool servedLate = false; //Done
    public bool soldWrongItem = false;
    public bool wastedItemsCrafting = false; //Done
    public bool priceTooHigh = false;
    public bool priceTooLow = false;

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
        moneyText = GameObject.Find("Money Text")?.GetComponent<TextMeshProUGUI>();
        dayText = GameObject.Find("Day Text")?.GetComponent<TextMeshProUGUI>();
        InitializeInventory();  // Initialize inventory to start with 0
        UpdateUI(); //Updates UI with information
        UpdateInventoryText();//updates inventory text
        startingStarRating();
        DaysTransactionsBalance.Add(totalMoney);
        raspberryItemObject = null;
        strawberryItemObject = null;
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "raspberryObject")
            {
                raspberryItemObject = obj;
                break;
            }
        }

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "strawberryObject")//looks for strawberry object
            {
                strawberryItemObject = obj;//assigns it to variable
                break;
            }
        }

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "raspberryLemonade")//looks for raspberry lemonade object
            {
                CraftRaspberryLemonadeButton = obj;//assigns it to variable
                break;
            }
        }


        if (raspberryItemObject != null)
        {
            raspberryItemObject.SetActive(false); // Hide Raspberry item initially
            CraftRaspberryLemonadeButton.SetActive(false);//hides button right away
            Debug.Log("found raspberry item");
        }
        else
        {
            Debug.LogError("Raspberry Item Object is not assigned.");
        }

        // Set the Raspberry inventory text to invisible at the start
        if (raspberryText != null)
        {
            raspberryText.gameObject.SetActive(false); // Hide Raspberry inventory text initially
            raspberryLemonadeText.gameObject.SetActive(false); //hide raspberry lemonade text
        }
        else
        {
            Debug.LogError("Raspberry Text UI element not found.");
        }

        if (strawberryItemObject != null)
        {
            strawberryItemObject.SetActive(false); // Hide Raspberry item initially
            Debug.Log("found strawberry item");
        }
        else
        {
            Debug.LogError("strawberry Item Object is not assigned.");
        }

         // Set the strawberry inventory text to invisible at the start
        if (strawberryText != null)
        {
            strawberryText.gameObject.SetActive(false); // Hide strawberry inventory text initially
        }
        else
        {
            Debug.LogError("strawberry Text UI element not found.");
        }

        if(errorPopup!= null){ //sets error popup to initially be hidden
            errorPopup.gameObject.SetActive(false);
        }
    }

    string changeCustomerImage () {
        //Array that holds all the customer images paths
        string[] customerImages = {
            "TheDuck",
            "turtle_customer",
            "bear_customer"
            // "grimace_customer",
            // "biggie_customer",
        };

        //Randomly select a customer image
        int randomIndex = UnityEngine.Random.Range(0, customerImages.Length);
        string selectedImagePath = customerImages[randomIndex];

        //display it in the inspector
        Debug.Log("Selected customer image: " + selectedImagePath);


        return selectedImagePath;
    }

       
    void MoveCustomerOffScreen()
    {
        // Find the customer object
        GameObject customer = GameObject.Find("Customer");
        GameObject dCustomer = GameObject.Find("DuckCustomer");
        //Hide the Order Panel and randomize order again.
        GameObject orderPanel = GameObject.Find("OrderPanel");
        GameObject orderUI = GameObject.Find("OrderUI");
        if (orderPanel != null)
        {
            orderPanel.SetActive(false);
            orderUI.GetComponent<CreateOrder>().RandomizeText();
        }

        if(timerBar != null)
        {
            timerBar.SetActive(false);
        }

        //Hide the Order Button
        Button takeOrderButton = GameObject.Find("TakeOrderButton")?.GetComponent<Button>();
        if (takeOrderButton != null)
        {
            takeOrderButton.gameObject.SetActive(false);
        }

        //Hide the took too long text
        GameObject tookTooLongMessage = GameObject.Find("TookTooLongMessage");
        if (tookTooLongMessage != null)
        {
            tookTooLongMessage.SetActive(false);
        }

        if (customer != null)
        {
            // Start the coroutine to move the customer off the screen
            StartCoroutine(MoveCustomerCoroutine(customer, dCustomer));
        }
        else
        {
            Debug.LogError("Customer object not found.");
        }
    }

    IEnumerator MoveCustomerCoroutine(GameObject customer, GameObject dCustomer)
    {
        float speed = 5f; // Adjust the speed as needed
        Vector3 offScreenPosition = new Vector3(10, customer.transform.position.y, customer.transform.position.z);

        Animator animator = dCustomer.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("isMoving", true); //Start walking animation
        }

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

        if (animator != null)
        {
            animator.SetBool("isMoving", false); //Stop walking animation
        }

        // Wait for a short duration before resetting the position
        yield return new WaitForSeconds(1f);

        // Instantly move the customer back to the start position
        customer.transform.position = new Vector3(-10, customer.transform.position.y, customer.transform.position.z);

        // Reset the customer image
        string newImagePath = changeCustomerImage();

        // Load the new sprite
        Sprite newSprite = Resources.Load<Sprite>(newImagePath); // Load the new sprite from Resources
        SpriteRenderer spriteRenderer = dCustomer.GetComponent<SpriteRenderer>();    
        if (spriteRenderer != null && newSprite != null)
        {
            spriteRenderer.sprite = newSprite; // Change the sprite
        }
        else
        {
            Debug.LogError("Failed to load new sprite or SpriteRenderer is missing.");
        }

        // Reset money popup
        if(moneyPopUp != null) {
            moneyPopUp.SetActive(false);
        } else {
            Debug.LogError("Money Popup null!");
        }

        // Reset star popup
        if(starPopUp != null) {
            starPopUp.SetActive(false);
        } else {
            Debug.LogError("Star Popup null!");
        }

        // Check if day ended when enough customers were served
        if(dailySalesRecords[day].Count >= getCustomerAmount()){
            EndDay();
        }

        // Start the movement again
        SimpleMovement simpleMovement = customer.GetComponent<SimpleMovement>();
        if (simpleMovement != null)
        {
            simpleMovement.StartMovement();
        }
    }

    public void setStartTime(){
        startTime = DateTime.Now;
    }

    // Initialize the inventory with 0 items
    void InitializeInventory()
    {
        inventory["Lemon"] = 0;
        inventory["Sugar"] = 0;
        inventory["Tea"] = 0;
        inventory["Grapes"] = 0;
        inventory["Lemonade"] = 0;
        inventory["Raspberries"] = 0;
        inventory["Strawberries"] = 0;
        inventory["Raspberry Lemonade"] = 0;

    }

    void starRating(string soldWrongItem = null)
    {
        // Calculate the time taken to serve the customer
        DateTime endTime = DateTime.Now;
        TimeSpan timeTaken = endTime - startTime;
        bool servedQuickly = false; // Default value

        if (soldWrongItem == null)
        {
            // Determine if the customer was served quickly
            servedQuickly = timeTaken.Seconds < timer;
            Debug.Log(servedQuickly 
                ? $"You served the customer in less than {timer} seconds!" 
                : $"You took more than {timer} seconds to serve the customer.");
        }
        else if (soldWrongItem == "true")
        {
            servedQuickly = false;
            Debug.Log("You sold the wrong item!");
        }

        // Update rating within bounds (0 to 100)
        int delta = servedQuickly ? 10 : -10; // Increase rating if served quickly, decrease if not
        // Play star popup.
        if(starPopUp != null){
            starPopUp.GetComponent<TextMeshProUGUI>().text = delta>=0 ? $"+{(float)delta/20}<sprite name=\"starPic_0\">" : $"{(float)delta/20}<sprite name=\"starPic_0\">";
            starPopUp.SetActive(true);
        } else {
            Debug.LogError("Star Popup null!");
        }

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

    void startingStarRating()
    {
        rating = Mathf.Clamp(rating, 0, 100);
        if (rating % 20 != 0) {
            rating += 10;
        }
        // List of star images
        Image[] stars = { starOne, starTwo, starThree, starFour, starFive };
        int starIndex = rating / 20; // Determines the number of visible stars

        // Enable or disable stars based on rating
        for (int i = 0; i < stars.Length; i++)
            stars[i].enabled = i < starIndex;
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

            // Update the dropdown menu to include the purchased item
            UpdateDropdownMenu(itemName, 1); // Add item to the dropdown menu

            Debug.Log("Purchased " + itemName + " for $" + cost + ". Total " + itemName + " in inventory: " + inventory[itemName]);

            // Update inventory text after purchase
            UpdateInventoryText();
            DaysTransactionsBalance.Add(totalMoney); //Add the total money to the list
            Debug.Log("Total money for the day: " + string.Join(", ", DaysTransactionsBalance));
        }
        else
        {
            Debug.Log("Not enough money to purchase " + itemName + " which costs $" + cost + "!");
            ShowError("Not enough money to purchase " + itemName + " which costs $" + cost + "!");
        }
    }

    // Sells item. Removes item from inventory and gives money to player.
    public void SellItem(){
        orderItem = GameObject.Find("CustomerOrder")?.GetComponent<TextMeshProUGUI>().text;

        //now get what is selected in the d.d.m.
        int selectedIndex = selectWhatToSell.value;  // Get the index of the currently selected option
        string selectedOption = selectWhatToSell.options[selectedIndex].text;  // Get the text of the selected option
        if (selectedOption == "Select Item")
        {
            Debug.Log("Please select an item to sell!");
            ShowError("Please select an item to sell!");
            return;
        } else if (selectedOption != orderItem){
            soldWrongItem = true;
            // Debug.Log($"Please choose the correct item to sell! Tried to sell {selectedOption}, customer wanted {orderItem}");
            // ShowError($"Please choose the correct item to sell!");
            inventory[selectedOption] -= 1;
            UpdateUI();
            UpdateInventoryText();

            //remove the item from the dropdown menu if needed (none left)
            if (inventory[selectedOption] == 0)
            {
                UpdateDropdownMenu(selectedOption, 2); // Remove item from the dropdown menu
            }

            // Record the sale
            RecordSale(orderItem, 0);
            totalMoney += 0;
            DaysTransactionsBalance.Add(totalMoney); //Add the total money to the list
            Debug.Log("Total money for the day: " + string.Join(", ", DaysTransactionsBalance));
            
            starRating("true");

            // Play money popup.
            if(moneyPopUp != null){
                moneyPopUp.GetComponent<TextMeshProUGUI>().text = $"+${0}";
                moneyPopUp.SetActive(true);
            } else {
                Debug.LogError("Money Popup null!");
            }

            // Move the customer off the screen after served
            MoveCustomerOffScreen();


        } else if (orderItem == selectedOption ) {
            //Check if customer will purchase item using price of default and current price
            if (willCustomerPurchase(itemSales[orderItem], defaultItemSales[orderItem])) {
                //if itemSales price is equal or lower than defaultItemSales price set priceTooLow to true
                if (itemSales[orderItem] <= defaultItemSales[orderItem])
                {
                    priceTooLow = true; //Price was too low so set true for hint
                }

                inventory[orderItem] -= 1;
                int salePrice = itemSales[orderItem];
                totalMoney += salePrice;
                UpdateUI();
                UpdateInventoryText();

                //remove the item from the dropdown menu if needed (none left)
                if (inventory[orderItem] == 0)
                {
                    UpdateDropdownMenu(orderItem, 2); // Remove item from the dropdown menu
                }

                // Record the sale
                RecordSale(orderItem, salePrice);
                DaysTransactionsBalance.Add(totalMoney); //Add the total money to the list
                Debug.Log("Total money for the day: " + string.Join(", ", DaysTransactionsBalance));
                
                //check if we need to remove a start or add a star 
                starRating();

                // Play money popup.
                if(moneyPopUp != null){
                    moneyPopUp.GetComponent<TextMeshProUGUI>().text = $"+${salePrice}";
                    moneyPopUp.SetActive(true);
                } else {
                    Debug.LogError("Money Popup null!");
                }
            } else {
                RecordSale(orderItem, 0);

                // Play money popup.
                if(moneyPopUp != null){
                    moneyPopUp.GetComponent<TextMeshProUGUI>().text = $"+${0}";
                    moneyPopUp.SetActive(true);
                } else {
                    Debug.LogError("Money Popup null!");
                }
                priceTooHigh = true; //Price was too high so set true for hint
                ShowError("This price is way too high for me!");
                //Remove 0.5 stars (JOSE PLEASE DO THIS IDK HOW TOOOOOO)

                starRating("true");

            }

            // Move the customer off the screen after served
            MoveCustomerOffScreen();
        } 
        
        
    }

    public void CraftRaspberryLemonade(){
        if (inventory["Raspberries"] >= 2 && inventory["Sugar"] >= 2){
            inventory["Raspberries"] -= 2;
            inventory["Sugar"] -= 2;
            inventory["Raspberry Lemonade"] += 1;
            UpdateInventoryText();

            //now we remove the Raspberry and sugar from menu if we have 0 of each
            if (inventory["Raspberries"] == 0)
            {
                UpdateDropdownMenu("Raspberries", 2); // Remove item from the dropdown menu
            }

            if (inventory["Sugar"] == 0)
            {
                UpdateDropdownMenu("Sugar", 2); // Remove item from the dropdown menu

            }

            // Update the dropdown menu to include the
            UpdateDropdownMenu("Raspberry Lemonade", 1);
        }
        else{
            Debug.Log("Not enough ingredients to craft raspberry Lemonade!");
            ShowError("Not enough ingredients to craft raspberry Lemonade!\n(Need 2 Raspberries & 2 Sugar)");
        }
    }

    public void CraftLemonade(){
        if (inventory["Lemon"] >= 2 && inventory["Sugar"] >= 1){
            inventory["Lemon"] -= 2;
            inventory["Sugar"] -= 1;
            inventory["Lemonade"] += 1;
            UpdateInventoryText();

            //now we remove the lemons and sugar from menu if we have 0 of each
            if (inventory["Lemon"] == 0)
            {
                UpdateDropdownMenu("Lemon", 2); // Remove item from the dropdown menu
            }

            if (inventory["Sugar"] == 0)
            {
                UpdateDropdownMenu("Sugar", 2); // Remove item from the dropdown menu

            }

            // Update the dropdown menu to include the
            UpdateDropdownMenu("Lemonade", 1); 
        }
        else{
            Debug.Log("Not enough ingredients to craft Lemonade!");
            ShowError("Not enough ingredients to craft Lemonade!\n(Need 2 Lemons & 1 Sugar)");
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
            lemonadeText.text =  "Lemonade: " + inventory["Lemonade"];
        }

        if (lemonText != null)
        {
            lemonText.text = "[" + inventory["Lemon"] + "]";
        }

        if (sugarText != null)
        {
            sugarText.text = "[" + inventory["Sugar"] + "]";
        }

        if (teaText != null)
        {
            teaText.text = "[" + inventory["Tea"] + "]";
        }

        if (grapeText != null)
        {
            grapeText.text = "[" + inventory["Grapes"] + "]";
        }

        if (raspberryText != null)
        {
            raspberryText.text = "[" + inventory["Raspberries"] + "]";
        }

        if (strawberryText != null)
        {
            strawberryText.text = "[" + inventory["Strawberries"] + "]";
        }

          if (raspberryLemonadeText != null)
        {
            raspberryLemonadeText.text = "Rasberry Lemonade: " + inventory["Raspberry Lemonade"];
        }
    }

    //function below edits drop down menu 
    void UpdateDropdownMenu(string itemName = null, int actionType = 3)
    {   
        //if action type is 3, we simply want to refresh the dropdown menu to show the current inventory
        if (actionType == 3)
        {
            //loop through the inventory and add each item to the dropdown
            foreach (KeyValuePair<string, int> item in inventory)
            {
                if (item.Value > 0) // Only add items that are in the inventory
                {

                    //remoe sugar and lemon from the dropdown menu
                    if (item.Key != "Sugar" || item.Key != "Lemon")
                    {
                        selectWhatToSell.options.Add(new TMP_Dropdown.OptionData(item.Key));
                        Debug.Log("Updating dropdown menu " + item.Key);
                    }

                }
            }
        }

        else if (actionType == 1) // Add item to the dropdown menu
        {
            // Check if the item is already in the dropdown
            if (selectWhatToSell.options.Any(option => option.text == itemName))
            {
                Debug.Log($"Item '{itemName}' is already in the dropdown menu.");
                return;
            } else if (itemName == "Lemon" || itemName == "Sugar") {
                return;
            }

            //if item == Raspberry Lemonade resize the dropdown to fit the text
            // if (itemName == "Raspberry Lemonade")
            // {
            //     selectWhatToSell.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 30);

            // }


            // Add the item to the dropdown
            selectWhatToSell.options.Add(new TMP_Dropdown.OptionData(itemName));
            selectWhatToSell.RefreshShownValue(); // Refresh the dropdown to show the new option
            Debug.Log($"Added '{itemName}' to the dropdown menu.");
        
        }
        else if (actionType == 2) // Remove item from the dropdown menu
        {
            // Check if the item exists in the dropdown
            if (!selectWhatToSell.options.Any(option => option.text == itemName))
            {
                Debug.Log($"Item '{itemName}' is not in the dropdown menu.");
                return;
            }

            // Remove the item from the dropdown
            selectWhatToSell.options.RemoveAll(option => option.text == itemName);
            selectWhatToSell.RefreshShownValue(); // Refresh the dropdown to reflect the changes
            Debug.Log($"Removed '{itemName}' from the dropdown menu.");
        }
        else
        {
            Debug.LogError("Invalid action type. Use 1 to add or 2 to delete.");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            moneyText = GameObject.Find("Money Text")?.GetComponent<TextMeshProUGUI>();
            dayText = GameObject.Find("Day Text")?.GetComponent<TextMeshProUGUI>();
            dayText.text = "Day: " + day;

            if (moneyText == null)
            {
                Debug.LogError("MoneyText UI element not found in the scene.");
            }

            AssignUI(); // Reconnect buttons

            errorPopup.SetActive(false);

            if (inventory.Count == 0) // If inventory is empty, initialize it
            {
                InitializeInventory();
            }

            startingStarRating(); // Set star rating

            // Re-find the raspberry object in the current scene
            raspberryItemObject = null;
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "raspberryObject")
                {
                    raspberryItemObject = obj;
                    break;
                }
            }
            if (raspberryItemObject == null)
            {
                Debug.LogError("Raspberry Item Object not found on scene load.");
            }


            // Re-find the raspberry object in the current scene
            strawberryItemObject = null;
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "strawberryObject")
                {
                    strawberryItemObject = obj;
                    break;
                }
            }
            if (strawberryItemObject == null)
            {
                Debug.LogError("strawberry Item Object not found on scene load.");
            }

            // Re-find the raspberry object in the current scene
            CraftRaspberryLemonadeButton = null;
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "raspberryLemonade")
                {
                    CraftRaspberryLemonadeButton = obj;
                    break;
                }
            }
            if (CraftRaspberryLemonadeButton == null)
            {
                Debug.LogError("crafting raspberry leomonade Item Object not found on scene load.");
            }

            // Show raspberries on unlockable day
            if (day >= raspberryUnlockDay)
            {
                if (raspberryItemObject != null)
                    raspberryItemObject.SetActive(true);
                    CraftRaspberryLemonadeButton.SetActive(true);
                if (raspberryText != null)
                    raspberryText.gameObject.SetActive(true);
                    CraftRaspberryLemonadeButton.SetActive(true);//unhides raspberry lemonade crafting
                    raspberryLemonadeText.gameObject.SetActive(true); //unhide raspberry lemonade text

                ingredientDropDown1 = GameObject.Find("ingredientDropDown1")?.GetComponent<TMP_Dropdown>();
                ingredientDropDown2 = GameObject.Find("ingredientDropDown2")?.GetComponent<TMP_Dropdown>();
                //Add the raspberry option to the dropdowns
                ingredientDropDown1.options.Add(new TMP_Dropdown.OptionData("Raspberry"));
                ingredientDropDown2.options.Add(new TMP_Dropdown.OptionData("Raspberry"));
                //Refresh the dropdowns so the UI updates
                ingredientDropDown1.RefreshShownValue();
                ingredientDropDown2.RefreshShownValue();


            }
            else
            {
                if (raspberryItemObject != null)
                    raspberryItemObject.SetActive(false);
                    CraftRaspberryLemonadeButton.SetActive(false);
                if (raspberryText != null)
                    raspberryText.gameObject.SetActive(false);
                    CraftRaspberryLemonadeButton.SetActive(false);//hides rasberry lemoneade crafting
                    raspberryLemonadeText.gameObject.SetActive(false); //hide raspberry lemonade text

            }

            // Show strawberrys on unlockable day
            if (day >= strawberryUnlockDay)
            {
                if (strawberryItemObject != null)
                    strawberryItemObject.SetActive(true);
                if (strawberryText != null)
                    strawberryText.gameObject.SetActive(true);
                

                ingredientDropDown1 = GameObject.Find("ingredientDropDown1")?.GetComponent<TMP_Dropdown>();
                ingredientDropDown2 = GameObject.Find("ingredientDropDown2")?.GetComponent<TMP_Dropdown>();
                //Add the strawberry option to the dropdowns
                ingredientDropDown1.options.Add(new TMP_Dropdown.OptionData("Strawberry"));
                ingredientDropDown2.options.Add(new TMP_Dropdown.OptionData("Strawberry"));
                //Refresh the dropdowns so the UI updates
                ingredientDropDown1.RefreshShownValue();
                ingredientDropDown2.RefreshShownValue();
            }
            else
            {
                if (strawberryItemObject != null)
                    strawberryItemObject.SetActive(false);
                if (strawberryText != null)
                    strawberryText.gameObject.SetActive(false);
            }

            UpdateUI(); // Refresh the displayed money amount
            UpdateInventoryText(); // Refresh inventory display 
            UpdateDropdownMenu(null,3); // Add items to the dropdown menu

                   
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
        strawberryText = GameObject.Find("strawberryText")?.GetComponent<TextMeshProUGUI>();
        raspberryLemonadeText = GameObject.Find("raspberryLemonadeText")?.GetComponent<TextMeshProUGUI>();
    
        errorPopup = GameObject.Find("ErrorPopup")?.gameObject;
        errorText  = GameObject.Find("ErrorText")?.GetComponent<TextMeshProUGUI>();
        errorOkButton= GameObject.Find("OkButton")?.GetComponent<Button>();


        //Below we find the star images and assign them to the variables
        starOne = GameObject.Find("StarOne").GetComponent<Image>();
        starTwo = GameObject.Find("StarTwo").GetComponent<Image>();
        starThree = GameObject.Find("StarThree").GetComponent<Image>();
        starFour = GameObject.Find("StarFour").GetComponent<Image>();
        starFive = GameObject.Find("StarFive").GetComponent<Image>();

        // Timer bar assignment
        timerBar = GameObject.Find("LinearTimerHolder");

        // Order Panel assignment
        selectWhatToSell = GameObject.Find("currentInventory")?.GetComponent<TMP_Dropdown>();

        //remove options two and three from the dropdown
        if (selectWhatToSell != null)
        {
            selectWhatToSell.options.RemoveAt(2);
            selectWhatToSell.options.RemoveAt(1);
            selectWhatToSell.RefreshShownValue();
        }
        else
        {
            Debug.LogError("SelectWhatToSell dropdown not found!");
        }


        // PopUps
        moneyPopUp = GameObject.Find("MoneyPopUp");
        starPopUp = GameObject.Find("StarPopUp");
        if(moneyPopUp != null) {
            moneyPopUp.SetActive(false);
        } else {
            Debug.LogError("Money Popup null!");
        }
        if(starPopUp != null) {
            starPopUp.SetActive(false);
        } else {
            Debug.LogError("Star Popup null!");
        }
        
        //Find and reconnect buttons
        AssignBuyButton("BuyLemonButton", "Lemon");
        AssignBuyButton("BuySugarButton", "Sugar");
        AssignBuyButton("BuyTeaButton", "Tea");
        AssignBuyButton("BuyGrapesButton", "Grapes");
        AssignBuyButton("BuyRaspberryButton", "Raspberries");
        AssignBuyButton("BuyStrawberryButton", "Strawberries");
        AssignRaspberryLemonadeCraftButton("CraftRaspberryLemonadeButton");
        AssignLemonadeCraftButton("CraftLemonadeButton");
        AssignSellButton("ServeButton");
        AssignCraftButton("craftButton");
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

    void AssignCraftButton(string buttonName)
    {
        Button button = GameObject.Find(buttonName)?.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners(); //Prevent duplicate listeners
            button.onClick.AddListener(() => CustomCraft()); //Reassign listener
        }
    }

    // Sets up "Serve" button aka Selling.
    void AssignSellButton(string buttonName)
    {
        Button button = GameObject.Find(buttonName)?.GetComponent<Button>();

        // Debug.Log("Selected option: " + selectedOption);
        if (button != null)
        {
            button.onClick.RemoveAllListeners(); //Prevent duplicate listeners
            button.onClick.AddListener(() => SellItem()); //Reassign listener and pass item
        }
    }

    void AssignLemonadeCraftButton(string buttonName){
        Button button = GameObject.Find(buttonName)?.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners(); //Prevent duplicate listeners
            button.onClick.AddListener(() => CraftLemonade()); //Reassign listener
        }
    }

    void AssignRaspberryLemonadeCraftButton(string buttonName)
    {
        Button button = GameObject.Find(buttonName)?.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners(); //Prevent duplicate listeners
            button.onClick.AddListener(() => CraftRaspberryLemonade()); //Reassign listener
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

    private int getCustomerAmount(){
        return Mathf.CeilToInt(3+(day*(rating/100f)));
    }
    
    private void EndDay(){
        EODBalance.Add(totalMoney);
        day++;//increments the day
        dayText.text = "Day: " + day; // Update the day text in the UI
        Debug.Log("It's now day " + day); 
        SceneManager.LoadScene("EndDayGraphScene");
    }

    //Function called when button clicked to reset DaysTransactionsBalance array and adds current money to the list
    public void ResetDaysTransactions()
    {
        
        DaysTransactionsBalance.Clear(); // Clear the list
        DaysTransactionsBalance.Add(totalMoney); // Add the current money to the list
        Debug.Log("Total money for the day: " + string.Join(", ", DaysTransactionsBalance));
    }

    // Show the popup
    void ShowError(string message)
    {
        errorText.text = message;
        errorPopup.SetActive(true);
    }

    void CustomCraft()
    {   
        firstIngredientInput = GameObject.Find("firstIngredientInput")?.GetComponent<TMP_InputField>(); //Connect all fields
        secondIngredientInput = GameObject.Find("secondIngredientInput")?.GetComponent<TMP_InputField>();
        ingredientDropDown1 = GameObject.Find("ingredientDropDown1")?.GetComponent<TMP_Dropdown>();
        ingredientDropDown2 = GameObject.Find("ingredientDropDown2")?.GetComponent<TMP_Dropdown>();
        string ingredient1 = ingredientDropDown1.options[ingredientDropDown1.value].text;
        string ingredient2 = ingredientDropDown2.options[ingredientDropDown2.value].text;

        int amount1;
        int amount2;

        //Return if ingredients are the same
        if (ingredient1 == ingredient2)
        {
            ShowError("Cannot use the same ingredient twice.");
            return;
        }

        if (!int.TryParse(firstIngredientInput.text, out amount1) || amount1 < 0) //Check if the input is a valid number and greater than 0
        {
            ShowError("Invalid amount for Ingredient 1.");
            return;
        }

        if (!int.TryParse(secondIngredientInput.text, out amount2) || amount2 < 0) //Check if the input is a valid number and greater than 0
        {
            ShowError("Invalid amount for Ingredient 2.");
            return;
        }

        if (inventory[ingredient1] < amount1 || inventory[ingredient2] < amount2) //Check if the player has enough ingredients
        {
            ShowError($"You do not have enough {ingredient1} or {ingredient2}.");
            return;
        }

        //Create a dictionary to store the selected ingredients and their amounts
        Dictionary<string, int> selectedIngredients = new Dictionary<string, int>
        {
            { ingredient1, amount1 },
            { ingredient2, amount2 }
        };

        //Check if the selected ingredients match a valid recipe
        if (selectedIngredients.ContainsKey("Lemon") && selectedIngredients["Lemon"] == 2 &&
            selectedIngredients.ContainsKey("Sugar") && selectedIngredients["Sugar"] == 1)
        {
            CraftLemonade();
        } else if (selectedIngredients.ContainsKey("Raspberries") && selectedIngredients["Raspberries"] == 2 &&
            selectedIngredients.ContainsKey("Sugar") && selectedIngredients["Sugar"] == 2)
        {
            CraftRaspberryLemonade();
        }
        else
        { //Invalid recipe, remove ingredients from inventory
            ShowError("Invalid crafting recipe. Ingredients were wasted.");
            inventory[ingredient1] -= amount1;
            inventory[ingredient2] -= amount2;

            wastedItemsCrafting = true; //Set the flag to true
        }

        UpdateInventoryText(); //Refresh inventory UI
        Debug.Log($"Crafted using {amount1} {ingredient1} and {amount2} {ingredient2}!");
    }

    //Function does math to check if the customer will purchase the product based on price
    bool willCustomerPurchase(float currentPrice, float basePrice)
    {
        float resistance = 0.6f; //resistance to price changes, higher means less likely to buy
        float priceFactor = currentPrice / basePrice;
        float buyChance = Mathf.Clamp01(1f / Mathf.Pow(priceFactor, resistance));
        return UnityEngine.Random.value <= buyChance;
    }
}