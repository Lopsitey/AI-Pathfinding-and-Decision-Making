using System;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public static Action<Vector2,Vector2> OnPickUpSpawned;

    [SerializeField]
    GameObject healthPickUpPrefab;

    [SerializeField]
    GameObject ammoPickUpPrefab;

    float minWaitTime = 15.0f;
    float maxWaitTime = 30.0f;

    float timeSpawnAllowed = 0;
    float currentSpawnDelay = 0;
    bool spawnAllowed = true;

    private void Start()
    {
        Pickup.PickUpCollected += OnPickupCollected;

        currentSpawnDelay = minWaitTime;
    }

    private void FixedUpdate()
    {
        if(spawnAllowed && Time.time > timeSpawnAllowed + currentSpawnDelay)
        {
            Spawn();
        }
    }

    void OnPickupCollected()
    {
        spawnAllowed = true;
        timeSpawnAllowed = Time.time;
    }
    

    void Spawn()
    {
        Vector2 selectedSpawn1 = TileGrid.GetRandomWalkableTile(2).transform.position;
        Vector2 selectedSpawn2 = TileGrid.GetRandomWalkableTile(2).transform.position;

        while((selectedSpawn1 - selectedSpawn2).magnitude < 5f) 
        {
            selectedSpawn2 = TileGrid.GetRandomWalkableTile(2).transform.position;
        }

        GameObject g1 = Instantiate(healthPickUpPrefab, selectedSpawn1, Quaternion.identity);
        GameObject g2 = Instantiate(ammoPickUpPrefab, selectedSpawn2, Quaternion.identity);

        OnPickUpSpawned.Invoke(selectedSpawn1, selectedSpawn2);

        currentSpawnDelay = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
        spawnAllowed = false;
    }
}
