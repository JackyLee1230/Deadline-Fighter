using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class EnemySpawner : MonoBehaviour

{
    [SerializeField]
    public int enemiesAlive = 0;

    [SerializeField] List<float> timeTook;

    [SerializeField] public float currentRoundTime;

    [SerializeField]
    public int round = 0;

    [SerializeField]
    public GameObject[] spawnPoints;

    [SerializeField]
    public float minDistanceToSpawn = 10;

    [SerializeField] public FirstPersonController fpsc;

    [SerializeField]
    public GameObject[] enemyPrefabs;

    [SerializeField]
    public GameObject[] enemies;

    [SerializeField]
    public AudioClip roundWin; //test

    public TextMeshProUGUI roundNum;
    // Start is called before the first frame update
    void Start()
    {
        timeTook = new List<float>();
    }

    // Update is called once per frame
    void Update()
    {
        currentRoundTime += Time.deltaTime;
        for (int i =0; i < enemies.Length; i++) {
            if (enemies[i] == null || enemies[i].GetComponentInChildren<AIExample>().isDead == true) {
                enemiesAlive = enemies.Length - 1;
                enemies[i] = enemies[enemies.Length - 1];
                // finally, let's decrement Array's size by one
                Array.Resize(ref enemies, enemies.Length - 1);
            }
        }
        roundNum.text = "Round: " + round.ToString() + " Time:" + currentRoundTime.ToString("F0") + "s; Alive: " + enemiesAlive.ToString();
        if (enemiesAlive == 0) {
            // add a random number of score  between (round * 100) to (round * 200 ) to fpsc.score
            fpsc.score += UnityEngine.Random.Range(round * 100, round * 200);

            round++;
            nextWave(round);
        roundNum.text = "Round: " + round.ToString() + " Time:" + currentRoundTime.ToString("F0") + "s; Alive: " + enemiesAlive.ToString();
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            // destroy all enemies in enemies
            for (int i = 0; i < enemies.Length; i++) {
                Destroy(enemies[i]);
            }
            round++;
            nextWave(round);
        roundNum.text = "Round: " + round.ToString() + " Time:" + currentRoundTime.ToString("F0") + "s; Alive: " + enemiesAlive.ToString();
        }
    }

    public void nextWave(int round) {
        // add the time it took to complete the round to the list
        timeTook.Add(currentRoundTime);
        currentRoundTime = 0.0f;
        // create a new array of enemies
        AudioSource.PlayClipAtPoint(roundWin, transform.position, 1.5f);
        int spwanCount = UnityEngine.Random.Range(round, round * 10);
        enemies = new GameObject[spwanCount];
        Debug.Log("round " + round + " spwanCount " + spwanCount);
        for (int i = 0; i < spwanCount; i++) {
            GameObject spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            while(Vector3.Distance(fpsc.transform.position, spawnPoint.transform.position) < minDistanceToSpawn) {
                spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            }

            GameObject enemySpawned = Instantiate(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)], spawnPoint.transform.position, Quaternion.identity);
            enemySpawned.GetComponentInChildren<AIExample>().fpsc = fpsc;
            //add the enemy to the enemies array
            enemies[i] = enemySpawned;
            enemiesAlive++;
        }
    }
}
