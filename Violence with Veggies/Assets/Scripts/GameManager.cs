using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText, scoreText2;
    private GameObject box;
    public GameObject player, player2;
    public int score, score2;

    // Start is called before the first frame update
    void Start()
    {
        box = GameObject.Find("SellBox");
        scoreText.text = "0";
        
    }

    // Update is called once per frame
    void Update()
    {
        score = player.GetComponent<PlayerController>().score;
        scoreText.text = score.ToString();
        score2 = player2.GetComponent<PlayerController>().score;
        scoreText2.text = score2.ToString();
    }
}
