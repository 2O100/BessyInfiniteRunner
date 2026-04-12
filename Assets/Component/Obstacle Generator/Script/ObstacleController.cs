using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float _baseLaneSpeed = 10f;       // Standard scrolling speed
    [SerializeField] private float _bossSpeedMultiplier = 1.5f; // Speed increase during boss phase
    [SerializeField] private int _activeChunkCount = 8;        // Total chunks visible/active at once
    [SerializeField] private int _chunksToKeepBehind = 2;      // Chunks maintained behind the player for safety

    [Header("Components")]
    [SerializeField] private ChunkController[] _chunksPool;    // Array of available chunk prefabs

    [Header("Boss Combat")]
    public GameObject dungBallPrefab;                          // Projectile for the boss fight
    [Range(0, 100)] public float dungBallSpawnChance = 30f;    // Spawn probability during boss phase
    public BossStateMachine bossStateMachine;

    [Header("Bonus & Collectibles")]
    [SerializeField] private GameObject healthBonusPrefab;     // Life restoration item
    [Range(0, 100)] public float healthSpawnChance = 15f;
    [Space]
    [SerializeField] private GameObject fireflyPrefab;         // Main score collectible
    [Range(0, 100)] public float fireflySpawnChance = 60f;

    private readonly List<ChunkController> _instancesChunks = new List<ChunkController>();
    private float _currentMultiplier = 1f;

    private void Start() => AddBaseChunks();

    // Event subscription for boss state changes
    void OnEnable() { EventSystem.OnBossStateChanged += HandleBossStateChange; }
    void OnDisable() { EventSystem.OnBossStateChanged -= HandleBossStateChange; }

    private void HandleBossStateChange(BossStateMachine.BossState newState) { }

    private void Update()
    {
        // Calculate total movement speed
        float finalSpeed = _baseLaneSpeed * _currentMultiplier;

        // Move all active chunks backwards to simulate forward player movement
        foreach (var chunk in _instancesChunks)
        {
            if (chunk != null) chunk.transform.Translate(Vector3.back * (finalSpeed * Time.deltaTime));
        }

        // Logic to recycle old chunks and spawn new ones
        UpdateChunks();
    }

    private void UpdateChunks()
    {
        // 1. Identify chunks that have moved past the player
        List<ChunkController> behindChunks = new List<ChunkController>();
        foreach (var chunk in _instancesChunks)
        {
            if (chunk.IsBehindPlayer()) behindChunks.Add(chunk);
        }

        // 2. Cleanup: Remove excess chunks behind the player
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

        // 3. Generation: Spawn new chunks to maintain the active count
        int missingChunkCount = _activeChunkCount - _instancesChunks.Count;
        for (int i = 0; i < missingChunkCount; i++)
        {
            // Position the new chunk at the end anchor of the last active chunk
            var chunk = AddChunk(LastActiveChunk().EndAnchor);
            _instancesChunks.Add(chunk);

            // Handle item spawning based on the current Boss State
            if (bossStateMachine != null)
            {
                if (bossStateMachine.currentState == BossStateMachine.BossState.Attacking)
                {
                    // Spawn parryable projectiles during attack phase
                    SpawnItemOnChunk(chunk, dungBallPrefab, dungBallSpawnChance, 0.5f);
                }
                else if (bossStateMachine.currentState == BossStateMachine.BossState.Waiting)
                {
                    // Spawn collectibles/health during the resting phase
                    SpawnItemOnChunk(chunk, healthBonusPrefab, healthSpawnChance, 1.5f);
                    SpawnItemOnChunk(chunk, fireflyPrefab, fireflySpawnChance, 2.0f);
                }
            }
        }
    }

    // Generic method to spawn items on a newly created chunk
    private void SpawnItemOnChunk(ChunkController targetChunk, GameObject prefab, float chance, float yOffset)
    {
        if (prefab == null) return;

        // Roll the dice to see if we spawn an item
        if (Random.Range(0f, 100f) <= chance)
        {
            // Search for spawn points within the chunk hierarchy
            List<Transform> spawnPoints = new List<Transform>();
            foreach (Transform child in targetChunk.transform)
            {
                if (child.name.Contains("DungBallSpawner")) spawnPoints.Add(child);
            }

            // Pick a random point among the found spawners and instantiate the prefab
            if (spawnPoints.Count > 0)
            {
                Transform selectedPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                Vector3 finalPos = selectedPoint.position;
                finalPos.y += yOffset;

                // Parent the item to the chunk so it moves with the world
                Instantiate(prefab, finalPos, Quaternion.identity, targetChunk.transform);
            }
        }
    }

    // Initial world generation at game start
    private void AddBaseChunks()
    {
        _instancesChunks.Clear();
        var firstChunk = AddChunk(Vector3.zero);
        float chunkLength = firstChunk.EndAnchor.z;

        // Offset the first chunk so part of it starts behind the player
        firstChunk.transform.position = new Vector3(0, 0, -(chunkLength * _chunksToKeepBehind));
        _instancesChunks.Add(firstChunk);

        // Fill the rest of the queue
        for (int i = 0; i < _activeChunkCount - 1; i++)
            _instancesChunks.Add(AddChunk(LastActiveChunk().EndAnchor));
    }

    // Helper to pick a random prefab and instantiate it
    private ChunkController AddChunk(Vector3 position)
    {
        if (_chunksPool.Length == 0) return null;
        return Instantiate(_chunksPool[Random.Range(0, _chunksPool.Length)], position, Quaternion.identity, transform);
    }

    private ChunkController LastActiveChunk() => _instancesChunks[_instancesChunks.Count - 1];

    // Method to toggle the speed multiplier during boss phases
    public void SetBossSpeedActive(bool active) => _currentMultiplier = active ? _bossSpeedMultiplier : 1f;
}