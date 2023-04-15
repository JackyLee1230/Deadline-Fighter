using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public class PauseScript : MonoBehaviour
{

    [SerializeField] public bool isPaused = false;

    [SerializeField] public GameObject pauseMenuUI;

    [SerializeField] public GameObject tabMenuUI;

    [SerializeField] public GameObject hitEffect;

    [SerializeField] public FirstPersonController fpsc;


    // Start is called before the first frame update
    void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    // Update is called once per frame
    public void Update()
    {
        // prevent pausing when dead
        if (Input.GetKeyDown(KeyCode.Escape) && fpsc.currentHealth > 0)
        {
            if (isPaused)
            {
                Resume();
            }
            else if (!tabMenuUI.activeInHierarchy)
            {
                Pause();
            }
        }
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        SceneManager.LoadScene("MenuScene");
        AudioListener.volume = 1;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Cursor.visible = false;
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.volume = 1;
        fpsc.enabled = true;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Cursor.visible = true;
        isPaused = true;
        Time.timeScale = 0f;
        hitEffect.SetActive(false);
        fpsc.enabled = false;
        AudioListener.volume = 0;
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
