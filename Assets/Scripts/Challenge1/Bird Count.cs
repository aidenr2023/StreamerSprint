using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BirdCount : MonoBehaviour
{
    public int birdCount;
    public TextMeshProUGUI birdText;
    public GameObject win; 

    
    void Start()
    {
        win.SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {
        birdText.text = birdCount.ToString(); 

        CheckWinCondition();
    }

    void CheckWinCondition()
    {
        if (birdCount == 10)
        {
            win.SetActive(true);
             
        }
    }
}
