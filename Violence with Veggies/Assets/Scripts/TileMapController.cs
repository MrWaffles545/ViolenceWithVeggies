using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapController : MonoBehaviour
{
    public float range, rangeY, speed;

    //Game Manager script
    public GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "item" && collision.transform.parent == null)
        {
            //makes temp variable for velocity
            Vector2 temp = collision.GetComponent<Rigidbody2D>().velocity;
            //if the item is outside the bounds it changes the velocity to zero so it doesnt push that way anymore

            //fix this line of code so it doesnt stop at the opposite side of the map relative to the wind push direction
            //          |
            //          V
            if (collision.transform.position.x <= -range || collision.transform.position.x >= range)
                temp.x = 0;
            if (collision.transform.position.y <= -rangeY || collision.transform.position.y >= rangeY)
                temp.y = 0;
            collision.GetComponent<Rigidbody2D>().velocity = temp;
            //Pushes the item torwards the center if it goes out of bounds
            collision.transform.position = Vector2.MoveTowards(collision.transform.position, Vector2.zero, speed);
        }
        if (collision.gameObject.tag == "ThrownItem2" || collision.gameObject.tag == "ThrownItem1")
        {
            collision.GetComponent<Rigidbody2D>().velocity = gameManager.wind;
            collision.tag = "item";
        }
    }
}
