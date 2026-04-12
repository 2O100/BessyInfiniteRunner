using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the infinite world generation, chunk recycling, 
/// and item spawning (collectibles and obstacles).
/// </summary>
public class ObstacleController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Fall-back speed if GameManager instance is not found.")]
    [SerializeField] private float _defaultBaseSpeed = 10f;
    [Tooltip("Speed multiplier applied when the boss enters the Attacking state.")]
    [SerializeField] private float _bossSpeedMultiplier = 1.5f;
    [Tooltip("Total number of chunks maintained in the scene.")]
    [SerializeField] private int _activeChunkCount = 8;
    [Tooltip("Number of chunks to keep behind the player before destroying them.")]
    [SerializeField] private int _chunksToKeepBehind = 2;

    [Header("Prefabs Pool")]
    [Tooltip("Array of different chunk prefabs to be randomly instantiated.")]
    [SerializeField] private ChunkController[] _chunksPool;

    [Header("Boss Combat References")]
    public GameObject dungBallPrefab;
    [Range(0, 100)] public float dungBallSpawnChance = 30f;
    public BossStateMachine bossStateMachine;

    [Header("Bonus & Collectibles")]
    [SerializeField] private GameObject healthBonusPrefab;
    [Range(0, 100)] public float healthSpawnChance = 15f;
    [SerializeField] private GameObject fireflyPrefab;
    [Range(0, 100)] public float fireflySpawnChance = 60f;

    // Internal list to track all active chunk instances
    private readonly List<ChunkController> _instancesChunks = new List<ChunkController>();

    // Multiplier modified by Boss State events
    private float _currentMultiplier = 1f;

    private void Start()
    {
        // Initial setup of the infinite road
        AddBaseChunks();
    }

    private void OnEnable()
    {
        // SUBSCRIBE to Boss events: Decouples the Boss script from the World movement
        if (EventSystem.EventSystemInstance != null)
            EventSystem.OnBossStateChanged += HandleBossStateChange;
    }

    private void OnDisable()
    {
        // UNSUBSCRIBE to prevent memory leaks and errors when the object is disabled
        if (EventSystem.EventSystemInstance != null)
            EventSystem.OnBossStateChanged += HandleBossStateChange;
    }

    /// <summary>
    /// Reacts to boss state changes sent via the EventSystem.
    /// </summary>
    private void HandleBossStateChange(BossStateMachine.BossState newState)
    {
        // Automatically speeds up the world if boss is attacking
        SetBossSpeedActive(newState == BossStateMachine.BossState.Attacking);
    }

    private void Update()
    {
        // SYNC POINT: We pull the game speed from GameManager to keep distance/UI in sync
        float globalSpeed = (GameManager.Instance != null) ? GameManager.Instance.gameSpeedMultiplier : _defaultBaseSpeed;

        // Final velocity combining base speed and boss state multiplier
        float finalVelocity = globalSpeed * _currentMultiplier;

        // PERFORMANCE: Single loop to move all chunks backward (simulating forward movement)
        foreach (var chunk in _instancesChunks)
        {
            if (chunk != null)
                chunk.transform.Translate(Vector3.back * (finalVelocity * Time.deltaTime));
        }

        // Check if chunks need to be recycled/respawned
        UpdateChunksLogic();
    }

    /// <summary>
    /// Handles chunk recycling logic: detects old chunks, destroys them, and spawns new ones.
    /// </summary>
    private void UpdateChunksLogic()
    {
        // 1. IDENTIFY: Find chunks that have fully passed the player (Z <= 0)
        List<ChunkController> behindChunks = new List<ChunkController>();
        foreach (var chunk in _instancesChunks)
        {
            if (chunk.IsBehindPlayer()) behindChunks.Add(chunk);
        }

        // 2. CLEANUP: Keep a safety buffer of chunks behind the player, destroy the rest
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

        // 3. GENERATION: Fill the gap to maintain the required active chunk count
        int missingChunkCount = _activeChunkCount - _instancesChunks.Count;
        for (int i = 0; i < missingChunkCount; i++)
        {
            // Always spawn at the 'EndAnchor' of the very last chunk in the list
            var chunk = AddChunk(LastActiveChunk().EndAnchor);
            _instancesChunks.Add(chunk);

            // Populate the new chunk based on current Boss state
            PopulateNewChunk(chunk);
        }
    }

    /// <summary>
    /// Decides what items (DungBalls or Bonuses) to spawn on a newly created chunk.
    /// </summary>
    private void PopulateNewChunk(ChunkController chunk)
    {
        if (bossStateMachine == null) return;

        // If Boss is attacking, only spawn hazards (DungBalls)
        if (bossStateMachine.currentState == BossStateMachine.BossState.Attacking)
        {
            SpawnItemOnChunk(chunk, dungBallPrefab, dungBallSpawnChance, 0.5f);
        }
        // If Boss is waiting, spawn beneficial items
        else
        {
            SpawnItemOnChunk(chunk, healthBonusPrefab, healthSpawnChance, 1.5f);
            SpawnItemOnChunk(chunk, fireflyPrefab, fireflySpawnChance, 2.0f);
        }
    }

    /// <summary>
    /// Generic method to spawn a prefab on a chunk at designated spawner locations.
    /// </summary>
    private void SpawnItemOnChunk(ChunkController targetChunk, GameObject prefab, float chance, float yOffset)
    {
        if (prefab == null || Random.Range(0f, 100f) > chance) return;

        // Search for specific empty GameObjects named "DungBallSpawner" inside the chunk prefab
        List<Transform> spawnPoints = new List<Transform>();
        foreach (Transform child in targetChunk.transform)
        {
            if (child.name.Contains("DungBallSpawner")) spawnPoints.Add(child);
        }

        if (spawnPoints.Count > 0)
        {
            // Pick a random lane among the found spawners
            Transform selectedPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            Vector3 finalPos = selectedPoint.position;
            finalPos.y += yOffset; // Adjust height based on item type

            // Instantiate and parent the item to the chunk so it moves with the world
            Instantiate(prefab, finalPos, Quaternion.identity, targetChunk.transform);
        }
    }

    /// <summary>
    /// Pre-generates the first set of chunks when the game begins.
    /// </summary>
    private void AddBaseChunks()
    {
        _instancesChunks.Clear();

        // Create the first chunk at the origin
        var firstChunk = AddChunk(Vector3.zero);
        float chunkLength = firstChunk.EndAnchor.z;

        // Reposition the road so the player starts "inside" the road rather than at the very start
        firstChunk.transform.position = new Vector3(0, 0, -(chunkLength * _chunksToKeepBehind));
        _instancesChunks.Add(firstChunk);

        // Fill up the rest of the chunks sequentially
        for (int i = 0; i < _activeChunkCount - 1; i++)
        {
            _instancesChunks.Add(AddChunk(LastActiveChunk().EndAnchor));
        }
    }

    /// <summary>
    /// Helper to pick a random prefab from the pool and instantiate it.
    /// </summary>
    private ChunkController AddChunk(Vector3 position)
    {
        if (_chunksPool == null || _chunksPool.Length == 0) return null;

        ChunkController randomPrefab = _chunksPool[Random.Range(0, _chunksPool.Length)];
        return Instantiate(randomPrefab, position, Quaternion.identity, transform);
    }

    // Helper to identify the last chunk currently in the list
    private ChunkController LastActiveChunk() => _instancesChunks[_instancesChunks.Count - 1];

    /// <summary>
    /// Changes the world movement speed based on Boss events.
    /// </summary>
    public void SetBossSpeedActive(bool active)
    {
        _currentMultiplier = active ? _bossSpeedMultiplier : 1f;
    }
}