using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //basic player variables
    private Rigidbody2D myRb;
    public float speed;

    //variables for the items the character picks up
    public Transform objectPickupPos;
    public GameObject holdingItem;
    public bool isHolding = false;
    public string itemName;

    //variables for the other items the character can pick up while holding one already
    private GameObject otherItem;
    public bool isTouching = false;
    public bool isTouchingOther = false;

    //2 player variables
    public bool playerOne;
    public Camera cam;

    //the variable to hold the gamepad input to move
    private Vector2 move;

    void Start()
    {
        //assigns the rigidbody when the game starts
        myRb = GetComponent<Rigidbody2D>();

        //makes the item the player is holding at the start "Hands"
        itemName = "Hands";
    }

    void Update()
    {
        //Gets the gamepad controller
        var gamepad = Gamepad.current;
        //if there isnt a gamepad it uses keyboard controls
        if (gamepad == null)
        {
            //if it is player one it uses basic controls and left click to pickup/drop
            if (playerOne)
            {
                Vector2 temp = myRb.velocity;
                temp.x = Input.GetAxisRaw("Horizontal") * speed;
                temp.y = Input.GetAxisRaw("Vertical") * speed;
                Pickup(Input.GetMouseButtonDown(0));
                myRb.velocity = temp;
            }
            //if it isnt player one it uses the mouses position to move and Q to pickup/drop
            else
            {
                Vector2 mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                transform.position = mousePos;
                Pickup(Input.GetKeyDown(KeyCode.Q));
            }
        }
        //if there is a gamepad controller it uses this
        else
        {
            //if player one it uses the left joystick to move and left trigger to pickup/drop
            if (playerOne)
            {
                if (gamepad.leftTrigger.wasPressedThisFrame)
                    Pickup(gamepad.leftTrigger.wasPressedThisFrame);
                move = gamepad.leftStick.ReadValue();
            }
            //if it isnt player one is uses right joystick to move and right trigger to pickup/drop
            else
            {
                if (gamepad.rightTrigger.wasPressedThisFrame)
                    Pickup(gamepad.rightTrigger.wasPressedThisFrame);
                move = gamepad.rightStick.ReadValue();
            }
            //converts the gamepad input to velocity of the player
            Vector2 temp = myRb.velocity;
            temp.x = move.x * speed;
            temp.y = move.y * speed;
            myRb.velocity = temp;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //when the player hits an item it flags a bool and assigns that item to holdingItem
        if (collision.gameObject.tag == "item" && !isHolding)
        {
            holdingItem = collision.gameObject;
            isTouching = true;
        }
        //if the player hits an item other than holdingItem it flags a different bool and assigns it to otherItem
        else if (collision.gameObject.tag == "item" && collision.gameObject != holdingItem && isHolding)
        {
            otherItem = collision.gameObject;
            isTouchingOther = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //when the player exits the item it flags a bool off and assigns holdingItem to null
        if (collision.gameObject.tag == "item" && !isHolding)
        {
            holdingItem = null;
            isTouching = false;
        }
        //when the player exits other item it flags a bool off and assigns otherItem to null
        else if (collision.gameObject == otherItem && isHolding)
        {
            otherItem = null;
            isTouchingOther = false;
        }
    }

    void Pickup(bool click)
    {
        //if the player presses the assigned button from above and isnt touching anything else it activates the pickup/drop
        if (click && isTouching && holdingItem != null && !isTouchingOther)
        {
            //if the player isnt holding an item it picks up the nearby item
            if (!isHolding)
            {
                //teleports the item to the correct position
                holdingItem.transform.position = objectPickupPos.position;
                //assigns the player as the parent so it doesnt move
                holdingItem.transform.SetParent(gameObject.transform);
                //makes the item the player picked up the itemName and flags the bool isHolding
                itemName = holdingItem.name;
                isHolding = true;
            }
            //drops the item the player is holding
            else
            {
                //sets the item the player was holding to have no parent object and make the player have no item it is holding
                holdingItem.transform.SetParent(null);
                holdingItem = null;
                itemName = "Hands";
                isHolding = false;
            }
        }
        //if the player has an item already and tries to pick up another it swaps the 2 items
        else if (click && holdingItem != null && isTouchingOther)
        {
            //makes a temp variable to hold the holdingItem
            GameObject tempGameObject = holdingItem;
            //drops holdingItem
            holdingItem.transform.SetParent(null);
            //assigns holdingItem to the otherItem
            holdingItem = otherItem;
            //finishes the swap
            otherItem = tempGameObject;
            //makes the new holdingItem the item the player is holding
            holdingItem.transform.position = objectPickupPos.position;
            holdingItem.transform.SetParent(gameObject.transform);
            itemName = holdingItem.name;
            isHolding = true;
            isTouchingOther = true;
            isTouching = true;
        }
    }
}
