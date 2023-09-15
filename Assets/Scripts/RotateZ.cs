using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateZ : MonoBehaviour
{
    public float rotationSpeed = 15f; // Degrees per second

    // Update is called once per frame
    void Update()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;  // Calculate the amount of rotation for this frame
        transform.Rotate(0, 0, rotationAmount);
    }
}
