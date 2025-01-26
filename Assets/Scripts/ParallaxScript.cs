using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ParallaxScript : MonoBehaviour
{
    private float startPos;
    private float length;
    public GameObject cam;
    [Range(0,1)]
    public float parallaxSpeed;
    void Awake()
    {
        startPos = transform.position.x;
        cam = GameObject.FindWithTag("MainCamera");
        length = GetComponent<SpriteRenderer>().bounds.size.x;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = cam.transform.position.x * parallaxSpeed; // 0 = move with cam. 1 = will not move
        float movement = cam.transform.position.x * (1-parallaxSpeed);

        transform.position = new Vector3(startPos + distance, transform.position.y,transform.position.z);

        if(movement > startPos + length)
        {
            startPos += length;
        }
        else if(movement < startPos - length)
        {
            startPos -= length;
        }
    }
}
