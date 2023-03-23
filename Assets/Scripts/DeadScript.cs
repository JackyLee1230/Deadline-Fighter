using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class DeadScript : MonoBehaviour
{

    [SerializeField] public bool isEnded = false;

    [SerializeField] public GameObject deadMenuUI;
    [SerializeField] public FirstPersonController fpsc;


    // Start is called before the first frame update
    void Start()
    {
        deadMenuUI.SetActive(false);
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
