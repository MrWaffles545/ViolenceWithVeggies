using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //basic player variables
    private Rigidbody2D myRb;
    public float speed;

    //Game Manager script
    public GameManager gameManager;

    //variables for the items the character picks up
    public Transform objectPickupPos;
    public GameObject holdingItem;
    public bool isHolding = false;
    public string itemName;

    //variables for the other items the character can pick up while holding one already
    private GameObject otherItem;
    public bool isTouching = false;
    public bool isTouchingOther = false;

    //two player variables
    public bool playerOne;
    public Camera cam;
    public Gamepad gamepad, gamepad2;

    //input type variable for other scripts
    public bool pickupButton;
    public bool pickupButtonRelease;
    public bool inputType;

    //Hold input variables
    public float time;
    public float timeToThrow;
    public bool hold;
    public float throwSpeedMin;
    public GameObject throwBar;

    //the variable to hold the gamepad input to move
    private Vector2 move;

    //player score variable
    public int score;

    void Start()
    {
        //assigns the rigidbody when the game starts
        myRb = GetComponent<Rigidbody2D>();

        //makes the item the player is holding at the start "Hands"
        itemName = "Hands";

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    void Update()
    {
        if (Time.timeScale != 0)
        {
            //Gets the gamepad controller
            if (Gamepad.all.Count > 0)
                gamepad = Gamepad.all[0];
            else
                gamepad = null;
            if (Gamepad.all.Count > 1)
                gamepad2 = Gamepad.all[1];
            else
                gamepad2 = null;
            //if there isnt a gamepad it uses keyboard controls
            Vector2 temp = myRb.velocity;
            if (playerOne)
            {
                if (gamepad != null)
                {
                    //if it is player one is uses controller one
                    pickupButton = gamepad.buttonEast.wasPressedThisFrame;
                    pickupButtonRelease = gamepad.buttonEast.wasReleasedThisFrame;
                    move = gamepad.leftStick.ReadValue();
                    inputType = gamepad.buttonSouth.wasPressedThisFrame;
                } 
                else
                {
                    //if it is player one it uses basic controls and left click to pickup/drop
                    move.x = Input.GetAxisRaw("Horizontal");
                    move.y = Input.GetAxisRaw("Vertical");
                    pickupButton = Input.GetMouseButtonDown(0);
                    pickupButtonRelease = Input.GetMouseButtonUp(0);
                    inputType = Input.GetMouseButtonDown(1);
                }
            }
            else
            {
                //if there is a gamepad controller it uses this
                if (gamepad2 != null)
                {
                    //if it isnt player one is uses the other controller
                    pickupButton = gamepad2.buttonEast.wasPressedThisFrame;
                    pickupButtonRelease = gamepad2.buttonEast.wasReleasedThisFrame;
                    move = gamepad2.leftStick.ReadValue();
                    inputType = gamepad2.buttonSouth.wasPressedThisFrame;
                }
                else
                {
                    //if it isnt player one it uses the mouses position to move and Q to pickup/drop
                    Vector2 mousePos = Input.mousePosition;
                    mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                    transform.position = mousePos;
                    pickupButton = Input.GetKeyDown(KeyCode.Q);
                    pickupButtonRelease = Input.GetKeyUp(KeyCode.Q);
                    inputType = Input.GetKeyDown(KeyCode.E);
                }
            }
            //converts the gamepad input to velocity of the player and pickup/drop/swap
            temp.x = move.x * speed;
            temp.y = move.y * speed;
            myRb.velocity = temp;


            //hold code to throw or pickup/drop/swap
            if (pickupButton)
            {
                if (!isHolding)
                    Pickup();
                else
                    hold = true;
            }
            if (pickupButtonRelease && hold)
            {
                if (time >= timeToThrow)
                    Throw();
                if (time <= timeToThrow)
                    DropSwap();
                throwBar.GetComponent<SpriteRenderer>().enabled = false;
                time = 0;
                hold = false;
            }
            if (time <= timeToThrow && hold)
            {
                time += Time.deltaTime;
                throwBar.GetComponent<SpriteRenderer>().enabled = true;
                throwBar.GetComponent<Transform>().localScale = new Vector2(time * 4, .25f);
            }
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
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
        if ((playerOne && collision.gameObject.tag == "ThrownItem2") || (!playerOne && collision.gameObject.tag == "ThrownItem1"))
        {
            Debug.Log("Smack");
            collision.GetComponent<Rigidbody2D>().velocity = gameManager.wind;
            collision.tag = "item";
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

    void DropSwap()
    {
        //if the player presses the assigned button from above and isnt touching anything else it activates the pickup/drop
        if (isTouching && holdingItem != null && !isTouchingOther && isHolding)
        {
            //sets the item the player was holding to have no parent object and make the player have no item it is holding
            holdingItem.transform.SetParent(null);
            holdingItem.GetComponent<Rigidbody2D>().velocity = gameManager.wind;
            itemName = "Hands";
            isHolding = false;
        }
        //if the player has an item already and tries to pick up another it swaps the 2 items
        else if (holdingItem != null && isTouchingOther && otherItem.transform.parent == null)
        {
            //makes a temp variable to hold the holdingItem
            GameObject tempGameObject = holdingItem;
            //drops holdingItem
            holdingItem.transform.SetParent(null);
            holdingItem.GetComponent<Rigidbody2D>().velocity = gameManager.wind;
            //assigns holdingItem to the otherItem
            holdingItem = otherItem;
            //finishes the swap
            otherItem = tempGameObject;
            //makes the new holdingItem the item the player is holding
            holdingItem.transform.position = objectPickupPos.position;
            holdingItem.transform.SetParent(gameObject.transform);
            itemName = holdingItem.name;
            isHolding = true;
            holdingItem.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            isTouchingOther = true;
            isTouching = true;
        }
    }

    void Pickup()
    {
        //if the player presses the assigned button from above and isnt touching anything else it activates the pickup/drop
        if (isTouching && holdingItem != null && !isTouchingOther && !isHolding)
        {
            //if the player isnt holding an item it picks up the nearby item
            if (holdingItem.transform.parent != null)
            {
                holdingItem.GetComponentInParent<PlayerController>().isHolding = false;
                holdingItem.GetComponentInParent<PlayerController>().itemName = "Hands";
            }
            //teleports the item to the correct position
            holdingItem.transform.position = objectPickupPos.position;
            //assigns the player as the parent so it doesnt move
            holdingItem.transform.SetParent(gameObject.transform);
            //makes the item the player picked up the itemName and flags the bool isHolding
            itemName = holdingItem.name;
            isHolding = true;
            holdingItem.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    void Throw()
    {
        Debug.Log("Throw");
        holdingItem.transform.SetParent(null);
        if (myRb.velocity.x <= -throwSpeedMin || myRb.velocity.x >= throwSpeedMin || myRb.velocity.y <= -throwSpeedMin || myRb.velocity.y >= throwSpeedMin)
        {
            holdingItem.GetComponent<Rigidbody2D>().velocity = (myRb.velocity * 2);
            if (playerOne)
                holdingItem.tag = "ThrownItem1";
            else
                holdingItem.tag = "ThrownItem2";
            holdingItem = null;
        }
        itemName = "Hands";
        isHolding = false;
    }
}
