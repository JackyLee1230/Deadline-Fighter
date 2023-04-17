using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField]
    public float maxDistanceToSpawn = 50;

    [SerializeField] public FirstPersonController fpsc;

    [SerializeField]
    public GameObject[] enemyPrefabs;

    [SerializeField]
    public GameObject[] enemies;

    [SerializeField]
    public AudioClip roundWin; //test

    int spwanCount;
    public Slider aliveBarSlider;
    public GameObject aliveBar;
    public TextMeshProUGUI roundNum;

    // Start is called before the first frame update
    void Start()
    {
        timeTook = new List<float>();
        isTransitioning = false;
        if (SaveGame.Load<int>("continue") == 1)
        {
            // load game
            setWave();
            SaveGame.Save<int>("continue", 0);
            Debug.Log("GAME STARTED WITH CONTINUE GAME");
        }
        aliveBarSlider.value = 1;
    }

    // Update is called once per frame
    void Update()
    {

        if (fpsc.GetComponent<FirstPersonController>().currentHealth <= 0)
        {
            // kill all enemies
            for (int i = 0; i < enemies.Length; i++)
            {
                Destroy(enemies[i]);
                enemiesAlive--;
            }
        }

        int unawareEnemies = 0;

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

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].GetComponentInChildren<AIExample>().isAware == false)
            {
                unawareEnemies++;
            }
        }


        if (enemies.Length == 1)
        {
            StartCoroutine(DelayAware(enemies[0].GetComponentInChildren<AIExample>()));
        }
        else if (enemies.Length > 1)
        {
            for (int i = 0; i < enemies.Length / 2; i++)
            {
                if (enemies[i].GetComponentInChildren<AIExample>().isAware == false)
                {
                    StartCoroutine(DelayAware(enemies[i].GetComponentInChildren<AIExample>()));
                    break;
                }
            }
        }
        if (!isTransitioning)
        {
            transitionTime = 0.0f;
            if (currentRoundTime < 60)
            {
                roundNum.text = "Round: " + round.ToString() + "\nTime: " + currentRoundTime.ToString("F0") + "s";
            }
            else if (currentRoundTime < 3600)
            {
                roundNum.text = "Round: " + round.ToString() + "\nTime: " + (currentRoundTime/60).ToString("F0") + "m " + (currentRoundTime % 60).ToString("F0") + "s";
            }
            else
            {
                roundNum.text = "Round: " + round.ToString() + "\nTime: " + (currentRoundTime / 3600).ToString("F0") + "h " + ((currentRoundTime % 3600)/60).ToString("F0") + "m " + (currentRoundTime % 60).ToString("F0") + "s";
            }
            if (spwanCount != 0)
            {
                Debug.Log("spwancount is " + spwanCount.ToString());
                Debug.Log("enemiesAlive is " + enemiesAlive.ToString());
                aliveBarSlider.value = (float)enemiesAlive / (float)spwanCount;
            }
            
        }
        else
        {
            transitionTime += Time.deltaTime;
            roundNum.text = "Round " + round.ToString() + " incoming in " + (5.0f - transitionTime).ToString("F1") + "s!";
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
        roundNum.text = "Round: " + round.ToString() + " Time:  " + currentRoundTime.ToString("F0") + "s";
        // nextWave(round);
        // fpsc.GetComponent<FirstPersonController>().currentHealth = fpsc.maxHealth;
        fpsc.GetComponent<FirstPersonController>().currency += UnityEngine.Random.Range(round * 10, round * 70);

        // weaponHolder.transform.GetChild(0).GetComponent<Gun>().gunData.reservedAmmo += ammoGiven;
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
        spwanCount = UnityEngine.Random.Range(round, round * 3);

        // save the data for load
        SaveGame.Save<int>("round", round);
        SaveGame.Save<int>("score", fpsc.GetComponent<FirstPersonController>().score);
        SaveGame.Save<int>("currency", fpsc.GetComponent<FirstPersonController>().currency);
        SaveGame.Save<int>("spawnCount", spwanCount);
        SaveGame.Save<float>("survivedTime", fpsc.timeSurvived);
        SaveGame.Save<int[]>("playerPosition", new int[] { (int)fpsc.transform.position.x, (int)fpsc.transform.position.y, (int)fpsc.transform.position.z });

        int[][] ammos = new int[fpsc.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(1).childCount][];
        for (int i = 0; i < fpsc.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(1).childCount; i++)
        {
            int[] tmp = new int[2];
            tmp[0] = fpsc.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(i).gameObject.GetComponent<Gun>().gunData.currentAmmo;
            tmp[1] = fpsc.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(i).gameObject.GetComponent<Gun>().gunData.reservedAmmo;
            ammos[i] = tmp;
        }
        SaveGame.Save<int[][]>("ammos", ammos);

        enemies = new GameObject[spwanCount];
        Debug.Log("round " + round + " spwanCount " + spwanCount);

        int tooFarCount = 0;

        for (int x = 0; x < spawnPoints.Length; x++)
        {
            if (Vector3.Distance(fpsc.transform.position, spawnPoints[x].transform.position) > maxDistanceToSpawn)
            {
                tooFarCount++;
            }
        }

        bool isTooFar = tooFarCount == spawnPoints.Length;

        for (int i = 0; i < spwanCount; i++)
        {
            GameObject spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            if (!isTooFar)
            {
                while (Vector3.Distance(fpsc.transform.position, spawnPoint.transform.position) < minDistanceToSpawn || Vector3.Distance(fpsc.transform.position, spawnPoint.transform.position) > maxDistanceToSpawn)
                {
                    spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
                }
            }
            else
            {
                // spawn at a random point
                spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            }

            GameObject enemySpawned = Instantiate(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)], spawnPoint.transform.position, Quaternion.identity);
            // increase enemy health by a linearly based on round number from 1 (round 1) to 5 in round(30)
            enemySpawned.GetComponentInChildren<AIExample>().health *= 5 * (round / 30.0f);
            enemySpawned.GetComponentInChildren<AIExample>().fpsc = fpsc;
            // Lower => Faster attack
            enemySpawned.GetComponentInChildren<AIExample>().AttackCooldownMultiplier = (float)UnityEngine.Random.Range(1f, 1.3f);
            // Lower => Smaller iframes
            enemySpawned.GetComponentInChildren<AIExample>().DamagedCooldownMultiplier = (float)UnityEngine.Random.Range(0.8f, 1.5f);
            //add the enemy to the enemies array
            enemies[i] = enemySpawned;
            enemiesAlive++;
        }
        isTransitioning = false;
    }

    public void nextWave(int round)
    {
        // fpsc.GetComponent<FirstPersonController>().currentHealth = fpsc.maxHealth;
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
        spwanCount = UnityEngine.Random.Range(round, round * 3);

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
        int roundNumber = SaveGame.Load<int>("round");
        round = roundNumber;
        // fpsc.GetComponent<FirstPersonController>().currentHealth = fpsc.maxHealth;
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
        fpsc.score = score;
        int currency = SaveGame.Load<int>("currency");
        fpsc.currency = currency;
        int[] player = SaveGame.Load<int[]>("playerPosition");
        float survivedTime = SaveGame.Load<float>("survivedTime");
        fpsc.timeSurvived = survivedTime;

        int[][] ammos = SaveGame.Load<int[][]>("ammos");
        for (int i = 0; i < ammos.Length; i++)
        {
            int[] tmp = ammos[i];
            fpsc.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(i).gameObject.GetComponent<Gun>().gunData.currentAmmo = tmp[0];
            fpsc.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(i).gameObject.GetComponent<Gun>().gunData.reservedAmmo = tmp[1];
        }

        fpsc.transform.position = new Vector3(player[0], player[1], player[2]);

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

    IEnumerator DelayAware(AIExample enemy)
    {
        yield return new WaitForSeconds(3f);
        if(!enemy.isAware || !enemy.forcedAware)
        {
            enemy.OnAware();
            enemy.forcedAware = true;
        }
    }
}
