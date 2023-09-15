using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class AIBallController : MonoBehaviour
{
    public float detectionRadius = 10000f;
    public float speed = 250f;
    public float maxVelocity = 60.0f;
    private Rigidbody rigid;
    private Material myMaterial;
    private Stack<GameObject> collectedBlocks;
    private Transform collectionPosition; // Initial position (I don't want to calculate each of them one by one everytime for GPU issues)
    Transform objectTransform;
    float yPosition;
    private SingleBlockManager blockManager;
    public int numberOfBlocksOnTheGround = 0;
    private int numberOfBlockCollected = 1;

    // private Vector3 targetPosition = new Vector3(-642, 0, 2463);
    private Transform targetOneTransform;
    private Transform targetTwoTransform;
    // private bool hasReachedTargetOne = false;
    // private bool headingToTargetTwo = false;  // New variable to track the state

    private int numberOfBlocksForCollecting;
    private int numberOfBlocksCollectedForAI = 0;

    private bool supposedToCollectBlocks = true;
    private bool reachedTargetOne = false;





    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        myMaterial = GetComponent<Renderer>().material;
        collectedBlocks = new Stack<GameObject>(); // initialize stack object
        collectionPosition = new GameObject("Collection Position " + gameObject.name).transform;
        collectionPosition.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 120f, gameObject.transform.position.z);
        objectTransform = gameObject.transform;
        yPosition = objectTransform.position.y;

        targetOneTransform = GameObject.Find("TargetOne").transform;
        targetTwoTransform = GameObject.Find("TargetTwo").transform;

        numberOfBlocksForCollecting = Random.Range(7, 15);
    }

    private void FixedUpdate()
    {
        MovementMechanics();
        collectionPosition.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 120f, gameObject.transform.position.z);
        updateCollectedBlocksPositions();
        updateCollectionPosition();
        characterRespawnn();
    }

    public void OnTriggerEnter(Collider other)
    {
        BlockEncounter(other);
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



    private void MovementMechanics()
    {
        // if it is supposed to collect blocks
        if(supposedToCollectBlocks && numberOfBlocksCollectedForAI <= numberOfBlocksForCollecting)
        {
            CollectBlocks();
        }
        // if it is not supposed to collect blocks
        else
        {
            // if it did not reach the target one
            if (!reachedTargetOne)
            {
                Vector3 aiBasePosition = transform.position;
                aiBasePosition.y = 0; // set y coordinate to ground level

                Vector3 targetOneBasePosition = targetOneTransform.position;
                targetOneBasePosition.y = 0; // set y coordinate to ground level

                float distanceToTargetOne = Vector3.Distance(aiBasePosition, targetOneBasePosition);


                if (distanceToTargetOne < 5.0f)  // Increased threshold
                {

                    reachedTargetOne = true;
                    Debug.Log("Reached TargetOne");
                }

                Vector3 directionToTargetOne = (targetOneTransform.position - transform.position).normalized;
                ApplyMovementLogic(directionToTargetOne);
                return;
            }
            // if it reached target one
            else
            {
                // go until reaching Target Two
                if(collectedBlocks.Count != 0)
                {
                    Vector3 directionToTargetTwo = (targetTwoTransform.position - transform.position).normalized;
                    directionToTargetTwo.y = 0; // Keep the y component unchanged
                    ApplyMovementLogic(directionToTargetTwo);
                }
                else
                {
                    supposedToCollectBlocks = true;
                    reachedTargetOne = false;
                    numberOfBlocksForCollecting = Random.Range(7, 15);
                    numberOfBlocksCollectedForAI = 0;
                }
            }
            
        }
    }



    private void CollectBlocks()
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, LayerMask.GetMask("BlockLayer"));

        Transform closestBlock = null;
        float closestDistance = detectionRadius + 1;

        foreach (var hitCollider in hitColliders)
        {


            Material blockMaterial = hitCollider.GetComponent<Renderer>().material;

            if (blockMaterial.color != myMaterial.color)
                continue;
            if (hitCollider.GetComponent<SingleBlockManager>().isCollected == true)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBlock = hitCollider.transform;
            }
        }

        if (closestBlock != null)
        {
            Vector3 directionToBlock = (closestBlock.position - transform.position).normalized;
            ApplyMovementLogic(directionToBlock);
        }
        else
        {
            Debug.Log("Else statement");
            supposedToCollectBlocks = false;
        }
    }


    private void characterRespawnn()
    {
        yPosition = objectTransform.position.y;
        if (yPosition <= 45)
        {
            Debug.Log("One of them");
            objectTransform.position = new Vector3(-542f, 56f, -1474.5f);
        }
    }
    
    
    
    private void ApplyMovementLogic(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            rigid.AddForce(direction * speed);
        }
        else
        {
            rigid.AddForce(-rigid.velocity * 0.1f);
        }
        if (rigid.velocity.magnitude > maxVelocity)
        {
            rigid.velocity = rigid.velocity.normalized * maxVelocity;
        }
    }



    private void BlockEncounter(Collider other)
    {
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
                numberOfBlocksCollectedForAI++;
            }

        }

        // if it is not his block
        else if (other.tag == "Block" && other.GetComponent<Renderer>().material.color != gameObject.GetComponent<Renderer>().material.color)
        {
            blockManager = other.GetComponent<SingleBlockManager>();
            // Debug.Log("_______");
            // Debug.Log(blockManager.isIgnored);
            // Debug.Log("_______");

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

    
    private void updateCollectedBlocksPositions()
    {
        int i = 1;
        // Debug.Log("_______");
        foreach (GameObject item in collectedBlocks)
        {

            // Debug.Log(item.GetComponent<SingleBlockManager>().blockId);     
            // item.GetComponent<Transform>().position = new Vector3();
            item.transform.position = new Vector3(collectionPosition.position.x, (collectionPosition.position.y) + i, collectionPosition.position.z);
            i += 25;
        }
    }
    public void updateCollectionPosition()
    {
        collectionPosition.position = new Vector3(gameObject.transform.position.x, collectionPosition.position.y, gameObject.transform.position.z);
    }
}
