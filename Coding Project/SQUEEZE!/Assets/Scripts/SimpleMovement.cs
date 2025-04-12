using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private bool isMoving = false;
    private bool orderSequenceStarted = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        startPosition = transform.position; // Store initial position

        if (TakeOrder != null)
        {
            TakeOrder.gameObject.SetActive(false); // Hide button at the start
            if (orderPanel != null) orderPanel.SetActive(false); // Hide order panel at start
            if (timerBar != null) timerBar.SetActive(false); // Hide timer bar at start

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
    }

    void FixedUpdate()
    {
        if (!isMoving) return; // Don't check distance if not moving

        float distanceTraveled = Vector2.Distance(startPosition, transform.position);

        if (distanceTraveled >= maxDistance)
        {
            rb.linearVelocity = Vector2.zero; // Stop moving
            isMoving = false;

            // Start the order sequence only once when the character stops
            if (!orderSequenceStarted)
            {
                orderSequenceStarted = true;
                if (timerBar != null)
                {
                    timerBar.SetActive(true); // Show the timer bar
                    timer_remaining = max_time; // Reset the timer
                    if (timer_linear_image != null)
                    {
                        timer_linear_image.fillAmount = 1f; // Initialize fill amount
                    }
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
                    timer_linear_image.fillAmount = timer_remaining / max_time; // Update the timer UI, clamping value
                }
            }
            else
            {
                timerBar.SetActive(false); // Hide the timer bar when time runs out
            }
        }

        // Debug logs can be helpful during development
        Debug.Log($"Order Panel Active: {(orderPanel != null ? orderPanel.activeSelf : false)}");
        Debug.Log($"Time Remaining: {timer_remaining}");
        Debug.Log($"Is Moving: {isMoving}");
        Debug.Log($"Order Sequence Started: {orderSequenceStarted}");
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