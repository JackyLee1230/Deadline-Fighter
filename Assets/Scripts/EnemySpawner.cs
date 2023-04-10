using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using BayatGames.SaveGameFree;

public class EnemySpawner : MonoBehaviour

{
    [SerializeField]
    public int enemiesAlive = 0;

    [SerializeField] public float timeBetweenWaves = 5.0f;

    [SerializeField] public bool isTransitioning;

    [SerializeField] public float transitionTime;

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
        isTransitioning = false;
    }

    // Update is called once per frame
    void Update()
    {

        currentRoundTime += Time.deltaTime;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null || enemies[i].GetComponentInChildren<AIExample>().isDead == true)
            {
                enemiesAlive = enemies.Length - 1;
                enemies[i] = enemies[enemies.Length - 1];
                Array.Resize(ref enemies, enemies.Length - 1);
            }
        }
        if (!isTransitioning)
        {
            transitionTime = 0.0f;
            roundNum.text = "Round: " + round.ToString() + " Time:" + currentRoundTime.ToString("F0") + "s; Alive: " + enemiesAlive.ToString();
        }
        else
        {
            transitionTime += Time.deltaTime;
            roundNum.text = "Round: " + round.ToString() + " Incoming in" + (5.0f - transitionTime).ToString("F1") + "s !";
        }

        if (enemiesAlive == 0 && isTransitioning == false)
        {
            // add a random number of score  between (round * 100) to (round * 200 ) to fpsc.score
            fpsc.score += UnityEngine.Random.Range(round * 100, round * 200);
            round++;
            // nextWave(round);
            if (round != 1)
            {
                StartCoroutine(waitAndSpawnNextWave());
            }
            else
            {
                nextWave(round);
            }

        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // destroy all enemies in enemies
            for (int i = 0; i < enemies.Length; i++)
            {
                Destroy(enemies[i]);
            }
            round++;
            nextWave(round);
            roundNum.text = "Round: " + round.ToString() + " Time:" + currentRoundTime.ToString("F0") + "s; Alive: " + enemiesAlive.ToString();
        }
#else
            Debug.Log("Skipping round only availble in editor mode");
