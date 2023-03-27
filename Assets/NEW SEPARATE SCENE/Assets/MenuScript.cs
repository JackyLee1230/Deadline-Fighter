using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuScript : MonoBehaviour
{
    public GameObject pauseCanvas;
    bool isGamePaused;
    public GameObject gameOverCanvas;
    bool isGameOver;
    public TextMeshProUGUI scoreText;
    public int score;
    // Start is called before the first frame update 

    void Start()
    {

    }
    // Update is called once per frame 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver )
            PauseGame();
    }

    public void PauseGame()
    {
        if (!isGamePaused)
        {
            isGamePaused = true;
            pauseCanvas.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;                                       
        }
        else
        {
            isGamePaused = false;
            pauseCanvas.SetActive(false);
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
    }

    public void GameOver()
    {

        Time.timeScale = 0;
        isGameOver = true;
        
        scoreText.text = "Score" + score;
        gameOverCanvas.SetActive(true);
    }

    public void RestartGame()
    {
        string _roomName = SceneManager.GetActiveScene().name;
        Time.timeScale = 1;
        SceneManager.LoadScene(_roomName);

    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        Application.Quit();
    }

}