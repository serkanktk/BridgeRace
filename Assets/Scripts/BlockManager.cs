using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class BlockManager : MonoBehaviour
{
    

    public int numberOfBlocksOnTheGroundPlayerOne = 0, numberOfBlocksOnTheGroundPlayerTwo = 0;

    private MainCharacter mainCharacterScript;


    [Header("Materials Manager Script")]
    public MaterialManager materialManager; // Reference to the MaterialManager script


    private void Start()
    {
        GameObject blocksParent = GameObject.Find("BlocksOnTheGround"); // find block's parent

        GameObject mainCharacter = GameObject.Find("MainCharacter"); // find main character game object

        Renderer mainCharacterRenderer = mainCharacter.GetComponent<Renderer>(); // find main character game object's renderer

        mainCharacterScript = mainCharacter.GetComponent<MainCharacter>(); // main character's script

        int numberOfBlocksOnTheGroundMainCharacter = mainCharacterScript.numberOfBlocksOnTheGround;
        
        // Debug.Log((numberOfBlocksOnTheGroundMainCharacter).ToString());


        /*  SETTING MATERIALS IN THE SCENE  */
        Material[] materials = new Material[3];
        materials[0] = mainCharacterRenderer.material; // main character material
        materials[1] = materialManager.materials[6]; // player one's material
        materials[2] = materialManager.materials[7]; // player two's material



        /*  SETTING BLOCKS IN THE SCENE*/
        Random random = new Random();
               
        if (blocksParent != null)
        {
            int x;
            bool playerOneCompleted = false, playerTwoCompleted = false, mainPlayerCompleted = false;
            // Loop through each block from Block0 to Block41
            for (int i = 0; i < 42; i++)
            {
                string blockName = "Block" + i;
                
                Renderer blockRenderer = blocksParent.transform.Find(blockName)?.gameObject.GetComponent<Renderer>();
                // blockRenderer.material = mainCharacterRenderer.material;
                if (!playerOneCompleted && !playerTwoCompleted && !mainPlayerCompleted) // 0, 1, 2
                {
                    x = random.Next(0, 3);
                    blockRenderer.material = materials[x];
                    if(x == 0)
                    {
                        numberOfBlocksOnTheGroundMainCharacter++;
                        if(numberOfBlocksOnTheGroundMainCharacter >= 14)
                        {
                            mainPlayerCompleted = true;
                        }
                    }
                    else if(x == 1)
                    {
                        numberOfBlocksOnTheGroundPlayerOne++;
                        if(numberOfBlocksOnTheGroundPlayerOne >= 14)
                        {
                            playerOneCompleted = true;
                        }
                    }
                    else
                    {
                        numberOfBlocksOnTheGroundPlayerTwo++;
                        if(numberOfBlocksOnTheGroundPlayerTwo >= 14)
                        {
                            playerTwoCompleted = true;
                        }
                    }
                }
                else if(!mainPlayerCompleted && !playerOneCompleted) // 0, 1
                {
                    x = random.Next(0, 2);
                    blockRenderer.material = materials[x];
                    if(x == 0)
                    {
                        numberOfBlocksOnTheGroundMainCharacter++;
                        if(numberOfBlocksOnTheGroundMainCharacter >= 14)
                        {
                            mainPlayerCompleted = true;
                        }
                    }
                    else
                    {
                        numberOfBlocksOnTheGroundPlayerOne++;
                        if(numberOfBlocksOnTheGroundPlayerOne >= 14)
                        {
                            playerOneCompleted = true;
                        }
                    }
                }
                else if(!mainPlayerCompleted && !playerTwoCompleted) // 0, 2
                {
                    x = random.Next(0, 2) * 2;
                    blockRenderer.material = materials[x];
                    if (x == 0)
                    {
                        numberOfBlocksOnTheGroundMainCharacter++;
                        if (numberOfBlocksOnTheGroundMainCharacter >= 14)
                        {
                            mainPlayerCompleted = true;
                        }
                    }
                    else
                    {
                        numberOfBlocksOnTheGroundPlayerTwo++;
                        if(numberOfBlocksOnTheGroundPlayerTwo >= 14)
                        {
                            playerTwoCompleted = true;
                        }
                    }
                }
                else if(!playerOneCompleted && !playerTwoCompleted) // 1, 2
                {
                    x = random.Next(1, 3);
                    blockRenderer.material = materials[x];
                    if(x == 1)
                    {
                        numberOfBlocksOnTheGroundPlayerOne++;
                        if(numberOfBlocksOnTheGroundPlayerOne >= 14)
                        {
                            playerOneCompleted = true;
                        }
                    }
                    else
                    {
                        numberOfBlocksOnTheGroundPlayerTwo++;
                        if(numberOfBlocksOnTheGroundPlayerTwo >= 14)
                        {
                            playerTwoCompleted = true;
                        }
                    }
                }
                else if (!mainPlayerCompleted)
                {
                    blockRenderer.material = materials[0];
                    numberOfBlocksOnTheGroundMainCharacter++;
                    if(numberOfBlocksOnTheGroundMainCharacter >= 14)
                    {
                        mainPlayerCompleted = true;
                    }
                }
                else if (!playerOneCompleted)
                {
                    blockRenderer.material = materials[1];
                    numberOfBlocksOnTheGroundPlayerOne++;
                    if (numberOfBlocksOnTheGroundPlayerOne >= 14)
                    {
                        playerOneCompleted = true;
                    }
                }
                else
                {
                    blockRenderer.material = materials[2];
                    numberOfBlocksOnTheGroundPlayerTwo++;
                    if(numberOfBlocksOnTheGroundPlayerTwo >= 14)
                    {
                        playerTwoCompleted = true;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Parent GameObject 'BlocksOnTheGround' not found!");
        }
    }
}
