using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    PlayerMovement player;
    UIController uiController;
    public float groundHeight;
    public float groundRight;
    public float screenRight;
    BoxCollider2D collider;

    bool didGenerateGround = false;

    public Obstacle boxTemplate;
    public Obstacle sewerTemplate;

    private int sewerObstacleCount = 0;
    public int maxSewerObstacles = 1;

    public Obstacle skyObstacleTemplate; // Add a reference for the sky obstacle template
    private float minSkyY = 10.0f; // Minimum Y coordinate for sky obstacle
    private float maxSkyY = 30.0f; // Maximum Y coordinate for sky obstacle // Maximum height above ground 
    public float skyObstacleSpawnChance = 0.3f; // 30% chance to spawn a sky obstacle

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        uiController = GameObject.Find("UIController").GetComponent<UIController>();
        collider = GetComponent<BoxCollider2D> ();
        groundHeight = transform.position.y + (collider.size.y / 2);
        screenRight = Camera.main.transform.position.x * 2;
        if(player == null || boxTemplate == null)
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

        if(groundRight < 0)
        {
            Destroy(gameObject);
            return;
        }

        groundRight = transform.position.x + (collider.size.x / 2);
        if(!didGenerateGround)
        {
            if (groundRight < screenRight)
            {
                didGenerateGround = true;
                GenerateGround();
            }
        }

        transform.position = pos;
        
    }

    void GenerateGround()
    {
        GameObject go = Instantiate(gameObject);
        BoxCollider2D goCollider = go.GetComponent<BoxCollider2D>();
        Vector2 pos;

        // Calculate the player's maximum jump height
        float h1 = player.jumpVelocity * player.maxHoldJumpTime;
        float t = player.jumpVelocity / -player.gravity;
        float h2 = player.jumpVelocity * t + (0.5f * (player.gravity * (t * t)));
        float maxJumpHeight = h1 + h2;

        // Limit the Y position to ensure it�s within a jumpable range
        float maxY = maxJumpHeight * 0.3f;
        maxY += groundHeight;
        float minY = 1;
        float actualY = Random.Range(minY, maxY);

        pos.y = actualY - goCollider.size.y / 2;

        // Ensure the ground is not placed too high
        if (pos.y > 2.7f)
            pos.y = 2.7f;

        // Calculate the player's max horizontal distance during a jump
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

        Ground goGround = go.GetComponent<Ground>();
        goGround.groundHeight = go.transform.position.y + (goCollider.size.y / 2);

        // Generate obstacles
        int obstacleNum = Random.Range(0, 4);
        bool sewerGenerated = false;


        for (int i = 0; i < obstacleNum; i++)
        {
         
         Obstacle chosenTemplate;
            
            // Choose the template based on the sewer count
            if (uiController != null && uiController.canSpawnSewer && !sewerGenerated && Random.Range(0, 2) == 0)
            {
                chosenTemplate = sewerTemplate;
                sewerGenerated = true; // Increment if a sewer is chosen
            }
            else
            {
                chosenTemplate = boxTemplate;
            }

            GameObject obstacle = Instantiate(chosenTemplate.gameObject);
            float y = goGround.groundHeight;
            float halfwidth = goCollider.size.x / 2 - 5;
            float left = go.transform.position.x - halfwidth;
            float right = go.transform.position.x + halfwidth;
            float x = Random.Range(left, right);
            Vector2 obstaclePos = new Vector2(x, y);
            obstacle.transform.position = obstaclePos;

            if (chosenTemplate == sewerTemplate)
            {
            sewerObstacleCount++;
            StartCoroutine(RaiseObstacle(obstacle, obstaclePos, goGround.groundHeight));
            }
        }

        // Generate sky obstacle
        if (uiController != null && uiController.canSpawnPigeon)
        {
            if (skyObstacleTemplate != null && Random.value < skyObstacleSpawnChance)
            {
                GameObject skyObstacle = Instantiate(skyObstacleTemplate.gameObject);

                // Define X position within ground boundaries
                float skyX = Random.Range(go.transform.position.x - goCollider.size.x / 2, go.transform.position.x + goCollider.size.x / 2);

                // Set the Y position to be within the specified range
                float skyY = Random.Range(minSkyY, maxSkyY); // Random Y coordinate between defined min and max

                // Debugging: Log the Y position
                Debug.Log($"Y Position for Sky Obstacle: {skyY}");

                // Set final position for sky obstacle
                Vector2 skyObstaclePos = new Vector2(skyX, skyY);
                skyObstacle.transform.position = skyObstaclePos;
            }
        }

    }

    IEnumerator RaiseObstacle(GameObject obstacle, Vector2 startPos, float targetHeight) //method to raise obstacle
        {
           
        
        Vector2 belowGroundPos = new Vector2(startPos.x, startPos.y - 2.0f);
        obstacle.transform.position = belowGroundPos;

        yield return new WaitForSeconds(0.1f); 

        Debug.Log("Target height for obstacle: " + targetHeight);

        float raiseSpeed = 2f; 
        while (obstacle.transform.position.y < targetHeight)
        {
            obstacle.transform.position = new Vector2(obstacle.transform.position.x, 
                Mathf.MoveTowards(obstacle.transform.position.y, targetHeight, raiseSpeed * Time.deltaTime));
            
            
            Debug.Log("Current position: " + obstacle.transform.position);
            yield return null; 
        }

        obstacle.transform.position = new Vector2(obstacle.transform.position.x, targetHeight);
        Destroy(obstacle, 5f); // how long till obstacle is destroyed
        sewerObstacleCount--;
        Debug.Log("Obstacle raised to: " + obstacle.transform.position);
            
        }

     bool IsPlayerClose(Vector2 obstaclePos)
    {
         float distanceThreshold = 1f;
        float distance = Vector2.Distance(player.transform.position, obstaclePos);
        return distance < distanceThreshold;
    }
    
}
