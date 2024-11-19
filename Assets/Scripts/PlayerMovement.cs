using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float gravity;
    public Vector2 velocity;
    public float maxXVelocity = 100;
    public float maxAcceleration = 10;
    public float acceleration = 10;
    public float distance = 0;
    public float groundHeight = 10;
    public float jumpVelocity = 20;
    public bool isGrounded = false;
    private bool isBubbleActive = false;
    private bool isBubbleOnCooldown = false;
    public Button bubbleButton;
    public GameObject bubbleButtonGO;

    public bool isHoldingJump = false;
    public float maxHoldJumpTime = 0.4f;
    public float maxFRHoldTime = 0.4f;
    public float holdJumpTimer = 0.0f;

    public float jumpGroundThreshold = 0.5f;

    public bool isDead = false;
    public bool controlsEnabled = true; // New variable to manage controls

    public TextMeshProUGUI trickText;
    public TextMeshProUGUI obstacleText;

    public LayerMask groundLayerMask;
    public LayerMask obstacleLayerMask;
    public Animator anim;
    public UIController uiController;
    public AudioManager audioManager;

    public GameObject bubble;

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private float minSwipeDistance = 50f;

    // Start is called before the first frame update
    void Start()
    {
        uiController = GameObject.Find("UIController").GetComponent<UIController>();

        if (uiController == null)
        {
            Debug.LogError("UIController not found. Make sure the object is named correctly and the script is attached.");
        }

        bubbleButtonGO.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Only process input if controls are enabled
        if (!controlsEnabled) return;

        Vector2 pos = transform.position;
        float groundDistance = Mathf.Abs(pos.y - groundHeight);

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Handle jump on tap immediately
            if (touch.phase == TouchPhase.Began)
            {
                if (isGrounded)
                {
                    Jump();  // Trigger the jump as soon as touch starts
                }
                else
                {
                    touchStartPos = touch.position;  // Store for potential trick detection if in-air
                }
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended)
            {
                touchEndPos = touch.position;
                HandleTouch(touchStartPos, touchEndPos);  // Handle trick input on swipe
            }
        }

        // Keyboard Inputs
        if (isGrounded || groundDistance <= jumpGroundThreshold)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
                
            }
        }

        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            trickText.gameObject.SetActive(false);
        }

        HandleTrickInput();
    }

    private void HandleTrickInput()
    {
        if (uiController != null && uiController.canDoTricks)
        {
            if (!isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    PerformTrick("+1", "D");
                    anim.SetTrigger("Trick2");
                    audioManager.PlaySFX(audioManager.Trick);
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    PerformTrick("+1", "A");
                    anim.SetTrigger("Trick2");
                    audioManager.PlaySFX(audioManager.Trick);
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    PerformTrick("+1", "W");
                    anim.SetTrigger("Trick");
                    audioManager.PlaySFX(audioManager.Trick);
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    PerformTrick("+1", "S");
                    anim.SetTrigger("Trick");
                    audioManager.PlaySFX(audioManager.Trick);
                }
            }

            // Deactivate trick text when keys are released
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
            {
                trickText.gameObject.SetActive(false);
            }
        }
    }

    private void Jump()
    {
        isGrounded = false;
        velocity.y = jumpVelocity;
        isHoldingJump = true;
        holdJumpTimer = 0;
        audioManager.PlaySFX(audioManager.Jump);
    }

    private void HandleTouch(Vector2 start, Vector2 end)
    {
        Vector2 swipeDirection = end - start;
        float swipeDistance = swipeDirection.magnitude;

        if (swipeDistance < minSwipeDistance)
        {
            // It's a tap, so trigger a jump
            if (isGrounded)
            {
                Jump();
            }
        }
        else if (!isGrounded && uiController != null && uiController.canDoTricks)
        {
            // It's a swipe, determine direction
            swipeDirection.Normalize();  // Normalize to get just the direction

            if (Vector2.Dot(swipeDirection, Vector2.up) > 0.7f)
            {
                PerformTrick("+1", "W");
                anim.SetTrigger("Trick");
            }
            else if (Vector2.Dot(swipeDirection, Vector2.down) > 0.7f)
            {
                PerformTrick("+1", "S");
                anim.SetTrigger("Trick");
            }
            else if (Vector2.Dot(swipeDirection, Vector2.left) > 0.7f)
            {
                PerformTrick("+1", "A");
                anim.SetTrigger("Trick2");
            }
            else if (Vector2.Dot(swipeDirection, Vector2.right) > 0.7f)
            {
                PerformTrick("+1", "D");
                anim.SetTrigger("Trick2");
            }
        }
    }

    private void PerformTrick(string trickName, string key)
    {
        trickText.gameObject.SetActive(true);
        trickText.text = trickName;
        uiController.GainSubscribers(1); // Gain subscribers for tricks
        Debug.Log($"{trickName} Performed - Simulating {key} key press");
    }

    // Increment the player's position every frame and also adjust the difference between frames
    private void FixedUpdate()
    {
        Vector2 pos = transform.position;

        if (isDead)
        {
            return;
        }

        if (pos.y < -20)
        {
            isDead = true;
        }

        if (!isGrounded)
        {
            if (isHoldingJump)
            {
                holdJumpTimer += Time.fixedDeltaTime;
                if (holdJumpTimer >= maxHoldJumpTime)
                {
                    isHoldingJump = false;
                }
            }

            pos.y += velocity.y * Time.fixedDeltaTime;
            if (!isHoldingJump)
            {
                velocity.y += gravity * Time.fixedDeltaTime;
            }

            Vector2 rayOrigin = new Vector2(pos.x + 0.7f, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, groundLayerMask);
            if (hit2D.collider != null)
            {
                Ground ground = hit2D.collider.GetComponent<Ground>();
                if (ground != null)
                {
                    if (pos.y >= ground.groundHeight)
                    {
                        groundHeight = ground.groundHeight;
                        pos.y = groundHeight;
                        isGrounded = true;
                    }
                }
            }
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);

            Vector2 wallOrigin = new Vector2(pos.x, pos.y);
            RaycastHit2D wallHit = Physics2D.Raycast(wallOrigin, Vector2.right, velocity.x * Time.fixedDeltaTime, groundLayerMask);
            if (wallHit.collider != null)
            {
                Ground ground = wallHit.collider.GetComponent<Ground>();
                if (ground != null)
                {
                    if (pos.y < ground.groundHeight)
                    {
                        velocity.x = 0;
                    }
                }
            }
        }

        distance += velocity.x * Time.fixedDeltaTime;

        if (isGrounded)
        {
            float velocityRatio = velocity.x / maxXVelocity;
            acceleration = maxAcceleration * (1 - velocityRatio);
            maxHoldJumpTime = maxFRHoldTime * velocityRatio;

            velocity.x += acceleration * Time.fixedDeltaTime;
            if (velocity.x >= maxXVelocity)
            {
                velocity.x = maxXVelocity;
            }

            Vector2 rayOrigin = new Vector2(pos.x - 0.7f, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
            if (hit2D.collider == null)
            {
                isGrounded = false;
            }
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.yellow);
        }

        Vector2 obstOrigin = new Vector2(pos.x, pos.y);
        RaycastHit2D obstHitX = Physics2D.Raycast(obstOrigin, Vector2.right, velocity.x * Time.fixedDeltaTime, obstacleLayerMask);
        if (obstHitX.collider != null)
        {
            Obstacle obstacle = obstHitX.collider.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                hitObstacle(obstacle);
            }
        }

        transform.position = pos;
    }

    void hitObstacle(Obstacle obstacle)
    {

        Destroy(obstacle.gameObject);

        if (!isBubbleActive) // Check if the bubble is inactive
        {
            velocity.x *= 0.7f; // Reduce velocity
            StartCoroutine(HideObstacleText());
            obstacleText.gameObject.SetActive(true);
            audioManager.PlaySFX(audioManager.obstacleHit);
            uiController.LoseSubscribers(1); // Lose subscribers
        }
        else
        {
            Debug.Log("No Damage done");
        }

    }

    public void ActivateBubble()
    {
        if (uiController.canUseBubble)
        {
            bubbleButtonGO.SetActive(true);
            if (!isBubbleActive)
            {
                bubble.SetActive(true); // Ensure the bubble GameObject is active
                isBubbleActive = true; // Set the flag
                StartCoroutine(TurnOffBubble());
                StartCoroutine(StartBubbleCooldown());
            }
            else if (isBubbleOnCooldown)
            {
                Debug.Log("Bubble is already active. Button press ignored.");
            }
        }
        else
        {
            bubbleButtonGO.SetActive(false);
        }
        

    }

    private IEnumerator StartBubbleCooldown()
    {
        isBubbleOnCooldown = true;
        bubbleButton.interactable = false; // Disable the button
        Debug.Log("Bubble cooldown started.");
        yield return new WaitForSeconds(30f);
        isBubbleOnCooldown = false;
        bubbleButton.interactable = true; // Re-enable the button
        Debug.Log("Bubble cooldown ended. Button is ready.");
    }

    IEnumerator HideObstacleText()
    {
        yield return new WaitForSeconds(.5f);
        obstacleText.gameObject.SetActive(false);
    }

    IEnumerator TurnOffBubble()
    {
        yield return new WaitForSeconds(5f);
        bubble.SetActive(false);
        isBubbleActive = false;
    }

    // Function to enable or disable player controls
    public void SetControlsEnabled(bool enabled)
    {
        controlsEnabled = enabled;
    }
}