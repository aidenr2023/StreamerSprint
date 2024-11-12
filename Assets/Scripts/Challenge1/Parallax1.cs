using UnityEngine;

public class Parallax1 : MonoBehaviour
{
    public float depth = 1;

    PlayerMovement1 player;

    public Sprite[] sprites; // Array to hold the sprites
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer

    private float backgroundWidth; // Holds the background's width
    private Vector2 resetPosition; // To store the reset position for the background
    private Vector2 initialPosition; // Stores the initial starting position for smooth transition

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement1>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
    }

    void Start()
    {
        // Set the background width based on the sprite size
        backgroundWidth = spriteRenderer.bounds.size.x;

        // Initialize positions
        initialPosition = transform.position;  // Set the initial position of the background
        resetPosition = new Vector2(backgroundWidth, initialPosition.y);  // Reset position for the background

        // Initial randomization of the sprite
        RandomizeSprite();
    }

    void FixedUpdate()
    {
        // Calculate the velocity of the background based on player's movement and depth
        float realVelocity = player.velocity.x / depth;

        // Get the current position of the background
        Vector2 pos = transform.position;

        // Move the background based on velocity
        pos.x -= realVelocity * Time.fixedDeltaTime;

        // Reset the background position when it goes off-screen
        if (pos.x <= -backgroundWidth)  // Reset when the background is completely off-screen
        {
            pos.x = resetPosition.x;  // Reset to the new position

            // Randomize the sprite after resetting
            RandomizeSprite();
        }

        // Apply the new position to the background
        transform.position = pos;
    }

    private void RandomizeSprite()
    {
        // Ensure the sprites array is not empty
        if (sprites.Length > 0)
        {
            int randomIndex = Random.Range(0, sprites.Length);
            spriteRenderer.sprite = sprites[randomIndex]; // Assign a new random sprite
        }
        else
        {
            Debug.LogWarning("No sprites assigned to the Parallax script on " + gameObject.name);
        }
    }
}
