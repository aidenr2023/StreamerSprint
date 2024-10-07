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

    public bool isHoldingJump = false;
    public float maxHoldJumpTime = 0.4f;
    public float maxFRHoldTime = 0.4f;
    public float holdJumpTimer = 0.0f;

    public float jumpGroundThreshold = 0.5f;

    public bool isDead = false;

    public TextMeshProUGUI trickText;

    public LayerMask groundLayerMask;
    public LayerMask obstacleLayerMask;
    public Animator anim;
    public UIController uiController;


    // Start is called before the first frame update
    void Start()
    {
        uiController = GameObject.Find("UIController").GetComponent<UIController>();

        if (uiController == null)
        {
            Debug.LogError("UIController not found. Make sure the object is named correctly and the script is attached.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.position;
        float groundDistance = Mathf.Abs(pos.y - groundHeight);

        if (isGrounded || groundDistance <= jumpGroundThreshold)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isGrounded = false;
                velocity.y = jumpVelocity;
                isHoldingJump = true;
                holdJumpTimer = 0;
            }
        }
        if (!isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("Left Trick Performed - Attempting to Gain Subscribers");
                trickText.gameObject.SetActive(true);
                trickText.text = "Right Trick";
                anim.SetTrigger("Trick");
                uiController.GainSubscribers(1); // Gain 1 subscribers for this skill move
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                trickText.gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                trickText.gameObject.SetActive(true);
                trickText.text = "Left Trick";
                anim.SetTrigger("Trick");
                uiController.GainSubscribers(1); // Gain 1 subscribers for this skill move
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                trickText.gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                trickText.gameObject.SetActive(true);
                trickText.text = "Up Trick";
                anim.SetTrigger("Trick");
                uiController.GainSubscribers(1); // Gain 1 subscribers for this skill move
            }

            if (Input.GetKeyUp(KeyCode.W))
            {
                trickText.gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                trickText.gameObject.SetActive(true);
                trickText.text = "Down Trick";
                anim.SetTrigger("Trick");
                uiController.GainSubscribers(1); // Gain 1 subscribers for this skill move
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                trickText.gameObject.SetActive(false);
            }
        }
        

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isHoldingJump = false;
        }
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
            if(isHoldingJump)
            {
                holdJumpTimer += Time.fixedDeltaTime;
                if(holdJumpTimer >= maxHoldJumpTime)
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
            if(hit2D.collider != null)
            {
                Ground ground = hit2D.collider.GetComponent<Ground>();
                if (ground != null) 
                {
                    if (pos.y >= ground.groundHeight){
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

        if(isGrounded)
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
            if (obstacle !=null)
            {
                hitObstacle(obstacle);
            }
        }


        transform.position = pos;
    }

    void hitObstacle(Obstacle obstacle)
    {
        Destroy(obstacle.gameObject);
        velocity.x *= 0.7f;
    }
}
