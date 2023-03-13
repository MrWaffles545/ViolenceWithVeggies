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
    public bool paused;

    // Update is called once per frame
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
                gameTimeText.text = Mathf.RoundToInt(gameTime / 60) + " : " + (gameTime / 60);
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
