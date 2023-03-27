using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public int round = 1;
    int zombiesInRound = 15;
    int zombiesSpawnedInRound = 0;
    float zombieSpawnTimer = 0;
    public Transform[] zombieSpawnPoints;
    public GameObject zombieEnemy;

    // Use this for initialization
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (zombiesSpawnedInRound < zombiesInRound)
        {
            if (zombieSpawnTimer > 0.5)
            {
                SpawnZombie();
                zombieSpawnTimer = 0;

        }
            else
            {
                zombieSpawnTimer += Time.deltaTime;
            }
    }
    }
    void SpawnZombie()
    {

        Vector3 randomSpawnPoint = zombieSpawnPoints[Random.Range(0, zombieSpawnPoints.Length)].position;
        Instantiate(zombieEnemy, randomSpawnPoint, Quaternion.identity);
        zombiesSpawnedInRound++;
    }
}
