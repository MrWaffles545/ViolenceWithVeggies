using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoilController : MonoBehaviour
{
    //variables for the player
    public GameObject player;
    private PlayerController script;
    public string playerItem;
    public bool isTouching;

    //soil stage
    public int stage;

    //growth bar
    public GameObject bar;

    //the selector for the crop planted
    public int crop;

    //variables for growing
    public float cropTime;
    public float cropReady;

    //crop prefabs to spawn when harvested
    public GameObject carrot;
    public GameObject wheat;
    public GameObject potato;
    public GameObject turnip;
    public GameObject artichoke;

    void Start()
    {
        //makes the player target null at the start
        player = null;
    }

    void Update()
    {
        //checks if the soil is touching a player
        if (player != null)
        {
            script = player.GetComponent<PlayerController>();
            //assigns playerItem to what the player is holding
            playerItem = script.itemName;
            Interact(script.inputType);
        }

        //timer for when the crop is growing in the soil
        if (stage == 3 && cropTime <= cropReady)
        {
            cropTime += Time.deltaTime;
            bar.GetComponent<Transform>().localScale = new Vector2(cropTime / cropReady * 1, .15f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //flag for when the player exits contact
        if (collision.gameObject.tag == "Player")
        {
            isTouching = false;
            bar.GetComponent<SpriteRenderer>().enabled = false;
            if (player != null)
                player.GetComponent<PlayerController>().speed = 7f;
            playerItem = null;
            player = null;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //flag for when the player comes in contact and assigns the player to the player variable
        if (collision.gameObject.tag == "Player")
        {
            isTouching = true;
            player = collision.gameObject;
            if (stage == 3)
            {
                bar.GetComponent<SpriteRenderer>().enabled = true;
                player.GetComponent<PlayerController>().speed = 2f;
            }
        }
    }


    void Interact(bool input)
    {
        //detects if the player hits the right button and is touching the player
        if (input && isTouching)
        {
            //if the soil is stage 0 it tills it and adds 1 to stage
            if (stage == 0 && playerItem == "Till")
            {
                Debug.Log("Tilled");
                stage++;
            }
            //if the soil is stage 1 it detects which seed the player interacted on it with and adds 1 to stage
            else if (stage == 1 && (playerItem == "Carrot Seed" || playerItem == "Wheat Seed" || playerItem == "Potato Seed" || playerItem == "Turnip Seed" || playerItem == "Artichoke Seed"))
            {
                Debug.Log("Planted " + playerItem);
                //gets rid of the item the player is holding
                Destroy(script.holdingItem);
                script.holdingItem = null;
                script.itemName = "Hands";
                script.isTouching = false;
                script.isHolding = false;
                //assigns the crop correspondant to the seed and sets the timer number
                if (playerItem == "Carrot Seed")
                {
                    crop = 1;
                    cropReady = 5;
                }
                else if (playerItem == "Wheat Seed")
                {
                    crop = 2;
                    cropReady = 3;
                }
                else if (playerItem == "Potato Seed")
                {
                    crop = 3;
                    cropReady = 5;
                }
                else if (playerItem == "Turnip Seed")
                {
                    crop = 4;
                    cropReady = 7;
                }
                else if (playerItem == "Artichoke Seed")
                {
                    crop = 5;
                    cropReady = 10;
                }
                stage++;
            }
            //if the soil is stage 2 it waters it and adds 1 to stage
            else if (stage == 2 && playerItem == "WaterCan")
            {
                Debug.Log("Watered");
                stage++;
            }
            //if the soil is stage 3 and the crop has grown it harvests it, gives the player the correct crop and adds 1 to stage
            else if (stage == 3 && playerItem == "Hands" && cropTime >= cropReady)
            {
                Debug.Log("Harvested");
                //harvests the selected crop
                if (crop == 1)
                    Harvest(carrot);
                else if (crop == 2)
                    Harvest(wheat);
                else if (crop == 3)
                    Harvest(potato);
                else if (crop == 4)
                    Harvest(turnip);
                else if (crop == 5)
                    Harvest(artichoke);
                cropTime = 0;
                //resets the bar and stage
                bar.GetComponent<Transform>().localScale = new Vector2(0, .15f);
                script.speed = 7f;
                stage = 0;
            }
        }
    }

    void Harvest(GameObject crop)
    {
        //spawns the correct crop
        GameObject c = Instantiate(crop);
        //teleports the crop to the correct position
        c.transform.position = script.objectPickupPos.position;
        //assigns the player as the crops parent
        c.transform.SetParent(player.transform);
        //renames the crop that spawned to the correct name instead of crop + "(Clone)"
        c.name = crop.name;
        //makes the player have the crop
        script.holdingItem = c;
        script.itemName = c.name;
        script.isTouching = true;
        script.isHolding = true;
    }
}
