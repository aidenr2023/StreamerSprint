using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax1 : MonoBehaviour
{
    public float depth = 1;

    PlayerMovement1 player;

    public Sprite[] sprites; // Array to hold the sprites
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement1>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
    }

    void Start()
    {
        // Initial randomization of the sprite
        RandomizeSprite();
    }

    void FixedUpdate()
    {
        float realVelocity = player.velocity.x / depth;
        Vector2 pos = transform.position;

        pos.x -= realVelocity * Time.fixedDeltaTime;

        // Check if the position is out of bounds and reset if necessary
        if (pos.x <= -25)
        {
            pos.x = 80; // Reset the position
            RandomizeSprite(); // Randomize the sprite when resetting
        }

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