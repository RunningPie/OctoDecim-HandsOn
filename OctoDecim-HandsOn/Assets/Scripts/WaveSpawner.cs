using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // UI Library
using TMPro;        // Text Mesh Pro Library

public class WaveSpawner : MonoBehaviour
{
    public TextMeshProUGUI actionText;

    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform enemy;
        public int enemyCount;
        public float rate;


    }

    public Transform[] spawnPoints;

    public Wave[] waves;
    private int nextWave = 0;

    public float timeBetweenWaves = 5f;
    public float waveCountDown;

    private float searchCountDown = 1f;

    public SpawnState state = SpawnState.COUNTING;

    public bool doneSpawning;

    void Start()
    {
        waveCountDown = 0;

        if (spawnPoints.Length == 0)
        {
            Debug.Log("NO Spawn Point Referenced");
        }
    }


    void Update()
    {
        if (state == SpawnState.WAITING)
        {
            // CHECK IF ENEMIES ARE STILL ALIVE
            if (!EnemyIsAlive())
            {
                // BEGIN NEW WAVE
                WaveCompleted();
                return;

            }
            else
            {
                return;
            }
        }

        if (waveCountDown <= 0)
        {
            if (state != SpawnState.SPAWNING)
            {
                // Start Spawning Wave
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else
        {
            waveCountDown -= Time.deltaTime;
        }
    }

    void WaveCompleted()
    {
        Debug.Log("Wave Completed");
        actionText.text = "Wave Completed";

        state = SpawnState.COUNTING;
        waveCountDown = timeBetweenWaves;

        if (nextWave + 1 > waves.Length - 1)
        {
            nextWave = 0;
            Debug.Log("Completed all waves. Looping...");
        }
        else
        {
            nextWave++;
        }

    }

    bool EnemyIsAlive()
    {
        searchCountDown -= Time.deltaTime;
        if (searchCountDown <= 0){
            searchCountDown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") ==  null)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        state = SpawnState.SPAWNING;

        // SPAWN WAVE
        Debug.Log("Spawning Wave: " + _wave.name);
        actionText.text = "Spawning Wave: " + _wave.name;
        for (int i = 0; i < _wave.enemyCount; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(0f);
        }

        
        state = SpawnState.WAITING;

        yield break;
    }

    void SpawnEnemy(Transform _enemy)
    {
        // SPAWN ENEMY
        Debug.Log("Spawning Enemy: "+_enemy.name);

        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate (_enemy, _sp.position, _sp.rotation);
        
    }
}
