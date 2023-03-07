using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public bool sellBox;
    public int veggieScore;
    public GameObject player;
    public string playerItem;


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
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player = collision.gameObject;
        }

        if ((playerItem == "Carrot(Clone)" || playerItem == "Wheat(Clone)" || playerItem == "Potato(Clone)" || playerItem == "Turnip(Clone)" || playerItem == "Artichoke(Clone)") && Input.GetMouseButtonDown(1))
        {
            Destroy(player.GetComponent<PlayerController>().holdingItem);
            player.GetComponent<PlayerController>().holdingItem = null;
            player.GetComponent<PlayerController>().itemName = "Hands";
            player.GetComponent<PlayerController>().isTouching = false;
            player.GetComponent<PlayerController>().isHolding = false;
        }

        if (collision.gameObject.name == "Wheat(Clone)" && Input.GetMouseButtonDown(1) && sellBox == true)
            veggieScore++;

        if (collision.gameObject.name == "Carrot(Clone)" && Input.GetMouseButtonDown(0) && sellBox == true)
            veggieScore += 2;

        if (collision.gameObject.name == "Potato(Clone)" && Input.GetMouseButtonDown(0) && sellBox == true)
            veggieScore += 2;

        if (collision.gameObject.name == "Turnip(Clone)" && Input.GetMouseButtonDown(0) && sellBox == true)
            veggieScore += 3;

        if (collision.gameObject.name == "Artichoke(Clone)" && Input.GetMouseButtonDown(0) && sellBox == true)
            veggieScore += 4;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player = null;
        }
    }





}
