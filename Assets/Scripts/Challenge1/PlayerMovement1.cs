using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public bool isHoldingJump = false;
    public float maxHoldJumpTime = 0.4f;
    public float holdJumpTimer = 0.0f;

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
    private float minSwipeDistance = 50f;

    void Start()
    {
        uiController = GameObject.Find("UIController").GetComponent<UIController1>();
        if (uiController == null)
        {
            Debug.LogError("UIController not found. Ensure the object is named correctly and the script is attached.");
        }
    }

    void Update()
    {
        if (!controlsEnabled) return;

        Vector2 pos = transform.position;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (isGrounded)
                {
                    Jump();
                }
                else
                {
                    touchStartPos = touch.position;
                }
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended)
            {
                HandleTouch(touchStartPos, touch.position);
            }
        }

        if (isGrounded || Mathf.Abs(pos.y - groundHeight) <= jumpGroundThreshold)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }

        HandleTrickInput();
    }

    private void HandleTrickInput()
    {
        if (uiController != null && uiController.canDoTricks && !isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.D))
                ExecuteTrick("+1", "D", "Trick2");
            if (Input.GetKeyDown(KeyCode.A))
                ExecuteTrick("+1", "A", "Trick2");
            if (Input.GetKeyDown(KeyCode.W))
                ExecuteTrick("+1", "W", "Trick");
            if (Input.GetKeyDown(KeyCode.S))
                ExecuteTrick("+1", "S", "Trick");
        }
    }

    private void ExecuteTrick(string trickName, string key, string animationTrigger)
    {
        trickText.gameObject.SetActive(true);
        trickText.text = trickName;
        uiController.GainSubscribers(1);
        anim.SetTrigger(animationTrigger);
        audioManager.PlaySFX(audioManager.Trick);
        Debug.Log($"{trickName} performed - Simulating {key} key press.");
    }

    private void Jump()
    {
        isGrounded = false;
        velocity.y = jumpVelocity;
        isHoldingJump = true;
        holdJumpTimer = 0f;
        audioManager.PlaySFX(audioManager.Jump);
    }

    private void HandleTouch(Vector2 start, Vector2 end)
    {
        Vector2 swipeDirection = end - start;
        float swipeDistance = swipeDirection.magnitude;

        if (swipeDistance < minSwipeDistance && isGrounded)
        {
            Jump();
        }
        else if (!isGrounded && uiController != null && uiController.canDoTricks)
        {
            swipeDirection.Normalize();

            if (Vector2.Dot(swipeDirection, Vector2.up) > 0.7f)
                ExecuteTrick("+1", "W", "Trick");
            else if (Vector2.Dot(swipeDirection, Vector2.down) > 0.7f)
                ExecuteTrick("+1", "S", "Trick");
            else if (Vector2.Dot(swipeDirection, Vector2.left) > 0.7f)
                ExecuteTrick("+1", "A", "Trick2");
            else if (Vector2.Dot(swipeDirection, Vector2.right) > 0.7f)
                ExecuteTrick("+1", "D", "Trick2");
        }
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;

        if (isDead) return;

        if (pos.y < -20)
        {
            isDead = true;
            return;
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

            CheckGroundCollision(ref pos);
        }

        distance += velocity.x * Time.fixedDeltaTime;

        if (isGrounded)
        {
            AdjustGroundMovement();
        }

        CheckObstacleCollision();
        transform.position = pos;
    }

    private void CheckGroundCollision(ref Vector2 pos)
    {
        Vector2 rayOrigin = new Vector2(pos.x + 0.7f, pos.y);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, velocity.y * Time.fixedDeltaTime, groundLayerMask);

        if (hit.collider != null)
        {
            Level1 ground = hit.collider.GetComponent<Level1>();
            if (ground != null && pos.y >= ground.groundHeight)
            {
                groundHeight = ground.groundHeight;
                pos.y = groundHeight;
                isGrounded = true;
            }
        }
        Debug.DrawRay(rayOrigin, Vector2.up * velocity.y * Time.fixedDeltaTime, Color.red);
    }

    private void AdjustGroundMovement()
    {
        float velocityRatio = velocity.x / maxXVelocity;
        acceleration = maxAcceleration * (1 - velocityRatio);
        maxHoldJumpTime = maxHoldJumpTime * velocityRatio;

        velocity.x += acceleration * Time.fixedDeltaTime;
        if (velocity.x >= maxXVelocity)
        {
            velocity.x = maxXVelocity;
        }

        Vector2 rayOrigin = new Vector2(transform.position.x - 0.7f, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, velocity.y * Time.fixedDeltaTime);

        if (hit.collider == null)
        {
            isGrounded = false;
        }
    }

    private void CheckObstacleCollision()
    {
        Vector2 rayOrigin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, velocity.x * Time.fixedDeltaTime, obstacleLayerMask);

        if (hit.collider != null)
        {
            Obstacle1 obstacle = hit.collider.GetComponent<Obstacle1>();
            if (obstacle != null)
            {
                HandleObstacleHit(obstacle);
            }
        }
    }

    private void HandleObstacleHit(Obstacle1 obstacle)
    {
        Destroy(obstacle.gameObject);
        obstacleText.gameObject.SetActive(true);
        audioManager.PlaySFX(audioManager.obstacleHit);
        StartCoroutine(HideObstacleText());
    }

    private IEnumerator HideObstacleText()
    {
        yield return new WaitForSeconds(0.5f);
        obstacleText.gameObject.SetActive(false);
    }

    public void SetControlsEnabled(bool enabled)
    {
        controlsEnabled = enabled;
    }
}