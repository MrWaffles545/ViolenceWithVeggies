using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    //UI variables
    public TextMeshProUGUI scoreText, scoreText2, gameTimeText, highscore, startTimerText, playerWin;
    public GameObject player, player2;
    public GameObject endGameUI, pauseUI;
    public Image[] boxs;
    public Sprite full;

    //Game Variables
    public int score, score2;
    public float gameTime;

    //start timer
    public float startTimer;

    //can use input
    public float menuTimer;
    public bool canInput;
    public InputAction joystick, pauseButton, selectButton, backButton;

    //Weather variables
    public int weather;
    public float weatherTimer, weatherTimerMin, weatherTimerMax;
    public float weatherDuration;
    public bool weatherDone, rainSecond;
    public Vector2 wind;

    public int warning;
    public GameObject warningSign;

    //Menu Variables
    public bool menuSelect, gameSelect, selection;
    public GameObject menuButtons, gameButtons, tutorialButtons;
    public GameObject game, tutorial, player1Select, player2Select;
    public Image tutorialImage;
    public TextMeshProUGUI highscoreText, highscoreText2;
    public int menuStage;

    //Pause variable
    public bool paused, pauseHold = false;
    public float pauseTimer;

    //the normal yellow sun
    public GameObject sunPrime;

    //where da sun go
    public GameObject sunPosUp;

    //sun tranisition speed
    public float tanSpeed;

    //da suns sprite renderer
    public SpriteRenderer sunRender;

    //the differt suns
    public Sprite[] daSuns;

    //sounds
    public AudioSource bloop, regMus, firMus, winMus, ranMus, snoMus, pop, menMus;

    //music or not to music that is the question
    public bool musOn;

    //weather overlays and effects
    public GameObject wetOver, heatOver, windOver, snowOver;
    public GameObject rainFall, snowFall;
    private Rigidbody2D rainBody, snowBody;

    public float step;

    public void OnEnable()
    {
        joystick.Enable();
        pauseButton.Enable();
        selectButton.Enable();
        backButton.Enable();
    }
    
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            rainBody = rainFall.GetComponent<Rigidbody2D>();

            snowBody = snowFall.GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if(!menMus.isPlaying)
                menMus.Play();

            if (highscoreText.text != "" + PlayerPrefs.GetInt("HighScore"))
                highscoreText.text = "" + PlayerPrefs.GetInt("HighScore");
            if (highscoreText2.text != "" + PlayerPrefs.GetInt("HighScore2"))
                highscoreText2.text = "" + PlayerPrefs.GetInt("HighScore2");

            //if (!canInput && menuTimer >= 0)
            //menuTimer -= Time.deltaTime;
            //else if (!canInput && menuTimer <= 0)
            //canInput = true;
            if (selection)
            {
                game.SetActive(false);
                player1Select.SetActive(false);
                tutorial.SetActive(true);
                player2Select.SetActive(true);
            }
            else
            {
                game.SetActive(true);
                player1Select.SetActive(true);
                tutorial.SetActive(false);
                player2Select.SetActive(false);
            }
            if (canInput)
            {
                if (selection && joystick.ReadValue<Vector2>().y <= -.25f)
                    selection = false;
                if (!selection && joystick.ReadValue<Vector2>().y >= .25f)
                    selection = true;

                if (menuStage == 1)
                {
                    if (selection && selectButton.WasPressedThisFrame())
                        LoadLevel(1);
                    if (!selection && selectButton.WasPressedThisFrame())
                        LoadLevel(2);
                    if (backButton.WasPressedThisFrame())
                        MenuSelect();
                }

                if (menuStage == 0)
                {
                    if (selection && selectButton.WasPressedThisFrame())
                        GameSelect();
                    if (!selection && selectButton.WasPressedThisFrame())
                        Tutorial();
                }

                if (menuStage == 2)
                {
                    tutorialImage.rectTransform.position += new Vector3(joystick.ReadValue<Vector2>().x, 0, 0);
                    if (backButton.WasPressedThisFrame())
                        MenuSelect();
                }
            }
        }
        //Works if it isnt the menu scene
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (GameObject.FindGameObjectsWithTag("item").Length > 25)
                Destroy(GameObject.FindGameObjectsWithTag("item")[3]);

            //Changes the score UI accordingly
            score = player.GetComponent<PlayerController>().score;
            scoreText.text = "Player 1 score: " + score;

            //Player 2 Score UI code
            if (player2 != null)
            {
                score2 = player2.GetComponent<PlayerController>().score;
                scoreText2.text = "Player 2 score: " + score2;
            }

            //time before game start
            if (startTimer >= 0)
            {
                startTimer -= Time.deltaTime;
                startTimerText.text = (Mathf.RoundToInt(startTimer)).ToString();
                canInput = false;
            }

            //after timer before game ends
            if (startTimer <= 0)
            {
                canInput = true;
                startTimerText.enabled = false;
            }

            //Game time code
            if (gameTime >= 0 && canInput)
            {
                gameTime -= Time.deltaTime;
                gameTimeText.text = ""+(Mathf.RoundToInt(gameTime));
                //Pause and resume
                if (selectButton.WasPressedThisFrame() && paused)
                    LoadLevel(0);
                if (backButton.WasPressedThisFrame() && paused)
                    Resume();
                if (pauseButton.WasPressedThisFrame())
                {
                    pauseTimer = 4f;
                    pauseHold = true;
                }
                if (pauseTimer >= 0 && pauseHold)
                    pauseTimer -= Time.deltaTime;
                if (pauseButton.WasReleasedThisFrame() && pauseHold)
                {
                    pauseHold = false;
                    pauseTimer = -.1f;
                }
                if (pauseTimer <= 0 && pauseHold)
                {
                    if (paused)
                        Resume();
                    else
                        Pause();
                    pauseHold = false;
                    pauseTimer = -.1f;
                }
            }

            //Ends the game if the time runs out
            if (gameTime <= 0)
            {
                Time.timeScale = 0f;
                endGameUI.SetActive(true);
                if (player2 != null)
                {
                    if (score > PlayerPrefs.GetInt("HighScore2"))
                        PlayerPrefs.SetInt("HighScore2", score);
                    if (score2 > PlayerPrefs.GetInt("HighScore2"))
                        PlayerPrefs.SetInt("HighScore2", score2);
                    highscore.text = "High Score: " + PlayerPrefs.GetInt("HighScore2");
                    if (score > score2)
                    {
                        playerWin.text = "Player 1 Wins!";
                        boxs[0].sprite = full;
                    }
                    if (score2 > score)
                    {
                        playerWin.text = "Player 2 Wins!";
                        boxs[1].sprite = full;
                    }
                    if (score == score2)
                        playerWin.text = "Tie!";
                }
                else
                {
                    if (score > PlayerPrefs.GetInt("HighScore"))
                        PlayerPrefs.SetInt("HighScore", score);
                    highscore.text = "High Score: " + PlayerPrefs.GetInt("HighScore");
                    playerWin.text = "Score: " + score;
                }
                if (selectButton.WasPressedThisFrame())
                    LoadLevel(0);
            }


        if (weather == 0)
                heatOver.SetActive(false);

        if (weather == 0)
                snowOver.SetActive(false);

        if (weather == 0)
                windOver.SetActive(false);

        if (weather == 0)
                wetOver.SetActive(false);

        if (weather == 0 || weather == 1 || weather == 2 || weather == 4)
                rainFall.transform.position = new Vector2(0, 29);

        if (weather == 0 || weather == 1 || weather == 3|| weather == 4)
                snowFall.transform.position = new Vector2(0, 29);



            if (startTimer >= 0f)
            {
                if (!pop.isPlaying && startTimer <= 0.5f)
                    pop.Play();
            }


            if (startTimer >= 1)
            {
                //bloop sound on the game start countdown
                if (!bloop.isPlaying && startTimer <= 5)
                    bloop.Play();
            }

            //Weather timer until next weather event
            if (weatherTimer >= 0 && weather == 0 && canInput)
            {
                weatherTimer -= Time.deltaTime;

                if (weatherTimer <= 2)
                {
                    if (warning == 0)
                    {
                        warning = Random.Range(1, 5);
                        GameObject sign = Instantiate(warningSign, new Vector2(9.997f, 3), Quaternion.identity);
                        sign.GetComponent<Rigidbody2D>().velocity = Vector2.left * 14;
                        Destroy(sign, 2f);

                        sunPosUp.transform.position = new Vector2(0, 4.3f);
                    }

                    //Debug.Log("warning weather " + warning);
                }
            }

            //When the weather timer runs out it picks a random weather event and random time when the next one starts
            if (weatherTimer <= 0 && weather == 0)
            {
                    weather = warning; //1, 2, 3, and 4 (not 5)
                    warning = 0;
                //normal, angry sun, snow, rain, windy
                    weatherTimer = Random.Range(weatherTimerMin, weatherTimerMax);
                    
            }

            if (weather == 1) //angry sun
            {
                if (weather == 1)
                    heatOver.SetActive(true);

                step = tanSpeed * Time.deltaTime;
                sunPrime.transform.position = Vector3.MoveTowards(sunPrime.transform.position, sunPosUp.transform.position, step);
                if (!firMus.isPlaying && musOn == true)
                    firMus.Play();


                    //only runs once
                    if (!weatherDone)
                    {
                    

                    //picks random soil and lights them on fire
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("Soil").Length / 3; i++)
                    {
                        SoilController target = GameObject.FindGameObjectsWithTag("Soil")[Random.Range(0, GameObject.FindGameObjectsWithTag("Soil").Length)].GetComponent<SoilController>();
                        target.watered = false;
                        target.fireSpreadDone = false;
                        target.onFire = true;
                        if (target.GetComponent<SoilController>().stage == 2)
                            target.GetComponent<SoilController>().stage = 1;
                        target.GetComponent<SoilController>().cropTime = 0f;
                        target.GetComponent<SoilController>().bar.GetComponent<Transform>().localScale = new Vector2(0, .15f);
                    }
                    //sets the weather duration
                    weatherDuration = 15f;
                    weatherDone = true;
                    Debug.Log("Angry sun");
                }
            }

            sunPrime.transform.position = Vector3.MoveTowards(sunPrime.transform.position, sunPosUp.transform.position, step);

            if (sunPrime.transform.position.y >= 4.3f && sunRender.sprite != daSuns[weather])
            {
                sunPosUp.transform.position = new Vector2(0, 2.814f);
                sunRender.sprite = daSuns[weather];
            }

            if (weather == 2) //Snow
            {
                if (weather == 2)
                    snowBody.velocity = new Vector2(0f, -4f);

                if (weather == 2)
                    snowOver.SetActive(true);

                step = tanSpeed * Time.deltaTime;
                sunPrime.transform.position = Vector3.MoveTowards(sunPrime.transform.position, sunPosUp.transform.position, step);
                if (!snoMus.isPlaying && musOn == true)
                    snoMus.Play();

                //only runs once
                if (!weatherDone)
                {
                    //picks random soil and covers them with snow
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("Soil").Length / 2; i++)
                    {
                        GameObject target = GameObject.FindGameObjectsWithTag("Soil")[Random.Range(0, GameObject.FindGameObjectsWithTag("Soil").Length)];
                        target.GetComponent<SoilController>().stage = -1;
                        target.GetComponent<SoilController>().cropTime = 0f;
                        target.GetComponent<SoilController>().bar.GetComponent<Transform>().localScale = new Vector2(0, .15f);
                    }
                    //sets the weather duration
                    weatherDuration = 15f;
                    weatherDone = true;
                    Debug.Log("Snow");
                }
            }

            if (weather == 3) //Rain
            {
                if (weather == 3)
                    rainBody.velocity = new Vector2(0f, -4f);

                if (weather == 3)
                    wetOver.SetActive(true);

                if (!ranMus.isPlaying && musOn == true)
                    ranMus.Play();

                step = tanSpeed * Time.deltaTime;
                sunPrime.transform.position = Vector3.MoveTowards(sunPrime.transform.position, sunPosUp.transform.position, step);
                //only runs once
                if (!weatherDone)
                {

                    //waters all the soil or puts out fire then waters after a while
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("Soil").Length / 2; i++)
                    {
                        SoilController target = GameObject.FindGameObjectsWithTag("Soil")[i].GetComponent<SoilController>();
                        if (target.onFire)
                        {
                            target.onFire = false;
                            target.rain = true;
                        }
                        else
                            target.watered = true;
                    }
                    //sets the weather duration
                    weatherDuration = 15f;
                    weatherDone = true;
                    rainSecond = false;
                    Debug.Log("Rain");
                }
                if (weatherDuration <= 5 && !rainSecond)
                {
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("Soil").Length / 2; i++)
                    {
                        SoilController target = GameObject.FindGameObjectsWithTag("Soil")[i].GetComponent<SoilController>();
                        if (target.onFire)
                        {
                            target.onFire = false;
                            target.rain = true;
                        }
                        else
                            target.watered = true;
                    }
                    rainSecond = true;
                }
            }

            if (weather == 4) //Windy
            {
                if (weather == 4)
                    windOver.SetActive(true);

                step = tanSpeed * Time.deltaTime;
                sunPrime.transform.position = Vector3.MoveTowards(sunPrime.transform.position, sunPosUp.transform.position, step);
                if (!winMus.isPlaying && musOn == true)
                    winMus.Play();

                //only runs once
                if (!weatherDone)
                {
                    //picks a random direction and magnitude to push the items
                    wind.x = Random.Range(-3, 3);
                    wind.y = Random.Range(-3, 3);
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("item").Length; i++)
                    {
                        GameObject target = GameObject.FindGameObjectsWithTag("item")[i];
                        //applies the wind if they arent being held
                        if (target.transform.parent == null)
                            target.GetComponent<Rigidbody2D>().velocity = wind;
                    }
                    //sets the weather duration
                    weatherDuration = 15f;
                    weatherDone = true;
                    Debug.Log("Windy");
                }
            }

            if(weather == 0)
            {
                if (!regMus.isPlaying && musOn == true && weatherTimer <= 29.99f)
                    regMus.Play();
            }

            if (weather > 0) //Resets the weather to normal when the weather duration timer ends
            {

                if (weatherDuration >= 0)
                {
                    weatherDuration -= Time.deltaTime;
                }

                if (weatherDuration <= 0)
                {
                    //Turns all the weather stuff off for the items and soil
                    weather = 0;
                    weatherDone = false;
                    wind = Vector2.zero;
                    sunPosUp.transform.position = new Vector2(0, 4.3f);
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("Soil").Length; i++)
                    {
                        SoilController target = GameObject.FindGameObjectsWithTag("Soil")[i].GetComponent<SoilController>();
                        target.onFire = false;
                        target.fireSpreadDone = false;
                        target.rain = false;
                        if (target.stage == -1)
                            target.stage = 0;
                    }
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("item").Length; i++)
                    {
                        GameObject target = GameObject.FindGameObjectsWithTag("item")[i];
                        Vector2 temp = target.GetComponent<Rigidbody2D>().velocity;
                        temp.x = 0;
                        temp.y = 0;
                        target.GetComponent<Rigidbody2D>().velocity = temp;
                    }
                }
            }
        }
    }

    public void LoadLevel(int levelNumber)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelNumber);
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
    }

    void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
    }

    public void SetHighscore(int number)
    {
        PlayerPrefs.SetInt("HighScore", number);
    }

    public void MenuSelect()
    {
        menuButtons.SetActive(true);
        gameButtons.SetActive(false);
        tutorialButtons.SetActive(false);
        menuStage = 0;
        menuTimer = .1f;
        //canInput = false;
        selection = true;
    }

    public void GameSelect()
    {
        menuButtons.SetActive(false);
        gameButtons.SetActive(true);
        tutorialButtons.SetActive(false);
        menuStage = 1;
        menuTimer = .1f;
        //canInput = false;
        selection = true;
    }

    public void Tutorial()
    {
        menuButtons.SetActive(false);
        gameButtons.SetActive(false);
        tutorialButtons.SetActive(true);
        menuStage = 2;
        menuTimer = .1f;
        //canInput = false;
        selection = true;
    }
}
