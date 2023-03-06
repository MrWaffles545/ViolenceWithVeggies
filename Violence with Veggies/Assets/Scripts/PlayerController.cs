using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D myRb;
    public float speed;
    public Transform objectPickupPos;
    public bool isTouching = false;
    public bool isTouchingOther = false;
    public bool isHolding = false;
    public string itemName = "Hands";
    public GameObject holdingItem;
    private GameObject otherItem;
    
    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        Vector2 temp = myRb.velocity;
        temp.x = Input.GetAxisRaw("Horizontal") * speed;
        temp.y = Input.GetAxisRaw("Vertical") * speed;
        myRb.velocity = temp;

        if (Input.GetMouseButtonDown(0) && isTouching && holdingItem != null && !isTouchingOther)
        {
            if (!isHolding)
            {
                holdingItem.transform.position = objectPickupPos.position;
                holdingItem.transform.SetParent(gameObject.transform);
                itemName = holdingItem.name;
                isHolding = true;
            }
            else
            {
                holdingItem.transform.SetParent(null);
                holdingItem = null;
                itemName = "Hands";
                isHolding = false;
            }
        }

        else if (Input.GetMouseButtonDown(0) && holdingItem != null && isTouchingOther)
        {
            GameObject tempGameObject = holdingItem;
            holdingItem.transform.SetParent(null);
            holdingItem = otherItem;
            otherItem = tempGameObject;
            holdingItem.transform.position = objectPickupPos.position;
            holdingItem.transform.SetParent(gameObject.transform);
            itemName = holdingItem.name;
            isHolding = true;
            isTouchingOther = true;
            isTouching = true;
        }
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "item" && !isHolding)
        {
            holdingItem = collision.gameObject;
            isTouching = true;
        }
        else if (collision.gameObject.tag == "item" && collision.gameObject != holdingItem && isHolding)
        {
            otherItem = collision.gameObject;
            isTouchingOther = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "item" && !isHolding)
        {
            holdingItem = null;
            isTouching = false;
        }
        else if (collision.gameObject == otherItem && isHolding)
        {
            otherItem = null;
            isTouchingOther = false;
        }
    }
}
