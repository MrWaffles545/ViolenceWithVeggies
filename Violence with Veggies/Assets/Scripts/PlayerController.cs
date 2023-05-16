using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //basic player variables
    private Rigidbody2D myRb;
    public float speed;
    public Animator anim;

    //Game Manager script
    private GameManager gameManager;

    //UI variables for the tutorial
    public TextMeshProUGUI pickup, interact;
    public bool showInteract;
    public string playerpickupbutton = "E", playerinteractbutton = "E";

    //variables for the items the character picks up
    public Transform objectPickupPos;
    public GameObject holdingItem;
    public bool isHolding = false;
    public string itemName;
    public GameObject interactItem;

    //variables for the other items the character can pick up while holding one already
    private GameObject otherItem;
    public bool isTouching = false;
    public bool isTouchingOther = false;

    //Fire variables
    public bool onFire, onFireCooldown;
    public float fireTimer, igniteTime, fireTime, fireCooldown, fireEffectTimer;
    public float fireEffectMin, fireEffectMax, fireEffect;
    public GameObject fireSoil;

    //two player variables
    public bool playerOne;
    public Camera cam;
    public Gamepad gamepad, gamepad2;

    //input type variable for player
    public InputAction joyStick, pickupButton, interactButton;

    //Soil that is going to be interacted with
    public GameObject soil;

    //Hold input variables
    public float time;
    public float timeToThrow;
    public bool hold;
    public float throwSpeedMin;
    public GameObject throwBar;

    //Stun
    public float stunTimer;
    public bool stunned;

    //the variable to hold the gamepad input to move
    private Vector2 move;

    //player score variable
    public int score;

    //sounds
    public AudioSource walking;

    public void OnEnable()
    {
        joyStick.Enable();
        pickupButton.Enable();
        interactButton.Enable();
    }

    void Start()
    {
        //assigns the rigidbody when the game starts
        myRb = GetComponent<Rigidbody2D>();

        //makes the item the player is holding at the start "Hands"
        itemName = "Hands";

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.timeScale != 0 && gameManager.GetComponent<GameManager>().canInput)
        {
            /*if (playerOne)
            {
                if (gamepad != null)
                {
                    //if it is player one is uses controller one
                    pickupButton = gamepad.buttonEast.wasPressedThisFrame;
                    pickupButtonRelease = gamepad.buttonEast.wasReleasedThisFrame;
                    move = gamepad.leftStick.ReadValue();
                    inputType = gamepad.buttonSouth.wasPressedThisFrame;
                    playerpickupbutton = "The Red Button";
                    playerinteractbutton = "The Green Button";
                }
                else
                {
                    //if it is player one it uses basic controls and left click to pickup/drop
                    move.x = Input.GetAxisRaw("Horizontal");
                    move.y = Input.GetAxisRaw("Vertical");
                    pickupButton = Input.GetMouseButtonDown(0);
                    pickupButtonRelease = Input.GetMouseButtonUp(0);
                    inputType = Input.GetMouseButtonDown(1);
                    playerpickupbutton = "Left Click";
                    playerinteractbutton = "Right Click";
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
                    playerpickupbutton = "The Red Button";
                    playerinteractbutton = "The Green Button";
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
                    playerpickupbutton = "Q";
                    playerinteractbutton = "E";
                }
            }*/

            playerpickupbutton = "The purple Button";
            playerinteractbutton = "The yellow Button";

            if (!stunned)
            {
                //converts the gamepad input to velocity of the player and pickup/drop/swap
                Vector2 temp = myRb.velocity;
                temp = joyStick.ReadValue<Vector2>() * speed * fireEffect;
                myRb.velocity = temp;

                if (myRb.velocity.x <= -0.1f)
                    GetComponent<SpriteRenderer>().flipX = false;
                else
                    GetComponent<SpriteRenderer>().flipX = true;


                //Interact stuff
                if (soil != null)
                {
                    soil.GetComponent<SoilController>().Interact(interactButton.WasPressedThisFrame());
                    if (interactButton.WasPressedThisFrame() && holdingItem != null)
                        interactItem = holdingItem;
                }

                if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Water") || anim.GetCurrentAnimatorStateInfo(0).IsName("Till")) && interactItem != null && interactItem.activeSelf)
                    interactItem.SetActive(false);
                else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Water") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Till") && interactItem != null && !interactItem.activeSelf)
                    interactItem.SetActive(true);

                if (myRb.velocity != Vector2.zero)
                {
                    anim.SetBool("Walking", true);
                    if(!walking.isPlaying)
                        walking.Play(0);

                }
                else
                {
                    anim.SetBool("Walking", false);
                    if (walking.isPlaying)
                        walking.Stop();
                }

                //hold code to throw or pickup/drop/swap
                if (pickupButton.WasPressedThisFrame())
                {
                    if (!isHolding)
                        Pickup();
                    else
                        hold = true;
                }
                if (pickupButton.WasReleasedThisFrame() && hold)
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


            //Hit stun stuff and timer
            if (stunTimer >= 0)
                stunTimer -= Time.deltaTime;
            else if (stunTimer <= 0 && stunned)
            {
                anim.SetBool("nutS", false);
                anim.Play("Idle");
                stunned = false;
            }

            //on fire code
            if (fireTimer <= fireCooldown && (fireSoil != null || fireTimer >= igniteTime))
                fireTimer += Time.deltaTime;
            //light on fire code
            if (fireTimer >= igniteTime && fireTimer <= fireTime)
            {
                if (!onFire)
                    onFire = true;
                if (!onFireCooldown)
                    onFireCooldown = true;
                //fire stuff
                //Debug.Log("on Fire");
                if (fireEffectTimer >= 0)
                    fireEffectTimer -= Time.deltaTime;
                else
                {
                    if (fireEffect > 0)
                        fireEffect = -1f;
                    else if (fireEffect < 0)
                        fireEffect = 1f;
                    fireEffectTimer = Random.Range(fireEffectMin, fireEffectMax);
                }
            }
            if (onFire && fireTimer >= fireTime)
            {
                fireEffect = 1f;
                onFire = false;
            }
            //fire cooldown stuff
            if (onFireCooldown && fireTimer >= fireCooldown)
            {
                onFireCooldown = false;
                fireTimer = 0;
                fireSoil = null;
            }

            //Interact tutorial text stuff
            if (showInteract && interact.text != "Press " + playerinteractbutton + " To Interact")
                interact.text = "Press " + playerinteractbutton + " To Interact";
            if (!showInteract && interact.text != "")
                interact.text = "";
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        //when the player hits an item it flags a bool and assigns that item to holdingItem
        if (collision.gameObject.tag == "item" && !isHolding)
        {
            holdingItem = collision.gameObject;
            isTouching = true;
            pickup.text = "Press " + playerpickupbutton + " To Pickup";
        }
        //if the player hits an item other than holdingItem it flags a different bool and assigns it to otherItem
        else if (collision.gameObject.tag == "item" && collision.gameObject != holdingItem && isHolding)
        {
            otherItem = collision.gameObject;
            isTouchingOther = true;
            pickup.text = "Press " + playerpickupbutton + " To Swap Items";
        }
        if (collision.gameObject.tag == "Soil" && collision.GetComponent<SoilController>().onFire && fireTimer <= igniteTime && !onFireCooldown)
            fireSoil = collision.gameObject;
        if (collision.gameObject.tag == "Soil" && collision.GetComponent<SoilController>().watered && speed != 2f)
            speed = 2f;

        if (collision.gameObject.tag == "Soil" && soil != collision.gameObject)
            soil = collision.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //stun code
        if ((playerOne && collision.gameObject.tag == "ThrownItem2") || (!playerOne && collision.gameObject.tag == "ThrownItem1"))
        {
            collision.GetComponent<Rigidbody2D>().velocity = gameManager.wind;
            collision.tag = "item";
            anim.SetBool("nutS", true);
            anim.Play("nutS");
            stunned = true;
            stunTimer = 2f;
            myRb.velocity = Vector2.zero;
            if (isHolding)
                Drop();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //when the player exits the item it flags a bool off and assigns holdingItem to null
        if (collision.gameObject.tag == "item" && !isHolding)
        {
            holdingItem = null;
            isTouching = false;
            pickup.text = "";
        }
        //when the player exits other item it flags a bool off and assigns otherItem to null
        else if (collision.gameObject == otherItem && isHolding)
        {
            otherItem = null;
            isTouchingOther = false;
            pickup.text = "Press " + playerpickupbutton + " To Drop or Hold To Throw";
        }
        if (collision.gameObject == fireSoil && fireSoil != null && fireTimer <= igniteTime)
        {
            fireTimer = 0;
            fireSoil = null;
        }

        if (collision.gameObject.tag == "Soil" && speed != 7f)
            speed = 7f;

        if (collision.gameObject.tag == "Soil" && soil != null)
            soil = null;
    }

    void DropSwap()
    {
        //if the player presses the assigned button from above and isnt touching anything else it activates the pickup/drop
        if (isTouching && holdingItem != null && !isTouchingOther && isHolding)
        {
            anim.Play("Drop");
            Drop();
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

    void Drop()
    {
        //sets the item the player was holding to have no parent object and make the player have no item it is holding
        holdingItem.transform.SetParent(null);
        holdingItem.GetComponent<Rigidbody2D>().velocity = gameManager.wind;
        itemName = "Hands";
        isHolding = false;
    }

    void Pickup()
    {
        //if the player presses the assigned button from above and isnt touching anything else it activates the pickup/drop
        if (holdingItem != null)
        {
            Vector3 pos = holdingItem.transform.position;
            if (isTouching && !isHolding && (pos.x - transform.position.x <= .7f && pos.y - transform.position.y <= .7f))
            {
                anim.Play("PickUp");
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
                pickup.text = "Press " + playerpickupbutton + " To Drop or Hold To Throw";
            }
        }
    }

    void Throw()
    {
        Debug.Log("Throw");
        holdingItem.transform.SetParent(null);
        if (myRb.velocity.x <= -throwSpeedMin || myRb.velocity.x >= throwSpeedMin || myRb.velocity.y <= -throwSpeedMin || myRb.velocity.y >= throwSpeedMin)
        {
            holdingItem.GetComponent<Rigidbody2D>().velocity = (myRb.velocity * 2);
            anim.Play("Throw");
            if (playerOne)
                holdingItem.tag = "ThrownItem1";
            else
                holdingItem.tag = "ThrownItem2";
            holdingItem = null;
        }
        itemName = "Hands";
        isHolding = false;
        pickup.text = "";
        showInteract = false;
    }
}
