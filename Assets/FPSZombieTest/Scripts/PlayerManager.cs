using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerManager : MonoBehaviour {
    public float health = 100;
    public TextMeshProUGUI healthNum;
    public GameManager gameManager;
    public GameObject playerCamera;
    public CanvasGroup hurtPanel;
    private float shakeTime;
    private float shakeDuration;
    private Quaternion playerCameraOriginalRotation;

    // Start is called before the first frame update
    void Start() {
        playerCameraOriginalRotation = playerCamera.transform.localRotation;
    }

    // Update is called once per frame
    void Update() {
        if(hurtPanel.alpha > 0) {
            hurtPanel.alpha -= Time.deltaTime;
        }
        if (shakeTime < shakeDuration) {
            shakeTime += Time.deltaTime;
            CameraShake();
        } else if(playerCamera.transform.localRotation != playerCameraOriginalRotation) {
            playerCamera.transform.localRotation = playerCameraOriginalRotation;
        }
    }

    public void Hit(float damage) {
        health -= damage;
        healthNum.text = health.ToString() + " Health ";
        if (health <= 0) {
            gameManager.EndGame();
        } else {
            shakeTime = 0;
            shakeDuration = .2f;
            hurtPanel.alpha = .7f;
        }
    }

    public void CameraShake() {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2, 2), 0, 0);
    }
}
