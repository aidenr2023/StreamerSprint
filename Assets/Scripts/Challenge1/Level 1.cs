using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : MonoBehaviour
{
    PlayerMovement1 player;
    UIController1 uiController;
    public float groundHeight;
    public float groundRight;
    public float screenRight;
    BoxCollider2D collider;

    bool didGenerateGround = false;

    public Obstacle1 skyObstacleTemplate; // Sky obstacle template
    private float minSkyY = 10.0f; // Minimum Y coordinate for sky obstacle
    private float maxSkyY = 30.0f; // Maximum Y coordinate for sky obstacle
    public float skyObstacleSpawnChance = 1.0f; // 70% chance to spawn a sky obstacle

    public int maxSkyObstacles = 50; // Max number of sky obstacles to spawn per ground tile

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement1>();
        uiController = GameObject.Find("UIController").GetComponent<UIController1>();
        collider = GetComponent<BoxCollider2D>();
        groundHeight = transform.position.y + (collider.size.y / 2);
        screenRight = Camera.main.transform.position.x * 2;
        if (player == null || skyObstacleTemplate == null)
        {
            Debug.LogError("Required component missing");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
    }


    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;
        pos.x -= player.velocity.x * Time.fixedDeltaTime;

        if (groundRight < 0)
        {
            Destroy(gameObject);
            return;
        }

        groundRight = transform.position.x + (collider.size.x / 2);
        if (!didGenerateGround)
        {
            if (groundRight < screenRight)
            {
                didGenerateGround = true;
                GenerateGround();
            }
        }

        transform.position = pos;
    }

    // Generate the ground and spawn multiple sky obstacles along with it
    void GenerateGround()
    {
        GameObject go = Instantiate(gameObject);
        BoxCollider2D goCollider = go.GetComponent<BoxCollider2D>();
        Vector2 pos;

       
        float h1 = player.jumpVelocity * player.maxHoldJumpTime;
        float t = player.jumpVelocity / -player.gravity;
        float h2 = player.jumpVelocity * t + (0.5f * (player.gravity * (t * t)));
        float maxJumpHeight = h1 + h2;

        
        float maxY = maxJumpHeight * 0.3f;
        maxY += groundHeight;
        float minY = 1;
        float actualY = Random.Range(minY, maxY);

        pos.y = actualY - goCollider.size.y / 2;

        
        if (pos.y > 2.7f)
            pos.y = 2.7f;

      
        float t1 = t + player.maxHoldJumpTime;
        float t2 = Mathf.Sqrt((2.0f * (maxY - actualY)) / -player.gravity);
        float totalTime = t1 + t2;
        float maxX = totalTime * player.velocity.x;
        maxX *= 0.4f;
        maxX += groundRight;

        float minX = screenRight + 5;
        float actualX = Random.Range(minX, maxX);

        pos.x = actualX + goCollider.size.x / 2;
        go.transform.position = pos;

        Level1 goGround = go.GetComponent<Level1>();
        goGround.groundHeight = go.transform.position.y + (goCollider.size.y / 2);

        
        if (uiController != null && uiController.canSpawnPigeon)
        {
            if (skyObstacleTemplate != null)
            {
             
                int skyObstaclesToSpawn = Random.Range(20, maxSkyObstacles + 1); 

                for (int i = 0; i < skyObstaclesToSpawn; i++)
                {
                    if (Random.value < skyObstacleSpawnChance) // Use spawn chance
                    {
                        GameObject skyObstacle = Instantiate(skyObstacleTemplate.gameObject);

                        // Define X position within the ground boundaries
                        float skyX = Random.Range(go.transform.position.x - goCollider.size.x / 2, go.transform.position.x + goCollider.size.x / 2);

                        // Set the Y position to be within the specified range
                        float skyY = Random.Range(minSkyY, maxSkyY); // Random Y coordinate between defined min and max

                        // Debugging: Log the Y position
                        Debug.Log($"Sky Obstacle Y Position: {skyY}");

                        // Set final position for sky obstacle
                        Vector2 skyObstaclePos = new Vector2(skyX, skyY);
                        skyObstacle.transform.position = skyObstaclePos;

                        // Optionally, destroy the obstacle after some time (5 seconds here)
                        Destroy(skyObstacle, 5f); // Adjust lifetime if needed
                    }
                }
            }
        }
    }
}
