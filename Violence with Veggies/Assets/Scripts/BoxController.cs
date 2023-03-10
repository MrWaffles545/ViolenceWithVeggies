using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public bool sellBox;
    public GameObject player;
    private PlayerController script;
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
        script = player.GetComponent<PlayerController>();
        if (player != null)
            playerItem = script.itemName;
        else
            playerItem = null;

        if (isTouching && script.inputType && sellBox == true)
        {
            if (playerItem == "Wheat")
                script.score++;

            if (playerItem == "Carrot")
                script.score += 2;

            if (playerItem == "Potato")
                script.score += 2;

            if (playerItem == "Turnip")
                script.score += 3;

            if (playerItem == "Artichoke")
                script.score += 4;

            if (playerItem == "Carrot" || playerItem == "Wheat" || playerItem == "Potato" || playerItem == "Turnip" || playerItem == "Artichoke")
            {
                Destroy(script.holdingItem);
                script.holdingItem = null;
                script.itemName = "Hands";
                script.isTouching = false;
                script.isHolding = false;
            }
        }

        else if (isTouching && script.inputType && !sellBox && !script.isHolding && seedTime >= seedReady)
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
