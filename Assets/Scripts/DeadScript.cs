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
        if(fpsc.currentHealth <= 0f) {
            deadMenuUI.SetActive(true);
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }

    public void MainMenu(){
        Time.timeScale = 1f;
        Cursor.visible = true;
        SceneManager.LoadScene("MenuScene");
    }

    public void Quit(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
