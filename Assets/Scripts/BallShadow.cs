using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShadow : MonoBehaviour
{
    public Transform target;
    private Vector2 startScale;
    public float minFloatX = .5f;
    public float minFloatY = .35f;
    public float scaleFallOff = 3f;


    private void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        // Smoothly move the camera towards that target position
        transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);

        // calculate scale and see if it is less than the min
        float xScale = Mathf.Max(startScale.x * (1 / (target.position.y / scaleFallOff + 1)), minFloatX);
        float yScale = Mathf.Max(startScale.y * (1 / (target.position.y / scaleFallOff + 1)), minFloatY);

        transform.localScale = new Vector2(xScale, yScale);
    }
} 
