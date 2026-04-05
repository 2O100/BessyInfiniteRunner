using UnityEngine;
using System.Collections.Generic;

public class ObstacleController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField, Tooltip("Vitesse de base des chunks en m/s")]
    private float _translationSpeed = 10f;

    [SerializeField, Tooltip("Multiplicateur appliqué lors de la phase Boss (modifiable dans Unity)")]
    private float _bossSpeedMultiplier = 1.5f; // <--- MODIFIABLE DANS UNITY

    [SerializeField] private int _activeChunkCount = 5;
    [SerializeField] private int _behindChunkCount = 1;

    [Header("Components")]
    [SerializeField] private ChunkController[] _chunksPool;

    private readonly List<ChunkController> _instancedChunks = new();
    private float _currentMultiplier = 1f; // Multiplicateur interne actuel (1 ou _bossSpeedMultiplier)

    private void Start()
    {
        AddBaseChunks();
    }

    private void Update()
    {
        // Calcul de la vitesse finale
        float finalSpeed = _translationSpeed * _currentMultiplier;

        foreach (var chunk in _instancedChunks)
        {
            chunk.transform.Translate(Vector3.back * finalSpeed * Time.deltaTime);
        }

        UpdateChunks();
    }

    private void UpdateChunks()
    {
        List<ChunkController> behindChunks = new();
        foreach (var chunk in _instancedChunks)
        {
            if (chunk.IsBehindPlayer())
            {
                behindChunks.Add(chunk);
            }
        }

        if (behindChunks.Count > _behindChunkCount)
        {
            int chunkToDeleteCount = behindChunks.Count - _behindChunkCount;
            for (int i = 0; i < chunkToDeleteCount; i++)
            {
                var chunkToDelete = behindChunks[i];
                _instancedChunks.Remove(chunkToDelete);
                Destroy(chunkToDelete.gameObject);
            }
        }

        int missingChunkCount = _activeChunkCount - _instancedChunks.Count;
        for (int i = 0; i < missingChunkCount; i++)
        {
            var chunk = AddChunk(LastActiveChunk().EndAnchor);
            _instancedChunks.Add(chunk);
        }
    }

    private void AddBaseChunks()
    {
        _instancedChunks.Add(AddChunk(Vector3.zero));

        for (int i = 0; i < _activeChunkCount - 1; i++)
        {
            var chunk = AddChunk(LastActiveChunk().EndAnchor);
            _instancedChunks.Add(chunk);
        }
    }

    private ChunkController AddChunk(Vector3 position)
    {
        if (_chunksPool.Length == 0) return null;

        int index = Random.Range(0, _chunksPool.Length);
        var chunk = Instantiate(_chunksPool[index], position, Quaternion.identity, transform);
        return chunk;
    }

    private ChunkController LastActiveChunk()
    {
        return _instancedChunks[_instancedChunks.Count - 1];
    }

    // --- Méthodes pour le Boss ---

    // Active le boost défini dans l'inspecteur
    public void SetBossSpeedActive(bool active)
    {
        _currentMultiplier = active ? _bossSpeedMultiplier : 1f;
        Debug.Log($"<color=cyan>ObstacleController : Multiplicateur = {_currentMultiplier}</color>");
    }
}