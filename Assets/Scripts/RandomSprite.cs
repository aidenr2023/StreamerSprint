using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Sprite[] sprites;

    private SpriteRenderer SpriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();

        AssignRandomSprite();
    }

    // Update is called once per frame
    void AssignRandomSprite()
    {
        if (sprites.Length == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, sprites.Length);

        SpriteRenderer.sprite = sprites[randomIndex];
    }

}
