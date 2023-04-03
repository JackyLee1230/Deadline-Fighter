using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

using BayatGames.SaveGameFree; 

public class DeadScript : MonoBehaviour
{

    [SerializeField] public bool isEnded = false;

    [SerializeField] public GameObject deadMenuUI;
    [SerializeField] public FirstPersonController fpsc;

    [SerializeField] public TextMeshProUGUI scoreUI;
    [SerializeField] public TextMeshProUGUI highScoreUI;


    // Start is called before the first frame update
    void Start()
    {
        deadMenuUI.SetActive(false);
        scoreUI = deadMenuUI.transform.Find("DeadMenu").Find("Score").GetComponent<TextMeshProUGUI>();
        highScoreUI = deadMenuUI.transform.Find("DeadMenu").Find("HighScore").GetComponent<TextMeshProUGUI>();
        
    }

    // Update is called once per frame
    public void Update()
    {
        if(isEnded == false && fpsc.currentHealth <= 0f) {
            End();
        }
    }

    public void End(){
        deadMenuUI.SetActive(true);
        Cursor.visible = true;
        Time.timeScale = 0f;
        Screen.lockCursor = false;
        Cursor.lockState = CursorLockMode.None;
        isEnded = true;
        fpsc.enabled = false;
        scoreUI.text = "Score: " + fpsc.score;
        int highScore = SaveGame.Load<int>("highScore");
        if (fpsc.score > highScore){
            SaveGame.Save<int>("highScore", fpsc.score);
        }
        if (highScore == 0){
            highScoreUI.text = "Not Bad! First Attemp\nHigh Score: " + fpsc.score;
        } else {
            if (fpsc.score > highScore){
                highScoreUI.text = "New High Score: " + fpsc.score;
                scoreUI.text = "";
            } else {
                highScoreUI.text = "High Score: " + highScore;
            }
        }
    }

    public void MainMenu(){
        Time.timeScale = 1f;
        Cursor.visible = true;
        SceneManager.LoadScene("MenuScene");
    }

    public void Restart(){
        Time.timeScale = 1f;
        Cursor.visible = true;
        SceneManager.LoadScene("Scene");
    }

    public void Quit(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
