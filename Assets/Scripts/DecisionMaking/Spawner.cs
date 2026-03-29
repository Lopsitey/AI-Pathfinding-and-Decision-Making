using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float sceneMinX = -10.0f;
    public float sceneMaxX = 10.0f;
    public float sceneMinY = -10.0f;
    public float sceneMaxY = 10.0f;

    int aliveEnemyCount = 0;
    int waveEnemiesSpawned = 0;
    int waveMaxEnemies = 5;
    int maxEnemiesAllowed = 10;

    public GameObject prefabToSpawn;
    public float InitialSpawnDelay = 5.0f;
    private float currentSpawnDelay;
    private float lastSpawnTime;
    public Transform[] spawnPoints = null;

    private Transform playerTransform;

    bool waveStart = true;
    float nextWaveTimer = 10;
    float waveEndTime = 0;

    //--------------------------------------------------------

    private void Start()
    {
        currentSpawnDelay = InitialSpawnDelay;

        Spawn();
        lastSpawnTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (waveStart && waveEnemiesSpawned < waveMaxEnemies && aliveEnemyCount < maxEnemiesAllowed)
        {
            if (Time.time > lastSpawnTime + currentSpawnDelay)
            {
                Spawn();
            }
        }

        if (!waveStart && Time.time > waveEndTime + nextWaveTimer)
        {
            waveStart = true;
        }
    }

    void DecrementAliveEnemyCount()
    {
        aliveEnemyCount--;
        if (aliveEnemyCount == 0 && waveEnemiesSpawned == waveMaxEnemies)
        {
            InitNextWave();
        }
    }

    void InitNextWave()
    {
        waveStart = false;
        currentSpawnDelay *= 0.9f;
        if (currentSpawnDelay < 1)
        {
            currentSpawnDelay = 1;
        }
        waveMaxEnemies += 5;
        waveEndTime = Time.time;
        waveEnemiesSpawned = 0;
    }

    void Spawn()
    {
        lastSpawnTime = Time.time;

        aliveEnemyCount++;
        waveEnemiesSpawned++;

        int potentialSpawns = spawnPoints.Length;

        Transform selectedSpawn = spawnPoints[Random.Range(0, potentialSpawns - 1)];

        GameObject g = Instantiate(prefabToSpawn, selectedSpawn.position, Quaternion.identity);
        g.GetComponent<Health>().OnHealthDepleted += DecrementAliveEnemyCount;

        WrapAround wrapAround = g.GetComponent<WrapAround>();
        if (wrapAround != null)
        {
            wrapAround.SetSceneDimensions(sceneMinX, sceneMaxX, sceneMinY, sceneMaxY);
        }
    }
}
