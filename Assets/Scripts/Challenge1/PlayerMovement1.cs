using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement1 : MonoBehaviour
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
    private float touchStartTime;

    public bool isHoldingJump = false;
    public float maxHoldJumpTime = 0.4f;
    public float maxFRHoldTime = 0.4f;
    public float holdJumpTimer = 0.0f;
    public BirdCount bird;

    public float jumpGroundThreshold = 0.5f;

    public bool isDead = false;
    public bool controlsEnabled = true; 

    public TextMeshProUGUI trickText;
    public TextMeshProUGUI obstacleText;

    public LayerMask groundLayerMask;
    public LayerMask obstacleLayerMask;
    public Animator anim;
    public UIController1 uiController;
    public AudioManager audioManager;

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private float minSwipeDistance = 50f;
    private float maxTapTime = 0.2f; // Max time allowed for a tap

    // Start is called before the first frame update
    void Start()
    {
        uiController = GameObject.Find("UIController").GetComponent<UIController1>();

        if (uiController == null)
        {
            Debug.LogError("UIController not found. Make sure the object is named correctly and the script is attached.");
        }
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

            if (touch.phase == TouchPhase.Began)
            {
                touchStartTime = Time.time;
                touchStartPos = touch.position; // Record starting position of the touch
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                float touchDuration = Time.time - touchStartTime; // Calculate touch duration
                Vector2 touchEndPos = touch.position; // Record ending position of the touch
                Vector2 swipeDirection = touchEndPos - touchStartPos; // Calculate swipe direction
                float swipeDistance = swipeDirection.magnitude;

                // Handle as a tap if it's quick and has minimal movement
                if (touchDuration < maxTapTime && swipeDistance < minSwipeDistance)
                {
                    HandleTap(); // Process the tap action
                }
                else
                {
                    // Otherwise, handle as a swipe
                    HandleSwipe(touchStartPos, touchEndPos);
                }
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

    private void HandleTap()
    {
        if (isGrounded)
        {
            Jump(); // Trigger jump on a valid tap
        }
    }

    private void HandleSwipe(Vector2 start, Vector2 end)
    {
        Vector2 swipeDirection = end - start;
        float swipeDistance = swipeDirection.magnitude;

        // Prevent further processing if the swipe distance is below the threshold
        if (swipeDistance < minSwipeDistance) return;

        // Swipe logic (only when airborne and tricks are allowed)
        if (!isGrounded && uiController != null && uiController.canDoTricks)
        {
            swipeDirection.Normalize(); // Normalize the swipe direction

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
                    
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    PerformTrick("+1", "A");
                    anim.SetTrigger("Trick2");
                    
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    PerformTrick("+1", "W");
                    anim.SetTrigger("Trick");
                    
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    PerformTrick("+1", "S");
                    anim.SetTrigger("Trick");
                   
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

    private void PerformTrick(string trickName, string key)
    {
        trickText.gameObject.SetActive(true);
        trickText.text = trickName;
        uiController.GainSubscribers(1); // Gain subscribers for tricks
        audioManager.PlaySFX(audioManager.Trick);
        Debug.Log($"{trickName} Performed - Simulating {key} key press");

        StartCoroutine(HideTrickTextAfterDelay());
    }

    private IEnumerator HideTrickTextAfterDelay()
    {
        yield return new WaitForSeconds(0.2f); // Adjust the delay duration as needed
        trickText.gameObject.SetActive(false);
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
                Level1 ground = hit2D.collider.GetComponent<Level1>();
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
                Level1 ground = wallHit.collider.GetComponent<Level1>();
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
            Obstacle1 obstacle = obstHitX.collider.GetComponent<Obstacle1>();
            if (obstacle != null)
            {
                hitObstacle(obstacle);
            }
        }

        transform.position = pos;
    }

    void hitObstacle(Obstacle1 obstacle)
    {
        bird.birdCount++;
        Destroy(obstacle.gameObject);
        StartCoroutine(HideObstacleText());
        obstacleText.gameObject.SetActive(true);
        audioManager.PlaySFX(audioManager.obstacleHit);
        
    }

    IEnumerator HideObstacleText()
    {
        yield return new WaitForSeconds(.5f);
        obstacleText.gameObject.SetActive(false);
    }

    // Function to enable or disable player controls
    public void SetControlsEnabled(bool enabled)
    {
        controlsEnabled = enabled;
    }
}