using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilController : MonoBehaviour
{
    public GameObject player;
    public bool isTouching;
    public int stage;
    public string playerItem;
    public int crop;
    public float cropTime;
    public float cropReady;

    public GameObject bar;

    public GameObject carrot;
    public GameObject wheat;
    public GameObject potato;
    public GameObject turnip;
    public GameObject artichoke;

    void Start()
    {
        player = null;
    }
    
    void Update()
    {
        if (player != null)
            playerItem = player.GetComponent<PlayerController>().itemName;
        else
            playerItem = null;

        if (stage == 3 && cropTime <= cropReady)
        {
            cropTime += Time.deltaTime;
            bar.GetComponent<Transform>().localScale = new Vector2(cropTime / cropReady * 1, .15f);
        }

        if (Input.GetMouseButtonDown(1) && isTouching)
        {
            if (stage == 0 && playerItem == "Till")
            {
                Debug.Log("Tilled");
                stage++;
            }
            else if (stage == 1 && (playerItem == "Carrot Seed" || playerItem == "Wheat Seed" || playerItem == "Potato Seed" || playerItem == "Turnip Seed" || playerItem == "Artichoke Seed"))
            {
                Debug.Log("Planted " + playerItem);
                Destroy(player.GetComponent<PlayerController>().holdingItem);
                player.GetComponent<PlayerController>().holdingItem = null;
                player.GetComponent<PlayerController>().itemName = "Hands";
                player.GetComponent<PlayerController>().isTouching = false;
                player.GetComponent<PlayerController>().isHolding = false;
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
            else if (stage == 2 && playerItem == "WaterCan")
            {
                Debug.Log("Watered");
                stage++;
            }
            else if (stage == 3 && playerItem == "Hands" && cropTime >= cropReady)
            {
                Debug.Log("Harvested");
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
                bar.GetComponent<Transform>().localScale = new Vector2(0, .15f);
                stage = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isTouching = false;
            bar.GetComponent<SpriteRenderer>().enabled = false;
            player = null;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (stage == 3)
                bar.GetComponent<SpriteRenderer>().enabled = true;
            isTouching = true;
            player = collision.gameObject;
        }
    }

    void Harvest(GameObject crop)
    {
        GameObject c = Instantiate(crop);
        c.transform.position = player.GetComponent<PlayerController>().objectPickupPos.position;
        c.transform.SetParent(player.transform);
        player.GetComponent<PlayerController>().holdingItem = c;
        player.GetComponent<PlayerController>().itemName = c.name;
        player.GetComponent<PlayerController>().isTouching = true;
        player.GetComponent<PlayerController>().isHolding = true;
    }
}
