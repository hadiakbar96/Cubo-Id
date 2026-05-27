using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movespeed = 4f;
    [SerializeField] private float interactRange = 1.2f;
    [SerializeField] private LayerMask pushableLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private GameObject pressSpaceText;
    [SerializeField] private Vector3 textOffset = new Vector3(0f, 1.5f, 0f); // Offset di atas objek

    private Camera mainCamera;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Vector2 lastMoveDirection = Vector2.right;
    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Rigidbody2D currentBox;
    private Collider2D currentBoxCollider;

    private bool isHoldingBox = false;

    private RaycastHit2D[] boxHits = new RaycastHit2D[5];

    [Header("Sprites for Directions")]
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
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
        updateSprite();
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
            currentBoxCollider = hit.collider;
            pressSpaceText.SetActive(true);

            // Posisikan teks di atas objek yang terdeteksi
            Vector3 worldPos = hit.collider.transform.position + textOffset;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
            pressSpaceText.transform.position = screenPos;
        }
        else
        {
            currentBox = null;
            currentBoxCollider = null;
            pressSpaceText.SetActive(false);
        }
    }

    private void HandleHoldInteraction()
    {
        bool spacePressed = playerControls.Movement.Interact.IsPressed();

        if (spacePressed && currentBox != null)
        {
            isHoldingBox = true;
        }
        else
        {
            isHoldingBox = false;
        }
    }

    private void MovePlayerOnly()
    {
        Vector2 delta = movement * movespeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + delta);
    }

    private void MovePlayerAndBox()
    {
        if (movement == Vector2.zero) return;

        Vector2 delta = movement * movespeed * Time.fixedDeltaTime;

        if (CanBoxMove(delta))
        {
            rb.MovePosition(rb.position + delta);
            currentBox.MovePosition(currentBox.position + delta);
        }
    }

    private bool CanBoxMove(Vector2 delta)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(obstacleLayer);
        filter.useTriggers = false;

        int hitCount = currentBoxCollider.Cast(
            delta.normalized,
            filter,
            boxHits,
            delta.magnitude
        );

        return hitCount == 0;
    }

    private void updateSprite()
    {
        if (movement.y > 0 && upSprite != null) spriteRenderer.sprite = upSprite;
        else if (movement.y < 0 && downSprite != null) spriteRenderer.sprite = downSprite;
        else if (movement.x > 0 && rightSprite != null) spriteRenderer.sprite = rightSprite;
        else if (movement.x < 0 && leftSprite != null) spriteRenderer.sprite = leftSprite;
        else if (downSprite != null) spriteRenderer.sprite = downSprite;
    }
}