using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public int enemiesAlive = 0;

    public int round = 0;

    public GameObject[] spawnPoints;

    public GameObject enemyPrefab;

    public GameObject pauseMenu;

    public TextMeshProUGUI roundNum;
    public TextMeshProUGUI roundsSurvived;
    public GameObject endScreen;

    public Animator blackScreenAnimator;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        //healthNum.text = "Health " + player.health.ToString();
        if (enemiesAlive == 0) {
            round++;
            NextWave(round);
            roundNum.text = "Round: " + round.ToString();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Pause();

        }
    }

    public void NextWave(int round) {
        for (int i = 0; i < round; i++) {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject enemySpawned = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            enemySpawned.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
        }
    }

    public void EndGame() {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        endScreen.SetActive(true);
        roundsSurvived.text = round.ToString();
    }

    public void ReplayGame() {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
        round = 0;
    }

    public void MainMenu() {
        Time.timeScale = 1; 
        AudioListener.volume = 1;
        blackScreenAnimator.SetTrigger("FadeIn");
        Invoke("LoadMainMenuScene", .4f);
    }

    void LoadMainMenuScene() {
        SceneManager.LoadScene(0);
    }

    public void Pause() {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        AudioListener.volume = 0;
    }

    public void UnPause() {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.volume = 1;
    }
}
