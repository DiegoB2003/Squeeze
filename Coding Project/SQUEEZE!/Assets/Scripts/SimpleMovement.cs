using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class SimpleMovement : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    public float maxDistance = 10f; // Distance before stopping
    public Button TakeOrder; // Reference to the UI Button
    public GameObject orderPanel;
    public GameObject timerBar; // Prefab for the order panel
    public Image timer_linear_image; // Prefab for the order panel
    private float timer_remaining; // Remaining time for the order
    public float max_time = 10f; // Total time for the order
    public Sprite greenSprite; // Default green sprite
    public Sprite orangeSprite; // Orange sprite for 6-3 seconds
    public Sprite redSprite; // Red sprite for 3 seconds or less
    public GameObject TookTooLongMessage; // Text to show when time runs out
    public GameObject TimerTitle; // Prefab for the order panel

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private bool isMoving = false;
    private bool orderSequenceStarted = false;

    public GameObject targetObject;
    private Rigidbody2D targetRb;
    private Animator targetAnimator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        if (targetObject != null)
        {
            targetRb = targetObject.GetComponent<Rigidbody2D>();
            targetAnimator = targetObject.GetComponent<Animator>();

            if (targetRb != null)
            {
                targetRb.gravityScale = 0;
                targetRb.freezeRotation = true;
            }
        }

        startPosition = transform.position; // Store initial position

        if (TakeOrder != null)
        {
            TakeOrder.gameObject.SetActive(false); // Hide button at the start
            if (orderPanel != null) orderPanel.SetActive(false); // Hide order panel at start
            if (timerBar != null) timerBar.SetActive(false); // Hide timer bar at start
            if (TimerTitle != null) TimerTitle.SetActive(false); // Hide timer title at start
            
  

            // Hide the "took too long" message
            if (TookTooLongMessage != null)
            {
                TookTooLongMessage.SetActive(false); // Hide at start
                // TextMeshProUGUI messageText = TookTooLongMessage.GetComponentInChildren<TextMeshProUGUI>();
                // if (messageText != null)
                // {
                //     messageText.color = Color.red; // Set red color
                // }
                // else
                // {
                //     Debug.LogError("TextMeshProUGUI not found in TookTooLongMessage.");
                // }
            }

            TakeOrder.onClick.AddListener(HideButtonShowOrder); // Attach function to button click
        }

        StartMovement(); // Start movement when the script is first initialized
    }

    public void StartMovement()
    {
        rb.linearVelocity = new Vector2(speed, 0);
        startPosition = transform.position; // Reset start position
        isMoving = true;
        orderSequenceStarted = false; // Reset the order sequence flag

        // Hide take order button and timer bar when starting movement
        if (TakeOrder != null) TakeOrder.gameObject.SetActive(false);
        if (timerBar != null) timerBar.SetActive(false);
        if (targetAnimator != null) targetAnimator.SetBool("isMoving", true);
    }

    void FixedUpdate()
    {
        if (!isMoving) return; // Don't check distance if not moving

        float distanceTraveled = Vector2.Distance(startPosition, transform.position);

        if (distanceTraveled >= maxDistance)
        {
            rb.linearVelocity = Vector2.zero; // Stop moving
            isMoving = false;
            if (targetAnimator != null) targetAnimator.SetBool("isMoving", false);

            // Start the order sequence only once when the character stops
            if (!orderSequenceStarted)
            {
                orderSequenceStarted = true;
                if (timerBar != null)
                {
                    timerBar.SetActive(true); // Show the timer bar
                    TimerTitle.SetActive(true); // Show timer title
                    timer_remaining = max_time; // Reset the timer
                }

                if (TakeOrder != null)
                {
                    TakeOrder.gameObject.SetActive(true); // Show the button
                    GameObject.Find("GameManager")?.GetComponent<ShopManager>().setStartTime();
                }
            }
        }
    }

    void Update()
    {
        // Update the timer and UI only if the timer bar is active and we are not moving
        if (timerBar != null && timerBar.activeSelf && !isMoving)
        {
            if (timer_remaining > 0)
            {
                timer_remaining -= Time.deltaTime; // Decrease remaining time
                if (timer_linear_image != null)
                {
                    float fillAmount = timer_remaining / max_time;
                    // Debug.Log($"Fill Amount: {fillAmount}");
                    timer_linear_image.fillAmount = fillAmount;

                    // Change the timer bar image based on the remaining time
                    if (timer_remaining <= 6f && timer_remaining > 3f)
                    {
                        timer_linear_image.sprite = orangeSprite;
                    }
                    else if (timer_remaining <= 3f)
                    {
                        timer_linear_image.sprite = redSprite;
                    }
                    else
                    {
                        timer_linear_image.sprite = greenSprite;
                    }
                }
            }
            else
            {
                timerBar.SetActive(false); // Hide the timer bar when time runs out
                //hide the timer title
                TimerTitle.SetActive(false); // Hide timer title when time runs out

                if (TookTooLongMessage != null)
                {
                    TookTooLongMessage.SetActive(true); // Show took too long message
                    ShopManager.Instance.servedLate = true; //Set the servedLate boolean to true since taken to long
                }
            }
        }
    }

    void HideButtonShowOrder()
    {
        if (TakeOrder != null)
        {
            TakeOrder.gameObject.SetActive(false); // Hide button when clicked
            if (orderPanel != null) orderPanel.SetActive(true); // Show order panel
        }
    }
}
