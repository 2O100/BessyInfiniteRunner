using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _baseLaneSpeed = 10f;
    [SerializeField] private float _bossSpeedMultiplier = 1.5f;
    [SerializeField] private int _activeChunkCount = 8;
    [SerializeField] private int _chunksToKeepBehind = 2;

    [Header("Components")]
    [SerializeField] private ChunkController[] _chunksPool;

    [Header("Boss Combat")]
    public GameObject dungBallPrefab;
    [Range(0, 100)] public float dungBallSpawnChance = 30f;
    public BossStateMachine bossStateMachine;

    [Header("Bonus & Collectibles")]
    [SerializeField] private GameObject healthBonusPrefab;
    [Range(0, 100)] public float healthSpawnChance = 15f;
    [Space]
    [SerializeField] private GameObject fireflyPrefab;
    [Range(0, 100)] public float fireflySpawnChance = 60f;

    private readonly List<ChunkController> _instancesChunks = new List<ChunkController>();
    private float _currentMultiplier = 1f;

    private void Start() => AddBaseChunks();

    void OnEnable() { EventSystem.OnBossStateChanged += HandleBossStateChange; }
    void OnDisable() { EventSystem.OnBossStateChanged -= HandleBossStateChange; }

    private void HandleBossStateChange(BossStateMachine.BossState newState) { }

    private void Update()
    {
        float finalSpeed = _baseLaneSpeed * _currentMultiplier;
        foreach (var chunk in _instancesChunks)
        {
            if (chunk != null) chunk.transform.Translate(Vector3.back * (finalSpeed * Time.deltaTime));
        }
        UpdateChunks();
    }

    private void UpdateChunks()
    {
        List<ChunkController> behindChunks = new List<ChunkController>();
        foreach (var chunk in _instancesChunks)
        {
            if (chunk.IsBehindPlayer()) behindChunks.Add(chunk);
        }

        if (behindChunks.Count > _chunksToKeepBehind)
        {
            int chunksToDeleteCount = behindChunks.Count - _chunksToKeepBehind;
            for (int i = 0; i < chunksToDeleteCount; i++)
            {
                var chunkToDelete = behindChunks[i];
                _instancesChunks.Remove(chunkToDelete);
                Destroy(chunkToDelete.gameObject);
            }
        }

        int missingChunkCount = _activeChunkCount - _instancesChunks.Count;
        for (int i = 0; i < missingChunkCount; i++)
        {
            var chunk = AddChunk(LastActiveChunk().EndAnchor);
            _instancesChunks.Add(chunk);

            if (bossStateMachine != null)
            {
                if (bossStateMachine.currentState == BossStateMachine.BossState.Attacking)
                {
                    SpawnItemOnChunk(chunk, dungBallPrefab, dungBallSpawnChance, 0.5f);
                }
                else if (bossStateMachine.currentState == BossStateMachine.BossState.Waiting)
                {
                    // On tente de spawn les deux, le hasard décidera !
                    SpawnItemOnChunk(chunk, healthBonusPrefab, healthSpawnChance, 1.5f);
                    SpawnItemOnChunk(chunk, fireflyPrefab, fireflySpawnChance, 2.0f);
                }
            }
        }
    }

    private void SpawnItemOnChunk(ChunkController targetChunk, GameObject prefab, float chance, float yOffset)
    {
        if (prefab == null) return;

        if (Random.Range(0f, 100f) <= chance)
        {
            List<Transform> spawnPoints = new List<Transform>();
            foreach (Transform child in targetChunk.transform)
            {
                if (child.name.Contains("DungBallSpawner")) spawnPoints.Add(child);
            }

            if (spawnPoints.Count > 0)
            {
                Transform selectedPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                Vector3 finalPos = selectedPoint.position;
                finalPos.y += yOffset;

                Instantiate(prefab, finalPos, Quaternion.identity, targetChunk.transform);
            }
        }
    }

    private void AddBaseChunks()
    {
        _instancesChunks.Clear();
        var firstChunk = AddChunk(Vector3.zero);
        float chunkLength = firstChunk.EndAnchor.z;
        firstChunk.transform.position = new Vector3(0, 0, -(chunkLength * _chunksToKeepBehind));
        _instancesChunks.Add(firstChunk);
        for (int i = 0; i < _activeChunkCount - 1; i++)
            _instancesChunks.Add(AddChunk(LastActiveChunk().EndAnchor));
    }

    private ChunkController AddChunk(Vector3 position)
    {
        if (_chunksPool.Length == 0) return null;
        return Instantiate(_chunksPool[Random.Range(0, _chunksPool.Length)], position, Quaternion.identity, transform);
    }

    private ChunkController LastActiveChunk() => _instancesChunks[_instancesChunks.Count - 1];
    public void SetBossSpeedActive(bool active) => _currentMultiplier = active ? _bossSpeedMultiplier : 1f;
}