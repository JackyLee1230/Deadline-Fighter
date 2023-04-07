using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree; 
using TMPro;

public class HighScoreMainMenu : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI highScoreUI;
    [SerializeField] public TextMeshProUGUI highScoreTimeUI;


    // Start is called before the first frame update
    void Start()
    {
        highScoreUI = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        highScoreTimeUI = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        int highScore = SaveGame.Load<int>("highScore");
        String highScoreTime = SaveGame.Load<String>("highScoreTime");
        Debug.Log("high score in main menu: " + highScore);
        if (highScore > 0 ){
            highScoreUI.text = "High Score: " + highScore;
            highScoreTimeUI.text = "On: " + highScoreTime;
        }
        else{
            highScoreUI.text = "High Score: 0";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
