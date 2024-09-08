using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public GameObject enemyPrefab;  // Prefab of the enemy to spawn
        public float spawnProbability;  // Probability for this enemy type to spawn
    }

    public Transform[] spawnPoints;  // Array of spawn points in the scene
    public EnemyType[] enemyTypes;  // Array of enemy types with their probabilities
    public float initialSpawnInterval = 5f;  // Initial spawn time interval
    public float spawnIntervalReduction = 0.1f; // How much to reduce the spawn interval over time
    public float minimumSpawnInterval = 1f;  // Minimum interval between spawns
    public float timeBetweenWaves = 5f;  // Time between each wave

    private float currentSpawnInterval;

    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy();

            // Wait for the current spawn interval before spawning the next enemy
            yield return new WaitForSeconds(currentSpawnInterval);

            // Reduce spawn interval to increase difficulty
            if (currentSpawnInterval > minimumSpawnInterval)
            {
                currentSpawnInterval -= spawnIntervalReduction;
            }
        }
    }

    void SpawnEnemy()
    {
        // Choose a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPosition = spawnPoint.position;

        // Choose an enemy to spawn based on probabilities
        GameObject selectedEnemy = SelectEnemyByProbability();

        if (selectedEnemy != null)
        {
            // Instantiate the enemy at the selected spawn position
            Instantiate(selectedEnemy, spawnPosition, Quaternion.identity);
        }
    }

    GameObject SelectEnemyByProbability()
    {
        // Calculate the total probability
        float totalProbability = 0f;
        foreach (EnemyType enemy in enemyTypes)
        {
            totalProbability += enemy.spawnProbability;
        }

        // Choose a random value within the total probability range
        float randomValue = Random.Range(0f, totalProbability);

        // Determine which enemy type corresponds to the random value
        float cumulativeProbability = 0f;
        foreach (EnemyType enemy in enemyTypes)
        {
            cumulativeProbability += enemy.spawnProbability;
            if (randomValue <= cumulativeProbability)
            {
                return enemy.enemyPrefab;
            }
        }

        return null; // In case something goes wrong
    }
}
