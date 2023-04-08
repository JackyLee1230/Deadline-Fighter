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
        int highScoreRound =  SaveGame.Load<int>("round");
        int highScore = SaveGame.Load<int>("highScore");
        String highScoreTime = SaveGame.Load<String>("highScoreTime");
        Debug.Log("high score in main menu: " + highScore);
        if (highScore > 0 ){
            highScoreUI.text = "High Score: " + highScore + " In " + highScoreRound + " Rounds";
            highScoreTimeUI.text = "On: " + highScoreTime;
            // GUIUtility.systemCopyBuffer = "HighScore:" + highScore + " on " +  highScoreTime;
        }
        else{
            highScoreUI.text = "High Score: 0";
        }
    }

    public void copyToClipboard(){
        GUIUtility.systemCopyBuffer = "HighScore:" + highScoreUI.text + " on " +  highScoreTimeUI.text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
