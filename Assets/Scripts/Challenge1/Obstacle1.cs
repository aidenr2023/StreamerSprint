using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle1 : MonoBehaviour
{
   PlayerMovement1 player;
    
    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement1>();
    }

    
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;

        pos.x -= player.velocity.x * Time.fixedDeltaTime;
        if (pos.x < -100)
        {
            Destroy(gameObject);
        }
        
        transform.position = pos;
    }


}
