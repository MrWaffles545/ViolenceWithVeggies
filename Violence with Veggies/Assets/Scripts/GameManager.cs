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
                //normal, angry sun, snow, rain
                weatherTimer = Random.Range(weatherTimerMin, weatherTimerMax);
            }

            if (weather == 1) //angry sun
            {
                if (!weatherDone)
                {
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("Soil").Length / 2; i++)
                    {
                        GameObject target = GameObject.FindGameObjectsWithTag("Soil")[Random.Range(0, GameObject.FindGameObjectsWithTag("Soil").Length)];
                        target.GetComponent<SoilController>().watered = false;
                        target.GetComponent<SoilController>().fireSpreadDone = false;
                        target.GetComponent<SoilController>().onFire = true;
                        //target.SetActive(false);
                    }
                    weatherDone = true;
                }

                if (weatherDuration >= 0)
                {
                    weatherDuration -= Time.deltaTime;
                }

                if (weatherDuration <= 0)
                {
                    weather = 0;
                    weatherDone = false;
                    weatherDuration = 10f;
                }
                Debug.Log("FIRE!");
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
