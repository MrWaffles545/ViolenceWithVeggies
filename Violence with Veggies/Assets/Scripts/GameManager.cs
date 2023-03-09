using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText, scoreText2, gameTimeText;
    private GameObject box;
    public GameObject player, player2;
    public GameObject endGameUI;
    public int score, score2;
    public float gameTimer, gameTime;

    // Start is called before the first frame update
    void Start()
    {
        box = GameObject.Find("SellBox");
        scoreText.text = "0";
        
    }

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

            if (gameTimer <= gameTime)
            {
                gameTimer += Time.deltaTime;
                gameTimeText.text = Mathf.RoundToInt(gameTimer).ToString();
            }

            if (gameTimer >= gameTime)
            {
                Time.timeScale = 0;
                endGameUI.SetActive(true);
            }
        }
    }

    public void LoadLevel(int levelNumber)
    {
        SceneManager.LoadScene(levelNumber);
    }
}
