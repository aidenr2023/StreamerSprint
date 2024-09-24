using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    PlayerMovement player;
    Text distanceText;

    GameObject results;
    // Start is called before the first frame update
    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        distanceText = GameObject.Find("DistanceText").GetComponent<Text>();

        results = GameObject.Find("Results");
        results.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        int distance = Mathf.FloorToInt(player.distance);
        distanceText.text = distance + "m";

        if (player.isDead)
        {
            results.SetActive(true);

            Invoke("RestartScene", 3f); //For the purpose of the prototype, remove later
        }
    }

    void RestartScene()
    {
        SceneManager.LoadScene("EndlessPath");
    }
}
