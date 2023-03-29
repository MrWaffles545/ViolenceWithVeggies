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

    public GameManager gameManager;

    //soil stage
    public int stage;
    public bool watered;

    //Fire variables
    public bool fireSpreadDone, onFire, rain;
    List<GameObject> fireSpreadRadius = new List<GameObject>();

    //growth bar
    public GameObject bar;

    //the selector for the crop planted
    public int crop;

    //variables for growing
    public float cropTime;
    public float cropReady;

    //crop prefabs to spawn when harvested
    public int goldChance, goldChanceMax;
    public GameObject carrot;
    public GameObject wheat;
    public GameObject potato;
    public GameObject turnip;
    public GameObject artichoke;


    void Start()
    {
        //makes the player target null at the start
        player = null;

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Soil").Length; i++)
        {
            GameObject target = GameObject.FindGameObjectsWithTag("Soil")[i];
            if (target.transform.position.x - transform.position.x <= 3 && target.transform.position.y - transform.position.y <= 3)
                fireSpreadRadius.Add(target);
        }
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
        if (watered && stage == 2 && cropTime <= cropReady)
        {
            cropTime += Time.deltaTime;
            bar.GetComponent<Transform>().localScale = new Vector2(cropTime / cropReady * 1, .15f);
        }

        //Fire spread code
        if (onFire && !fireSpreadDone && gameManager.weatherDuration <= 5)
        {
            GameObject target = fireSpreadRadius[Random.Range(0, fireSpreadRadius.Count / 2)];
            target.GetComponent<SoilController>().onFire = true;
            target.GetComponent<SoilController>().watered = false;
            fireSpreadDone = true;
        }

        //rain water after fire code
        if (rain && gameManager.weatherDuration <= 5)
        {
            watered = true;
            fireSpreadDone = false;
            rain = false;
        }

        //turns color (temp)
        if (onFire)
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        if (stage == -1)
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        if (stage == 1)
            gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        if (watered)
            gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        if (stage != 1 && !onFire && stage != -1 && !watered)
            gameObject.GetComponent<SpriteRenderer>().color = Color.black;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //flag for when the player exits contact
        if (collision.gameObject.tag == "Player")
        {
            isTouching = false;
            bar.GetComponent<SpriteRenderer>().enabled = false;
            if (player != null)
            {
                player.GetComponent<PlayerController>().speed = 7f;
                player.GetComponent<PlayerController>().showInteract = false;
            }
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
            if ((stage == 0 && playerItem == "Till") || (stage == 1 && (playerItem == "Carrot Seed" || playerItem == "Wheat Seed" || playerItem == "Potato Seed" || playerItem == "Turnip Seed" || playerItem == "Artichoke Seed")) 
                || (!watered && playerItem == "WaterCan") || (watered && stage == 2 && playerItem == "Hands" && cropTime >= cropReady))
                player.GetComponent<PlayerController>().showInteract = true;
            else
                player.GetComponent<PlayerController>().showInteract = false;

            if (watered)
            {
                if (stage == 2)
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
            if (!onFire)
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
                else if (!watered && playerItem == "WaterCan")
                {
                    Debug.Log("Watered");
                    watered = true;
                }
                //if the soil is stage 3 and the crop has grown it harvests it, gives the player the correct crop and adds 1 to stage
                else if (watered && stage == 2 && playerItem == "Hands" && cropTime >= cropReady)
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
                    watered = false;
                    stage = 0;
                }
            }

            //fire
            else if (onFire && playerItem == "WaterCan")
                onFire = false;

            if (stage == -1 && playerItem == "Till")
            {
                Debug.Log("Tilled The Snow");
                stage++;
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
        //gold veggie stuff
        goldChance = Random.Range(0, goldChanceMax);
        if (goldChance == 0)
            c.name = "Golden " + c.name;
        //makes the player have the crop
        script.holdingItem = c;
        script.itemName = c.name;
        script.isTouching = true;
        script.isHolding = true;
    }
}
