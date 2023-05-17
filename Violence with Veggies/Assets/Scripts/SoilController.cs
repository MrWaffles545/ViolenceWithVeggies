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

    private GameManager gameManager;

    //soil stage
    public int stage;
    public bool watered;

    //soil overlays for the soil
    public GameObject fire, water, snow, sprout;
    public Sprite[] sprouts;

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
    public int goldChanceMax;
    public GameObject carrot, garlic, potato, turnip, gus;
    public Sprite[] daGold;


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
        }

        //timer for when the crop is growing in the soil
        if (watered && stage == 2)
        {
            if (cropTime <= cropReady)
            {
                cropTime += Time.deltaTime;
                bar.GetComponent<Transform>().localScale = new Vector2(cropTime / cropReady * 11.1f, 1.5f);
            }
            if (cropTime >= cropReady)
                sprout.GetComponent<SpriteRenderer>().sprite = sprouts[crop];
        }

        //Fire spread code
        if (onFire && !fireSpreadDone && gameManager.weatherDuration <= 5)
        {
            GameObject target = fireSpreadRadius[Random.Range(0, fireSpreadRadius.Count / 2)];
            target.GetComponent<SoilController>().onFire = true;
            target.GetComponent<SoilController>().watered = false;
            if (target.GetComponent<SoilController>().stage == 2)
                target.GetComponent<SoilController>().stage = 1;
            target.GetComponent<SoilController>().cropTime = 0f;
            target.GetComponent<SoilController>().bar.GetComponent<Transform>().localScale = new Vector2(0, .15f);
            fireSpreadDone = true;
        }

        //rain water after fire code
        if (rain && gameManager.weatherDuration <= 5)
        {
            watered = true;
            fireSpreadDone = false;
            rain = false;
        }

        //turns visual accordingly
        if (onFire && !fire.activeSelf)
        {
            fire.SetActive(true);
            sprout.SetActive(false);
        }
        if (!onFire && fire.activeSelf)
            fire.SetActive(false);

        if (stage == -1 && !snow.activeSelf)
        {
            snow.SetActive(true);
            sprout.SetActive(false);
        }
        if (stage != -1 && snow.activeSelf)
            snow.SetActive(false);

        if (watered && !water.activeSelf)
            water.SetActive(true);
        if (!watered && water.activeSelf)
            water.SetActive(false);

        if ((stage == 1 || stage == 2) && gameObject.GetComponent<SpriteRenderer>().enabled == false)
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        if (stage != 1 && stage != 2 && gameObject.GetComponent<SpriteRenderer>().enabled == true)
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //flag for when the player exits contact
        if (collision.gameObject.tag == "Player")
        {
            isTouching = false;
            //bar.GetComponent<SpriteRenderer>().enabled = false;
            if (player != null)
                player.GetComponent<PlayerController>().showInteract = false;
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
            if (((stage == -1 || stage == 0) && playerItem == "Till") || (stage == 1 && (playerItem == "Carrot Seed" || playerItem == "Garlic Bulb" || playerItem == "Potato Seed" || playerItem == "Turnip Seed" || playerItem == "Gus Seed")) 
                || (!watered && playerItem == "WaterCan") || (watered && stage == 2 && playerItem == "Hands" && cropTime >= cropReady))
                player.GetComponent<PlayerController>().showInteract = true;
            else
                player.GetComponent<PlayerController>().showInteract = false;

            if (watered && stage == 2 && !bar.GetComponent<SpriteRenderer>().enabled)
                bar.GetComponent<SpriteRenderer>().enabled = true;
        }
    }


    public void Interact(bool input)
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
                    player.GetComponent<PlayerController>().showInteract = false;
                    player.GetComponent<PlayerController>().anim.Play("Till");
                }
                //if the soil is stage 1 it detects which seed the player interacted on it with and adds 1 to stage
                else if (stage == 1 && (playerItem == "Carrot Seed" || playerItem == "Garlic Bulb" || playerItem == "Potato Seed" || playerItem == "Turnip Seed" || playerItem == "Gus Seed"))
                {
                    Debug.Log("Planted " + playerItem);
                    //gets rid of the item the player is holding
                    Destroy(script.holdingItem);
                    script.holdingItem = null;
                    script.itemName = "Hands";
                    script.isTouching = false;
                    script.isHolding = false;
                    script.pickup.text = "";
                    player.GetComponent<PlayerController>().showInteract = false;
                    sprout.GetComponent<SpriteRenderer>().sprite = sprouts[0];
                    sprout.SetActive(true);
                    //assigns the crop correspondant to the seed and sets the timer number
                    if (playerItem == "Carrot Seed")
                    {
                        crop = 1;
                        cropReady = 5;
                    }
                    else if (playerItem == "Garlic Bulb")
                    {
                        crop = 2;
                        cropReady = 5;
                    }
                    else if (playerItem == "Potato Seed")
                    {
                        crop = 3;
                        cropReady = 3;
                    }
                    else if (playerItem == "Turnip Seed")
                    {
                        crop = 4;
                        cropReady = 7;
                    }
                    else if (playerItem == "Gus Seed")
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
                    player.GetComponent<PlayerController>().showInteract = false;
                    player.GetComponent<PlayerController>().anim.Play("Water");
                }
                //if the soil is stage 3 and the crop has grown it harvests it, gives the player the correct crop and adds 1 to stage
                else if (watered && stage == 2 && playerItem == "Hands" && cropTime >= cropReady)
                {
                    Debug.Log("Harvested");
                    //harvests the selected crop
                    if (crop == 1)
                        Harvest(carrot);
                    else if (crop == 2)
                        Harvest(garlic);
                    else if (crop == 3)
                        Harvest(potato);
                    else if (crop == 4)
                        Harvest(turnip);
                    else if (crop == 5)
                        Harvest(gus);
                    cropTime = 0;
                    //resets the bar and stage
                    bar.GetComponent<Transform>().localScale = new Vector2(0, .15f);
                    script.speed = 7f;
                    sprout.SetActive(false);
                    watered = false;
                    stage = 0;
                }
            }

            //fire
            else if (onFire && playerItem == "WaterCan")
            {
                onFire = false;
                player.GetComponent<PlayerController>().anim.Play("Water");
            }

            if (stage == -1 && playerItem == "Till")
            {
                Debug.Log("Tilled The Snow");
                player.GetComponent<PlayerController>().anim.Play("Till");
                stage++;
            }
        }
    }

    void Harvest(GameObject veggie)
    {
        //spawns the correct crop
        GameObject c = Instantiate(veggie);
        //teleports the crop to the correct position
        c.transform.position = script.objectPickupPos.position;
        //assigns the player as the crops parent
        c.transform.SetParent(player.transform);
        //renames the crop that spawned to the correct name instead of crop + "(Clone)"
        c.name = veggie.name;
        //gold veggie stuff
        int goldChance = Random.Range(0, goldChanceMax);
        if (goldChance == 0)
        {
            c.name = "Golden " + c.name;
            c.GetComponent<SpriteRenderer>().sprite = daGold[crop - 1];
        }
        //makes the player have the crop
        script.holdingItem = c;
        script.itemName = c.name;
        script.isTouching = true;
        script.isHolding = true;
        script.pickup.text = "Press " + script.playerpickupbutton + " To Drop or Hold To Throw";
        player.GetComponent<PlayerController>().showInteract = false;
    }
}
