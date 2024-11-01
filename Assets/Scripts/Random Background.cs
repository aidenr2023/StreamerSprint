using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBackground : MonoBehaviour
{
    public GameObject background1;
    public GameObject background2;
    // Start is called before the first frame update
    private void Start()
    {
        int randomChoice = Random.Range(0, 2);

        if (randomChoice == 0)
        {
            background1.SetActive(true);
            background2.SetActive(false);
        }
        else
        {
            background1.SetActive(false);
            background2.SetActive(true);
        }
    }
}
