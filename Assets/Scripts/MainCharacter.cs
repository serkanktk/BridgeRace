using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainCharacter : MonoBehaviour
{
    private Stack<GameObject> collectedBlocks;
    private Transform collectionPosition; // Initial position (I don't want to calculate each of them one by one everytime for GPU issues)
    Transform objectTransform;
    float yPosition;
    private SingleBlockManager blockManager;
    private int numberOfBlockCollected = 1;
    public int numberOfBlocksOnTheGround = 0;

    // for collection of objects on the ground
    private SphereCollider detectionCollider;
    // for the sake of collecting algorithm
    // the initial value of the number of blocks collected is setted to 1
    public float speed = 250f;
    Rigidbody rigid;
    
    // these are for camera movement
    private Vector3 cameraDifference;
    [Header("Camera and Velocity")]
    public GameObject cam;
    public float maxVelocity = 100.0f; // Define your maximum velocity here
    [Header("Materials Manager Script")]
    public MaterialManager materialManager; // Reference to the MaterialManager script

    void Start()
    {
        
        collectedBlocks = new Stack<GameObject>(); // initialize stack object
        // initialize the first collection position
        // Objects in ground will be collected above the character
        collectionPosition = new GameObject("Collection Position").transform;
        collectionPosition.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 120f, gameObject.transform.position.z);

        rigid = GetComponent<Rigidbody>(); // for character's movement
        cameraDifference = cam.transform.position - gameObject.transform.position; // get the camera difference in Start() in order to not calculate each time on FixedUpdate function
        if (DataManager.Instance != null)
        {
            int index = DataManager.Instance.buttonValue - 1; // array indexes are 0-based
            Debug.Log(index);
            Debug.Log(materialManager.materials.Length);
            if (index >= 0 && index < materialManager.materials.Length)
            {
                Debug.Log("İçine girdi");
                GetComponent<Renderer>().material = materialManager.materials[index];
            }
        }
        // getting the position and y axis of the object
        objectTransform = gameObject.transform;
        yPosition = objectTransform.position.y;

    }
    public void FixedUpdate()
    {
        collectionPosition.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 120f, gameObject.transform.position.z);
        characterMovements();
        cameraMovements();
        // Debug.Log(DataManager.Instance.buttonValue.ToString());
        characterRespawnn();
        updateCollectionPosition();
        updateCollectedBlocksPositions();
    }
    private void updateCollectedBlocksPositions()
    {
        int i = 1;
        // Debug.Log("_______");
        foreach (GameObject item in collectedBlocks)
        {
            item.transform.position = new Vector3(collectionPosition.position.x, (collectionPosition.position.y) + i, collectionPosition.position.z);
            i += 25;
        }
    }
    public void updateCollectionPosition()
    {
        collectionPosition.position = new Vector3(gameObject.transform.position.x, collectionPosition.position.y, gameObject.transform.position.z);
    }
    private void characterRespawnn()
    {
        yPosition = objectTransform.position.y;
        if (yPosition <= 45)
        {
            objectTransform.position = new Vector3(-64f, 56f, -1474.5f);
        }
    }
    public void characterMovements()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        direction = direction.normalized;

        if (direction != Vector3.zero)
        {
            rigid.AddForce(direction * speed);
        }
        else
        {
            // Apply a small amount of force in the opposite direction of the current velocity
            // This will gradually slow the object down when no keys are being pressed
            rigid.AddForce(-rigid.velocity * 0.1f);
        }
        if (rigid.velocity.magnitude > maxVelocity)
        {
            rigid.velocity = rigid.velocity.normalized * maxVelocity;
        }
    }
    public void cameraMovements()
    {
        cam.gameObject.transform.position = gameObject.transform.position + cameraDifference;
    }

    private void AIPlayerEncounter(Collider other)
    {
        if (other.name == "Player1" || other.name == "Player2")
        {
            Debug.Log("In player");
            // Store current Y positions
            float myCurrentY = transform.position.y;
            float playerCurrentY = other.transform.position.y;

            // Some other logic you might want to execute

            // Reset Y positions to their original values
            transform.position = new Vector3(transform.position.x, myCurrentY, transform.position.z);
            other.transform.position = new Vector3(other.transform.position.x, playerCurrentY, other.transform.position.z);
        }
        // if it is his block
        if (other.tag == "Block" && other.GetComponent<Renderer>().material.color == gameObject.GetComponent<Renderer>().material.color)
        {

            blockManager = other.GetComponent<SingleBlockManager>();
            if (blockManager == null)
            {
                Debug.LogError("SingleBlockManager not found on object " + other.gameObject.name);
                return;
            }
            if (!blockManager.isCollected)
            {
                other.transform.position = new Vector3(collectionPosition.position.x, (collectionPosition.position.y + 10) * numberOfBlockCollected, collectionPosition.position.z);
                numberOfBlockCollected += 1;
                numberOfBlocksOnTheGround += 1;
                blockManager.isCollected = true;
                collectedBlocks.Push(other.gameObject);
            }
        }
        // if it is not his block
        else if (other.tag == "Block" && other.GetComponent<Renderer>().material.color != gameObject.GetComponent<Renderer>().material.color)
        {
            blockManager = other.GetComponent<SingleBlockManager>();

            if (!blockManager.isIgnored)
            {

                // Ignore collision with the layer that blocks belong to
                Physics.IgnoreLayerCollision(gameObject.layer, other.gameObject.layer, true);

                // Get the block's renderer
                Renderer blockRenderer = other.GetComponent<Renderer>();
                // Get the current color
                Color blockColor = blockRenderer.material.color;
                // Set the alpha value to 50%
                blockColor.a = 0.01f;
                // Apply the new color
                blockRenderer.material.color = blockColor;

                blockManager.isIgnored = true;
            }
        }
    }

    private void RopeEncounter(Collider other)
    {
        if (other.tag == "Rope")
        {
            // Debug.Log("Rope is detected");
            // there is no rope
            if (!other.GetComponent<Renderer>().enabled)
            {
                // stack is not empty
                if (collectedBlocks.Count >= 1)
                {
                    // change the material and make it visible
                    other.GetComponent<Renderer>().material = gameObject.GetComponent<Renderer>().material;
                    other.GetComponent<Renderer>().enabled = true;
                    // reduce stack by one
                    GameObject lastBlock = collectedBlocks.Pop();
                    // assign new position to popped block object
                    // Note: lastBlock.transform.position doesn't work. Because it refers to a global position. That's why I have used localPosition here:
                    lastBlock.transform.localPosition = new Vector3(lastBlock.GetComponent<SingleBlockManager>().xAxisCor, lastBlock.GetComponent<SingleBlockManager>().yAxisCor, lastBlock.GetComponent<SingleBlockManager>().zAxisCor);
                    // assign rotation
                    lastBlock.GetComponent<SingleBlockManager>().isCollected = false;
                    lastBlock.transform.rotation = GameObject.Find("BlockRotation").transform.rotation;
                }
                // stack is empty
                else
                {
                    // nothing to do
                }
            }
            // there is rope
            else
            {
                // it is replaced by the character
                if (other.GetComponent<Renderer>().material.color == gameObject.GetComponent<Renderer>().material.color)
                {
                    // nothing to do
                }
                // replaced by another character
                else
                {
                    if (collectedBlocks.Count >= 1)
                    {
                        // change rope's material
                        other.GetComponent<Renderer>().material = gameObject.GetComponent<Renderer>().material;
                        // reduce stack by one
                        GameObject lastBlock = collectedBlocks.Pop();
                        // assign new position to popped block object
                        lastBlock.transform.localPosition = new Vector3(lastBlock.GetComponent<SingleBlockManager>().xAxisCor, lastBlock.GetComponent<SingleBlockManager>().yAxisCor, lastBlock.GetComponent<SingleBlockManager>().zAxisCor);                    // assign rotation
                        lastBlock.GetComponent<SingleBlockManager>().isCollected = false;
                        lastBlock.transform.rotation = GameObject.Find("BlockRotation").transform.rotation;
                    }
                    else
                    {

                    }
                }
            }
        }
    }

    private void LimitEncounter(Collider other)
    {
        if (other.tag == "Limit")
        {
            if (collectedBlocks.Count < 1)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 10f);
            }
        }
    }


    public void OnTriggerEnter(Collider other)
    {

        AIPlayerEncounter(other);
        RopeEncounter(other);
        LimitEncounter(other);   
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Block" && other.GetComponent<Renderer>().material.color != gameObject.GetComponent<Renderer>().material.color)
        {
            blockManager = other.GetComponent<SingleBlockManager>();
            if (blockManager.isIgnored)
            {
                // Re-enable collision with the block's layer
                Physics.IgnoreLayerCollision(gameObject.layer, other.gameObject.layer, false);

                // Restore the block's original color by setting alpha back to 1
                Renderer blockRenderer = other.GetComponent<Renderer>();
                Color blockColor = blockRenderer.material.color;
                blockColor.a = 1f;
                blockRenderer.material.color = blockColor;

                blockManager.isIgnored = false;
            }
        }
    }
}




