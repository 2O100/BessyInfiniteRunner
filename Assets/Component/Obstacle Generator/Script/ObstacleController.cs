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

    [Header("Bonus Settings")]
    [SerializeField] private GameObject bonusPrefab;
    private bool _shouldSpawnBonus = false;

    private readonly List<ChunkController> _instancesChunks = new List<ChunkController>();
    private float _currentMultiplier = 1f;


    private void Start() => AddBaseChunks();

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

        // Garder 2 chunks derrière
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
            // Utilisation de TA syntaxe exacte : .EndAnchor sans .position derrière
            var chunk = AddChunk(LastActiveChunk().EndAnchor);
            _instancesChunks.Add(chunk);
            SpawnDungBall(chunk);
        }
    }

    private void OnEnable()
    {
        // On s'abonne à l'événement de mort du boss
        if (EventSystem.EventSystemInstance != null)
        {
            EventSystem.OnBossDefeated += PrepareBonus;
        }
    }

    private void OnDisable()
    {
        // TRÈS IMPORTANT : On se désabonne pour éviter les fuites de mémoire
        if (EventSystem.EventSystemInstance != null)
        {
            EventSystem.OnBossDefeated -= PrepareBonus;
        }
    }

    private void PrepareBonus()
    {
        _shouldSpawnBonus = true;

        Debug.Log("<color=green>[ObstacleController]</color> Bonus prêt pour le prochain spawn !");
    }

    public void SpawnDungBall(ChunkController targetChunk)
    {
        if (bossStateMachine == null || bossStateMachine.currentState != BossStateMachine.BossState.Attacking)
            return;



        if (Random.Range(0f, 100f) <= dungBallSpawnChance)
        {
            List<Transform> spawnPoints = new List<Transform>();
            foreach (Transform child in targetChunk.transform)
            {
                if (child.name.Contains("DungBallSpawner")) spawnPoints.Add(child);
            }

            if (spawnPoints.Count > 0)
            {
                Transform selectedPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                GameObject dungBall = Instantiate(dungBallPrefab, selectedPoint.position, Quaternion.identity, targetChunk.transform);
            }
        }
    }

    private void AddBaseChunks()
    {
        _instancesChunks.Clear();

        // 1. On crée le premier chunk
        var firstChunk = AddChunk(Vector3.zero);

        // 2. Pour le décalage, on utilise la valeur brute de ton anchor
        // On suppose que EndAnchor.z est ta longueur
        float chunkLength = firstChunk.EndAnchor.z;
        firstChunk.transform.position = new Vector3(0, 0, -(chunkLength * _chunksToKeepBehind));

        _instancesChunks.Add(firstChunk);

        for (int i = 0; i < _activeChunkCount - 1; i++)
        {
            _instancesChunks.Add(AddChunk(LastActiveChunk().EndAnchor));
        }
    }

    private ChunkController AddChunk(Vector3 position)
    {
        if (_chunksPool.Length == 0) return null;
        int index = Random.Range(0, _chunksPool.Length);
        return Instantiate(_chunksPool[index], position, Quaternion.identity, transform);
    }

    private ChunkController LastActiveChunk()
    {
        return _instancesChunks[_instancesChunks.Count - 1];
    }

    public void SetBossSpeedActive(bool active)
    {
        _currentMultiplier = active ? _bossSpeedMultiplier : 1f;
    }
}