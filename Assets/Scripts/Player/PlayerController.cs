using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movespeed = 4f;
    [SerializeField] private float interactRange = 1.2f;
    [SerializeField] private LayerMask pushableLayer;
    [SerializeField] private GameObject pressSpaceText;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Vector2 lastMoveDirection = Vector2.right;

    private Rigidbody2D rb;
    private Rigidbody2D currentBox;

    private bool isHoldingBox = false;
    private Vector2 boxOffsetFromPlayer;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        PlayerInput();
        DetectPushableObject();
        HandleHoldInteraction();
    }

    private void FixedUpdate()
    {
        if (isHoldingBox && currentBox != null)
        {
            MovePlayerAndBox();
        }
        else
        {
            MovePlayerOnly();
        }
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        if (movement != Vector2.zero)
        {
            lastMoveDirection = movement.normalized;
        }
    }

    private void DetectPushableObject()
    {
        if (isHoldingBox)
        {
            pressSpaceText.SetActive(false);
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(
            rb.position,
            lastMoveDirection,
            interactRange,
            pushableLayer
        );

        if (hit.collider != null)
        {
            currentBox = hit.collider.GetComponent<Rigidbody2D>();
            pressSpaceText.SetActive(true);
        }
        else
        {
            currentBox = null;
            pressSpaceText.SetActive(false);
        }
    }

    private void HandleHoldInteraction()
    {
        bool spacePressed = playerControls.Movement.Interact.IsPressed();

        if (spacePressed && currentBox != null)
        {
            if (!isHoldingBox)
            {
                isHoldingBox = true;
                boxOffsetFromPlayer = currentBox.position - rb.position;
            }
        }
        else
        {
            isHoldingBox = false;
        }
    }

    private void MovePlayerOnly()
    {
        rb.MovePosition(rb.position + movement * movespeed * Time.fixedDeltaTime);
    }

    private void MovePlayerAndBox()
    {
        Vector2 playerTargetPosition = rb.position + movement * movespeed * Time.fixedDeltaTime;
        Vector2 boxTargetPosition = playerTargetPosition + boxOffsetFromPlayer;

        rb.MovePosition(playerTargetPosition);
        currentBox.MovePosition(boxTargetPosition);
    }
}