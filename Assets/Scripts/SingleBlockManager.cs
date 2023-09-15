using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SingleBlockManager : MonoBehaviour
{
    public bool isCollected = false;
    // I don't want to rotation disabled each time, that's why I have created this boolean value
    private bool isCollectedChecked = false;
    public bool isIgnored { get; set; }
    

    [Header("Block Id")]
    public int blockId;
    [Header("Block Coordinates")]
    public float xAxisCor;
    public float yAxisCor;
    public float zAxisCor;

    private void FixedUpdate()
    {
        if (!isCollected)
        {
            rotateBlock();
        }
        if(isCollected)
        {
            
            gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        }
    }

    
    public void ropeCreated()
    {
        isCollected = false;
        isCollectedChecked = false;
    }
    
    
    private void rotateBlock()
    {
        gameObject.transform.Rotate(new Vector3(0f, 0.65f, 0f));
    }
}
