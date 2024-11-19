using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    private void Start()
    {
        // Ensure the game runs at normal speed
        Time.timeScale = 1f;
    }

    [SerializeField] private RawImage _img;
    [SerializeField] private float _x, _y;

    // Update is called once per frame
    void Update()
    {
        // Scroll the image by modifying the UV Rect
        _img.uvRect = new Rect(
            _img.uvRect.position + new Vector2(_x, _y) * Time.deltaTime,
            _img.uvRect.size
        );
    }
}
