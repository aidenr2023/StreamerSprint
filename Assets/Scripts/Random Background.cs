using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBackground : MonoBehaviour
{
    public GameObject background1;
    public GameObject background2;
    public GameObject background3;

    // Start is called before the first frame update
    private void Start()
    {
        // Generate a random number between 0 and 2 (inclusive)
        int randomChoice = Random.Range(0, 3);

        // Deactivate all backgrounds initially
        background1.SetActive(false);
        background2.SetActive(false);
        background3.SetActive(false);

        // Activate the randomly chosen background
        switch (randomChoice)
        {
            case 0:
                background1.SetActive(true);
                break;
            case 1:
                background2.SetActive(true);
                break;
            case 2:
                background3.SetActive(true);
                break;
        }
    }
}
