using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText, scoreText2, gameTimeText;
    public GameObject player, player2;
    public GameObject endGameUI, pauseUI;
    public int score, score2;
    public float gameTime;
    public int weather;
    public float weatherTimer, weatherTimerMin, weatherTimerMax;
    public float weatherDuration;
    public bool weatherDone;
    public bool paused;

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            score = player.GetComponent<PlayerController>().score;
            scoreText.text = score.ToString();

            if (player2 != null)
            {
                score2 = player2.GetComponent<PlayerController>().score;
                scoreText2.text = score2.ToString();
            }

            if (gameTime >= 0)
            {
                gameTime -= Time.deltaTime;
                gameTimeText.text = (Mathf.RoundToInt(gameTime)).ToString();
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (paused)
                        Resume();
                    else
                        Pause();
                }
            }

            if (gameTime <= 0)
            {
                Time.timeScale = 0f;
                endGameUI.SetActive(true);
            }

            if (weatherTimer >= 0 && weather == 0)
                weatherTimer -= Time.deltaTime;

            if (weatherTimer <= 0 && weather == 0)
            {
                weather = Random.Range(1, 5); //1, 2, 3, and 4 (not 5)
                Debug.Log("Weather " + weather);
                //normal, angry sun, snow, rain, windy
                weatherTimer = Random.Range(weatherTimerMin, weatherTimerMax);
            }

            if (weather == 1) //angry sun
            {
                if (!weatherDone)
                {
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("Soil").Length / 2; i++)
                    {
                        SoilController target = GameObject.FindGameObjectsWithTag("Soil")[Random.Range(0, GameObject.FindGameObjectsWithTag("Soil").Length)].GetComponent<SoilController>();
                        target.watered = false;
                        target.fireSpreadDone = false;
                        target.onFire = true;
                    }
                    weatherDuration = 15f;
                    weatherDone = true;
                    Debug.Log("Angry sun");
                }
            }

            if (weather == 2) //Snow
            {
                if (!weatherDone)
                {
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("Soil").Length / 2; i++)
                    {
                        GameObject target = GameObject.FindGameObjectsWithTag("Soil")[Random.Range(0, GameObject.FindGameObjectsWithTag("Soil").Length)];
                        target.GetComponent<SoilController>().stage = -1;
                    }
                    weatherDuration = 15f;
                    weatherDone = true;
                    Debug.Log("Snow");
                }
            }

            if (weather == 3) //Rain
            {
                if (!weatherDone)
                {
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
                    weatherDuration = 15f;
                    weatherDone = true;
                    Debug.Log("Rain");
                }
            }

            if (weather == 4) //Windy
            {
                if (!weatherDone)
                {
                    float random = Random.Range(-3, 3);
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("item").Length; i++)
                    {
                        GameObject target = GameObject.FindGameObjectsWithTag("item")[i];
                        Vector2 temp = target.GetComponent<Rigidbody2D>().velocity;
                        temp.x = random;
                        temp.y = random;
                        if (target.transform.parent == null)
                            target.GetComponent<Rigidbody2D>().velocity = temp;
                    }
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
                    weather = 0;
                    weatherDone = false;
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
