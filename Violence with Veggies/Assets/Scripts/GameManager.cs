using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private GameObject box;
    public int score;
    private Component controllerScrpt;

    // Start is called before the first frame update
    void Start()
    {
        box = GameObject.Find("SellBox");
        scoreText.text = "0";
        
    }

    // Update is called once per frame
    void Update()
    {
        score = box.GetComponent<BoxController>().veggieScore;
        scoreText.text = score.ToString();
    }
}
