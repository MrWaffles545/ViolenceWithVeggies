using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Transform objectPickupPos;
    private Rigidbody2D myRb;
    public float speed;

    public bool isTouching = false;
    public bool isTouchingOther = false;

    public GameObject holdingItem;
    private GameObject otherItem;
    public bool isHolding = false;
    public string itemName = "Hands";

    public bool playerOne;
    public Camera cam;

    private Vector2 move;

    void Start()
    {
        myRb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            if (playerOne)
            {
                Vector2 temp = myRb.velocity;
                temp.x = Input.GetAxisRaw("Horizontal") * speed;
                temp.y = Input.GetAxisRaw("Vertical") * speed;
                Pickup(Input.GetMouseButtonDown(0));
                myRb.velocity = temp;
            }
            else
            {
                Vector2 mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                transform.position = mousePos;
            }
        }

        else
        {
            if (playerOne)
            {
                if (gamepad.leftTrigger.wasPressedThisFrame)
                    Pickup(gamepad.leftTrigger.wasPressedThisFrame);
                move = gamepad.leftStick.ReadValue();
            }

            else
            {
                if (gamepad.rightTrigger.wasPressedThisFrame)
                    Pickup(gamepad.rightTrigger.wasPressedThisFrame);
                move = gamepad.rightStick.ReadValue();
            }

            Vector2 temp = myRb.velocity;
            temp.x = move.x * speed;
            temp.y = move.y * speed;
            myRb.velocity = temp;
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

    void Pickup(bool click)
    {
        if (click && isTouching && holdingItem != null && !isTouchingOther)
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

        else if (click && holdingItem != null && isTouchingOther)
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
}
