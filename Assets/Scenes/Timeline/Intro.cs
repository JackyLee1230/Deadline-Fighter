using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public float changeTime;
    public string sceneName;

    private void Update()
    {
        changeTime -= Time.deltaTime;
        if ((changeTime <= 0) || Input.GetKeyDown("space"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
