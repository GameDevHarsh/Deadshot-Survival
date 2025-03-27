using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> levelSpawnParents;
    private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private float spawnDelay;
    [SerializeField] private List<GameObject> zombiesList;
    [SerializeField] private List<GameObject> NextZone;
    [HideInInspector] public int currentMissionNumber;
    [SerializeField] private GameObject QuestPanel;
    [SerializeField] private GameObject ExitMessagePanel;
    public TextMeshProUGUI timerText;
    public float targetTimeInSeconds = 60f;

    private float spawnCooldownTime;
    private float _targetTimeInSeconds;
    private float startTime;
    [HideInInspector] public GameObject EnemyParent;

    private Dictionary<GameObject, Queue<GameObject>> zombiePools = new Dictionary<GameObject, Queue<GameObject>>();
    private int initialPoolSize = 10;

    private void OnEnable()
    {
        EnemyParent = new GameObject("Enemy's Parent");
        _targetTimeInSeconds = targetTimeInSeconds;
        startTime = Time.time;
        QuestPanel.SetActive(true);
        ExitMessagePanel.SetActive(false);
        spawnPoints.Clear();

        foreach (Transform g in levelSpawnParents[currentMissionNumber].GetComponentsInChildren<Transform>())
        {
            if (g != levelSpawnParents[currentMissionNumber].transform)
            {
                spawnPoints.Add(g);
            }
        }

        InitializeObjectPools();
    }

    private void InitializeObjectPools()
    {
        zombiePools.Clear();
        foreach (var zombiePrefab in zombiesList)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject zombie = Instantiate(zombiePrefab, EnemyParent.transform);
                zombie.SetActive(false);
                pool.Enqueue(zombie);
            }
            zombiePools[zombiePrefab] = pool;
        }
    }

    private GameObject GetPooledZombie(GameObject zombiePrefab)
    {
        if (zombiePools.ContainsKey(zombiePrefab))
        {
            if (zombiePools[zombiePrefab].Count > 0)
            {
                GameObject zombie = zombiePools[zombiePrefab].Dequeue();
                return zombie;
            }
        }
        // Expand pool if needed
        GameObject newZombie = Instantiate(zombiePrefab, EnemyParent.transform);
        return newZombie;
    }

    public void ReturnToPool(GameObject zombie, GameObject zombiePrefab)
    {
        zombie.SetActive(false);
        if (!zombiePools.ContainsKey(zombiePrefab))
        {
            zombiePools[zombiePrefab] = new Queue<GameObject>();
        }
        zombiePools[zombiePrefab].Enqueue(zombie);
    }

    void Update()
    {
        float currentTime = Time.time - startTime;
        float remainingTime = Mathf.Max(_targetTimeInSeconds - currentTime, 0f);

        if (remainingTime <= 0)
        {
            NextZone[currentMissionNumber].SetActive(true);
            ExitMessagePanel.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            NextZone[currentMissionNumber].SetActive(false);
            timerText.text = $"{Mathf.Floor(remainingTime / 60):00}:{Mathf.Floor(remainingTime % 60):00}";
        }

        if (remainingTime > 30)
        {
            spawnCooldownTime += Time.deltaTime;
            if (spawnCooldownTime > spawnDelay)
            {
                QuestPanel.SetActive(false);
                int spawnPointIndex = Random.Range(0, spawnPoints.Count);
                int zombieIndex = Random.Range(0, zombiesList.Count);
                GameObject zombiePrefab = zombiesList[zombieIndex];

                GameObject zombie = GetPooledZombie(zombiePrefab);
                zombie.transform.position = spawnPoints[spawnPointIndex].position;
                zombie.transform.rotation = zombiePrefab.transform.rotation;
                zombie.SetActive(true);
                spawnCooldownTime = 0;
            }
        }
    }

    private void OnDisable()
    {
        Destroy(EnemyParent);
    }
}
