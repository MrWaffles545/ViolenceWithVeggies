using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public bool sellBox;
    public int veggieScore;
    public GameObject player;
    public string playerItem;
    public bool isTouching;


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

        if (isTouching && Input.GetMouseButtonDown(1) && sellBox == true)
        {
            if (playerItem == "Wheat(Clone)")
                veggieScore++;

            if (playerItem == "Carrot(Clone)")
                veggieScore += 2;

            if (playerItem == "Potato(Clone)")
                veggieScore += 2;

            if (playerItem == "Turnip(Clone)")
                veggieScore += 3;

            if (playerItem == "Artichoke(Clone)")
                veggieScore += 4;

            if (playerItem == "Carrot(Clone)" || playerItem == "Wheat(Clone)" || playerItem == "Potato(Clone)" || playerItem == "Turnip(Clone)" || playerItem == "Artichoke(Clone)")
            {
                Destroy(player.GetComponent<PlayerController>().holdingItem);
                player.GetComponent<PlayerController>().holdingItem = null;
                player.GetComponent<PlayerController>().itemName = "Hands";
                player.GetComponent<PlayerController>().isTouching = false;
                player.GetComponent<PlayerController>().isHolding = false;
            }


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
