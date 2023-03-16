using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //UI variables
    public TextMeshProUGUI scoreText, scoreText2, gameTimeText;
    public GameObject player, player2;
    public GameObject endGameUI, pauseUI;

    //Game Variables
    public int score, score2;
    public float gameTime;

    //Weather variables
    public int weather;
    public float weatherTimer, weatherTimerMin, weatherTimerMax;
    public float weatherDuration;
    public bool weatherDone;
    public Vector2 wind;

    //Pause variable
    public bool paused;

    void Update()
    {
        //Works if it isnt the menu scene
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            //Changes the score UI accordingly
            score = player.GetComponent<PlayerController>().score;
            scoreText.text = score.ToString();

            //Player 2 Score UI code
            if (player2 != null)
            {
                score2 = player2.GetComponent<PlayerController>().score;
                scoreText2.text = score2.ToString();
            }

            //Game time code
            if (gameTime >= 0)
            {
                gameTime -= Time.deltaTime;
                gameTimeText.text = (Mathf.RoundToInt(gameTime)).ToString();
                //Pause and resume
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (paused)
                        Resume();
                    else
                        Pause();
                }
            }

            //Ends the game if the time runs out
            if (gameTime <= 0)
            {
                Time.timeScale = 0f;
                endGameUI.SetActive(true);
            }

            //Weather timer until next weather event
            if (weatherTimer >= 0 && weather == 0)
                weatherTimer -= Time.deltaTime;

            //When the weather timer runs out it picks a random weather event and random time when the next one starts
            if (weatherTimer <= 0 && weather == 0)
            {
                weather = Random.Range(1, 5); //1, 2, 3, and 4 (not 5)
                Debug.Log("Weather " + weather);
                //normal, angry sun, snow, rain, windy
                weatherTimer = Random.Range(weatherTimerMin, weatherTimerMax);
            }

            if (weather == 1) //angry sun
            {
                //only runs once
                if (!weatherDone)
                {
                    //picks random soil and lights them on fire
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("Soil").Length / 4; i++)
                    {
                        SoilController target = GameObject.FindGameObjectsWithTag("Soil")[Random.Range(0, GameObject.FindGameObjectsWithTag("Soil").Length)].GetComponent<SoilController>();
                        target.watered = false;
                        target.fireSpreadDone = false;
                        target.onFire = true;
                    }
                    //sets the weather duration
                    weatherDuration = 15f;
                    weatherDone = true;
                    Debug.Log("Angry sun");
                }
            }

            if (weather == 2) //Snow
            {
                //only runs once
                if (!weatherDone)
                {
                    //picks random soil and covers them with snow
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("Soil").Length / 3; i++)
                    {
                        GameObject target = GameObject.FindGameObjectsWithTag("Soil")[Random.Range(0, GameObject.FindGameObjectsWithTag("Soil").Length)];
                        target.GetComponent<SoilController>().stage = -1;
                    }
                    //sets the weather duration
                    weatherDuration = 15f;
                    weatherDone = true;
                    Debug.Log("Snow");
                }
            }

            if (weather == 3) //Rain
            {
                //only runs once
                if (!weatherDone)
                {
                    //waters all the soil or puts out fire then waters after a while
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("Soil").Length; i++)
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
                    Debug.Log("Rain");
                }
            }

            if (weather == 4) //Windy
            {
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
}
