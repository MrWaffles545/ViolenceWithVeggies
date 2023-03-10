using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public bool sellBox;
    public GameObject player;
    public string playerItem;
    public bool isTouching;
    public GameObject[] seeds;
    public float seedTime;
    public float seedReady;
    public GameObject bar;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
            playerItem = player.GetComponent<PlayerController>().itemName;
        else
            playerItem = null;

        if (isTouching && player.GetComponent<PlayerController>().inputType && sellBox == true)
        {
            if (playerItem == "Wheat")
                player.GetComponent<PlayerController>().score++;

            if (playerItem == "Carrot")
                player.GetComponent<PlayerController>().score += 2;

            if (playerItem == "Potato")
                player.GetComponent<PlayerController>().score += 2;

            if (playerItem == "Turnip")
                player.GetComponent<PlayerController>().score += 3;

            if (playerItem == "Artichoke")
                player.GetComponent<PlayerController>().score += 4;

            if (playerItem == "Carrot" || playerItem == "Wheat" || playerItem == "Potato" || playerItem == "Turnip" || playerItem == "Artichoke")
            {
                Destroy(player.GetComponent<PlayerController>().holdingItem);
                player.GetComponent<PlayerController>().holdingItem = null;
                player.GetComponent<PlayerController>().itemName = "Hands";
                player.GetComponent<PlayerController>().isTouching = false;
                player.GetComponent<PlayerController>().isHolding = false;
            }

   
        }

        else if (isTouching && player.GetComponent<PlayerController>().inputType && !sellBox && !player.GetComponent<PlayerController>().isHolding && seedTime >= seedReady)
        {
            GameObject temp = seeds[Random.Range(0, seeds.Length)];
            //spawns the correct crop
            GameObject c = Instantiate(temp);
            //teleports the crop to the correct position
            c.transform.position = player.GetComponent<PlayerController>().objectPickupPos.position;
            //assigns the player as the crops parent
            c.transform.SetParent(player.transform);
            //renames the crop that spawned to the correct name instead of crop + "(Clone)"
            c.name = temp.name;
            //makes the player have the crop
            player.GetComponent<PlayerController>().holdingItem = c;
            player.GetComponent<PlayerController>().itemName = c.name;
            player.GetComponent<PlayerController>().isTouching = true;
            player.GetComponent<PlayerController>().isHolding = true;
            seedTime = 0;

        }

        if (seedTime <= seedReady && !sellBox)
        {
            seedTime += Time.deltaTime;
            bar.GetComponent<Transform>().localScale = new Vector2(seedTime / seedReady * 1, .15f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player = collision.gameObject;
            isTouching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player = null;
            isTouching = false;
        }
    }





}
