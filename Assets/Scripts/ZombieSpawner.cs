using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 2f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnZombie), 0f, spawnInterval);
    }

    void SpawnZombie()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(zombiePrefab, spawnPoints[randomIndex].position, Quaternion.identity);
    }
}
