using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    public int enemiesAlive = 0;

    [SerializeField]
    public int round = 0;

    [SerializeField]
    public GameObject[] spawnPoints;

    [SerializeField] public FirstPersonController fpsc;

    [SerializeField]
    public GameObject[] enemyPrefabs;

    public GameObject[] enemies;

    public TextMeshProUGUI roundNum;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesAlive == 0) {
            round++;
            nextWave(round);
            roundNum.text = "Round: " + round.ToString();
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            // destroy all enemies in enemies
            for (int i = 0; i < enemies.Length; i++) {
                Destroy(enemies[i]);
            }
            round++;
            nextWave(round);
            roundNum.text = "Round: " + round.ToString();
        }
    }

    public void nextWave(int round) {
        // create a new array of enemies
        enemies = new GameObject[round];
        for (int i = 0; i < round; i++) {
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject enemySpawned = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPoint.transform.position, Quaternion.identity);
            enemySpawned.GetComponentInChildren<AIExample>().fpsc = fpsc;
            //add the enemy to the enemies array
            enemies[i] = enemySpawned;
            enemiesAlive++;
        }
    }
}
