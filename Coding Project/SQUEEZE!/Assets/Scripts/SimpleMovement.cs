using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class SimpleMovement : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    public float maxDistance = 10f; // Distance before stopping
    public Button TakeOrder; // Reference to the UI Button

    public GameObject orderPanel;

    private Rigidbody2D rb;
    private Vector2 startPosition;

    private bool isOrderButtonActivated = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        startPosition = transform.position; // Store initial position

        if (TakeOrder != null)
        {
            TakeOrder.gameObject.SetActive(false); // Hide button at the start
            orderPanel.SetActive(false); // Hide order panel at start
            TakeOrder.onClick.AddListener(HideButtonShowOrder); // Attach function to button click
        }

        StartMovement(); // Start movement when the script is first initialized
    }

    public void StartMovement()
    {
        rb.linearVelocity = new Vector2(speed, 0);
        startPosition = transform.position; // Reset start position
        isOrderButtonActivated = false; // Reset the button activation state

        // Hide take order button
        TakeOrder.gameObject.SetActive(false);
    
    }

    void FixedUpdate()
    {
        float distanceTraveled = Vector2.Distance(startPosition, transform.position);

        if (distanceTraveled >= maxDistance)
        {
            rb.linearVelocity = Vector2.zero; // Stop moving
            if (TakeOrder != null && !isOrderButtonActivated)
            {
                TakeOrder.gameObject.SetActive(true); // Show the button
                isOrderButtonActivated = true;
                GameObject.Find("GameManager")?.GetComponent<ShopManager>().setStartTime();
            }
        }
    }

    void HideButtonShowOrder()
    {
        if (TakeOrder != null)
        {
            TakeOrder.gameObject.SetActive(false); // Hide button when clicked
            orderPanel.SetActive(true); // Show order panel
        }
    }
}