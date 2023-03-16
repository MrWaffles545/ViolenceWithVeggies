using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapController : MonoBehaviour
{
    public float range, rangeY, speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "item" && collision.transform.parent == null)
        {
            Debug.Log("Push");
            Vector2 temp = collision.GetComponent<Rigidbody2D>().velocity;
            if (collision.transform.position.x <= -range || collision.transform.position.x >= range)
                temp.x = 0;
            if (collision.transform.position.y <= -rangeY || collision.transform.position.y >= rangeY)
                temp.y = 0;
            collision.GetComponent<Rigidbody2D>().velocity = temp;
            collision.transform.position = Vector2.MoveTowards(collision.transform.position, Vector2.zero, speed);
        }
    }
}
