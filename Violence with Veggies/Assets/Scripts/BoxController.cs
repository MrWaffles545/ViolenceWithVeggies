using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    //changes wether the box is for selling of spawning seeds
    public bool sellBox;

    //player related variables
    public GameObject player;
    private PlayerController script;
    public string playerItem;
    public bool isTouching;

    //array containg each different seeds
    public GameObject[] seeds;

    //seed ready timer and visual bar
    public float seedTime;
    public float seedReady;
    public GameObject bar;

    // Update is called once per frame
    void Update()
    {

        //gets name of held item if the box is a seed box
        if (player != null && !sellBox)
        {
            script = player.GetComponent<PlayerController>();
            playerItem = script.itemName;
        }
        else if (player == null && !sellBox)
            playerItem = null;

        if (isTouching && script.inputType && !sellBox && !script.isHolding && seedTime >= seedReady)
        {
            GameObject temp = seeds[Random.Range(0, seeds.Length)];
            //spawns the correct crop
            GameObject c = Instantiate(temp);
            //teleports the crop to the correct position
            c.transform.position = script.objectPickupPos.position;
            //assigns the player as the crops parent
            c.transform.SetParent(player.transform);
            //renames the crop that spawned to the correct name instead of crop + "(Clone)"
            c.name = temp.name;
            //makes the player have the crop
            script.holdingItem = c;
            script.itemName = c.name;
            script.isTouching = true;
            script.isHolding = true;
            seedTime = 0;

        }

        //timer to bar convertion
        if (seedTime <= seedReady && !sellBox)
        {
            seedTime += Time.deltaTime;
            bar.GetComponent<Transform>().localScale = new Vector2(seedTime / seedReady * 1, .15f);
        }
    }

    //detects if a crops enters the trigger then sells it
    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((collision.gameObject.tag == "item" || collision.gameObject.tag == "Thrownitem1" || collision.gameObject.tag == "Thrownitem2") && sellBox && collision.gameObject.transform.parent == null)
        {
            PlayerController playerScore = player.GetComponent<PlayerController>();
            if (collision.name == "Wheat")
            {
                playerScore.score++;
                Destroy(collision.gameObject);
            }

            if (collision.name == "Carrot" && collision.name == "Potato")
            {
                playerScore.score += 2;
                Destroy(collision.gameObject);
            }

            if (collision.name == "Turnip")
            {
                playerScore.score += 3;
                Destroy(collision.gameObject);
            }

            if (collision.name == "Artichoke")
            {
                playerScore.score += 4;
                Destroy(collision.gameObject);
            }

            if (collision.name == "Golden Wheat")
            {
                playerScore.score += 2;
                Destroy(collision.gameObject);
            }

            if (collision.name == "Golden Carrot" && collision.name == "Golden Potato")
            {
                playerScore.score += 4;
                Destroy(collision.gameObject);
            }

            if (collision.name == "Golden Turnip")
            {
                playerScore.score += 6;
                Destroy(collision.gameObject);
            }

            if (collision.name == "Golden Artichoke")
            {
                playerScore.score += 8;
                Destroy(collision.gameObject);
            }
        }
        
        if (collision.gameObject.tag == "Player" && !sellBox)
        {
            player = collision.gameObject;
            if (!sellBox && !script.isHolding && seedTime >= seedReady)
                player.GetComponent<PlayerController>().showInteract = true;
            isTouching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !sellBox)
        {
            player.GetComponent<PlayerController>().showInteract = false;
            player = null;
            isTouching = false;
        }
    }
}