#endif
    }

    IEnumerator waitAndSpawnNextWave()
    {
        isTransitioning = true;
        yield return new WaitForSeconds(timeBetweenWaves);
        roundNum.text = "Round: " + round.ToString() + " Time:" + currentRoundTime.ToString("F0") + "s; Alive: " + enemiesAlive.ToString();
        // nextWave(round);
        fpsc.GetComponent<FirstPersonController>().currentHealth = fpsc.maxHealth;
        // remove all enemies from the enemies array and the scene
        for (int i = 0; i < enemies.Length; i++)
        {
            Destroy(enemies[i]);
        }
        // reset the enemy count
        enemiesAlive = 0;
        // add the time it took to complete the round to the list
        timeTook.Add(currentRoundTime);
        currentRoundTime = 0.0f;
        // create a new array of enemies
        AudioSource.PlayClipAtPoint(roundWin, transform.position, 1.5f);
        int spwanCount = UnityEngine.Random.Range(round, round * 3);

        // save the data for load
        SaveGame.Save<int>("round", round);
        SaveGame.Save<int>("score", fpsc.GetComponent<FirstPersonController>().score);
        SaveGame.Save<int>("currency", fpsc.GetComponent<FirstPersonController>().currency);
        SaveGame.Save<int>("spawnCount", spwanCount);

        enemies = new GameObject[spwanCount];
        Debug.Log("round " + round + " spwanCount " + spwanCount);
        for (int i = 0; i < spwanCount; i++)
        {
            // yield return new WaitForSeconds(1.0f);
            GameObject spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            while (Vector3.Distance(fpsc.transform.position, spawnPoint.transform.position) < minDistanceToSpawn)
            {
                spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            }

            GameObject enemySpawned = Instantiate(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)], spawnPoint.transform.position, Quaternion.identity);
            // increase enemy health by a linearly based on round number from 1 (round 1) to 5 in round(30)
            enemySpawned.GetComponentInChildren<AIExample>().health *= UnityEngine.Random.Range(1, 5) * (round / 30.0f);
            enemySpawned.GetComponentInChildren<AIExample>().fpsc = fpsc;
            // Lower => Faster attack
            enemySpawned.GetComponentInChildren<AIExample>().AttackCooldownMultiplier = (float)UnityEngine.Random.Range(0.8f, 1.3f);
            // Lower => Smaller iframes
            enemySpawned.GetComponentInChildren<AIExample>().DamagedCooldownMultiplier = (float)UnityEngine.Random.Range(0.8f, 1.3f);
            //add the enemy to the enemies array
            enemies[i] = enemySpawned;
            enemiesAlive++;
        }
        isTransitioning = false;
    }

    public void nextWave(int round)
    {
        fpsc.GetComponent<FirstPersonController>().currentHealth = fpsc.maxHealth;
        // remove all enemies from the enemies array and the scene
        for (int i = 0; i < enemies.Length; i++)
        {
            Destroy(enemies[i]);
        }
        // reset the enemy count
        enemiesAlive = 0;
        // add the time it took to complete the round to the list
        timeTook.Add(currentRoundTime);
        currentRoundTime = 0.0f;
        // create a new array of enemies
        AudioSource.PlayClipAtPoint(roundWin, transform.position, 1.5f);
        int spwanCount = UnityEngine.Random.Range(round, round * 3);

        // save the data for load
        SaveGame.Save<int>("round", round);
        SaveGame.Save<int>("score", fpsc.GetComponent<FirstPersonController>().score);
        SaveGame.Save<int>("currency", fpsc.GetComponent<FirstPersonController>().currency);
        SaveGame.Save<int>("spawnCount", spwanCount);

        enemies = new GameObject[spwanCount];
        Debug.Log("round " + round + " spwanCount " + spwanCount);
        for (int i = 0; i < spwanCount; i++)
        {
            GameObject spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            while (Vector3.Distance(fpsc.transform.position, spawnPoint.transform.position) < minDistanceToSpawn)
            {
                spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            }

            GameObject enemySpawned = Instantiate(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)], spawnPoint.transform.position, Quaternion.identity);
            // increase enemy health by a linearly based on round number from 1 (round 1) to 5 in round(30)
            enemySpawned.GetComponentInChildren<AIExample>().health *= UnityEngine.Random.Range(1, 5) * (round / 30.0f);
            enemySpawned.GetComponentInChildren<AIExample>().fpsc = fpsc;
            // Lower => Faster attack
            enemySpawned.GetComponentInChildren<AIExample>().AttackCooldownMultiplier = (float)UnityEngine.Random.Range(0.8f, 1.3f);
            // Lower => Smaller iframes
            enemySpawned.GetComponentInChildren<AIExample>().DamagedCooldownMultiplier = (float)UnityEngine.Random.Range(0.8f, 1.3f);
            //add the enemy to the enemies array
            enemies[i] = enemySpawned;
            enemiesAlive++;
        }
    }

    // used when resuming game  
    public void setWave()
    {
        int round = SaveGame.Load<int>("round");
        fpsc.GetComponent<FirstPersonController>().currentHealth = fpsc.maxHealth;
        // remove all enemies from the enemies array and the scene
        for (int i = 0; i < enemies.Length; i++)
        {
            Destroy(enemies[i]);
        }
        // reset the enemy count
        enemiesAlive = 0;
        // add the time it took to complete the round to the list
        timeTook.Add(currentRoundTime);
        currentRoundTime = 0.0f;
        // create a new array of enemies
        AudioSource.PlayClipAtPoint(roundWin, transform.position, 1.5f);
        int spwanCount = SaveGame.Load<int>("spawnCount");

        // save the data for load
        int score = SaveGame.Load<int>("score");
        int currency = SaveGame.Load<int>("currency");

        enemies = new GameObject[spwanCount];
        Debug.Log("round " + round + " spwanCount " + spwanCount);
        for (int i = 0; i < spwanCount; i++)
        {
            GameObject spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            while (Vector3.Distance(fpsc.transform.position, spawnPoint.transform.position) < minDistanceToSpawn)
            {
                spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            }

            GameObject enemySpawned = Instantiate(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)], spawnPoint.transform.position, Quaternion.identity);
            // increase enemy health by a linearly based on round number from 1 (round 1) to 5 in round(30)
            enemySpawned.GetComponentInChildren<AIExample>().health *= UnityEngine.Random.Range(1, 5) * (round / 30.0f);
            enemySpawned.GetComponentInChildren<AIExample>().fpsc = fpsc;
            // Lower => Faster attack
            enemySpawned.GetComponentInChildren<AIExample>().AttackCooldownMultiplier = (float)UnityEngine.Random.Range(0.8f, 1.3f);
            // Lower => Smaller iframes
            enemySpawned.GetComponentInChildren<AIExample>().DamagedCooldownMultiplier = (float)UnityEngine.Random.Range(0.8f, 1.3f);
            //add the enemy to the enemies array
            enemies[i] = enemySpawned;
            enemiesAlive++;
        }
    }
}
