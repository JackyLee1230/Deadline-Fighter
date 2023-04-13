using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;
using TMPro;
using BayatGames.SaveGameFree;

public class HighScoreMainMenu : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI highScoreUI;
    [SerializeField] public TextMeshProUGUI highScoreTimeUI;
    [SerializeField] public TextMeshProUGUI resumeUI;

    [SerializeField] public GameObject continueButton;


    // Start is called before the first frame update
    void Start()
    {
        highScoreUI = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        highScoreTimeUI = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        resumeUI = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        int highScoreRound = SaveGame.Load<int>("round");
        int highScore = SaveGame.Load<int>("highScore");
        String highScoreTime = SaveGame.Load<String>("highScoreTime");
        Debug.Log("high score in main menu: " + highScore);
        if (highScore > 0)
        {
            highScoreUI.text = "High Score: " + highScore + " In " + highScoreRound + " Rounds";
            highScoreTimeUI.text = "On: " + highScoreTime;
            // GUIUtility.systemCopyBuffer = "HighScore:" + highScore + " on " +  highScoreTime;
        }
        else
        {
            highScoreUI.text = "High Score: 0";
        }

        if (SaveGame.Load<int>("round") > 0)
        {
            resumeUI.text = "Continue: You left on Round " + SaveGame.Load<int>("round");
        }
        else
        {
            resumeUI.text = "";
            continueButton.SetActive(false);
        }
    }

    public void copyToClipboard()
    {
        GUIUtility.systemCopyBuffer = "HighScore:" + highScoreUI.text + " on " + highScoreTimeUI.text;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
