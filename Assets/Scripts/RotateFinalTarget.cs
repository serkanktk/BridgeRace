using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RotateFinalTarget : MonoBehaviour
{
    public float rotationSpeed = 45f; // Speed of rotation in degrees per second
    DataManager dataManager;


    private void Start()
    {
        dataManager = FindObjectOfType<DataManager>();
    }



    void Update()
    {
        // Calculate the rotation amount based on the time passed since the last frame
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Rotate the object around the z-axis
        transform.Rotate(Vector3.forward, rotationAmount);
    }




    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "MainCharacter")
        {
            // Increment totalWinnings by one
            dataManager.totalWinnings += 1;
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
        }
    }


}
