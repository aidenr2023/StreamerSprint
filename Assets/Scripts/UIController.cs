using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    PlayerMovement player;
    Text distanceText;
    // Start is called before the first frame update
    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        distanceText = GameObject.Find("DistanceText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        int distance = Mathf.FloorToInt(player.distance);
        distanceText.text = distance + "m";
    }
}
