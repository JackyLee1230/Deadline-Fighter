using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public class TabScript : MonoBehaviour
{

    [SerializeField] public bool isTabbed = false;

    [SerializeField] public GameObject tabMenuUI;

    [SerializeField] public GameObject pauseMenuUI;

    [SerializeField] public GameObject hitEffect;

    [SerializeField] public FirstPersonController fpsc;


    // Start is called before the first frame update
    void Start()
    {
        tabMenuUI.SetActive(false);
    }

    // Update is called once per frame
    public void Update()
    {
        // prevent pausing when dead
        if (Input.GetKeyDown(KeyCode.Tab) && fpsc.currentHealth > 0)
        {
            if (isTabbed)
            {
                Resume();
            }
            else if (!pauseMenuUI.activeInHierarchy)
            {
                Pause();
            }
        }
    }


    public void Resume()
    {
        tabMenuUI.SetActive(false);
        Cursor.visible = false;
        isTabbed = false;
        Time.timeScale = 1f;
        fpsc.enabled = true;
    }

    public void Pause()
    {
        tabMenuUI.SetActive(true);
        isTabbed = true;
        Time.timeScale = 0f;
        hitEffect.SetActive(false);
        fpsc.enabled = false;
        Debug.Log(((float)fpsc.shotsHit * 100 / fpsc.shotsFired).ToString());
        string accuracy = ((float)fpsc.shotsHit * 100 / fpsc.shotsFired).ToString("F1");
        if (fpsc.shotsFired == 0)
        {
            accuracy = "0.0";
        }
        tabMenuUI.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Score: " + fpsc.score + "\nSurvived: " + fpsc.timeSurvived.ToString("F1") + "s\nKills: " + fpsc.kills + "\nHeadshots: " + fpsc.headshots + "\nAccuracy: " + accuracy + "%\nDamage Dealt: " + fpsc.damageDealt.ToString("F1") + "HP";
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
