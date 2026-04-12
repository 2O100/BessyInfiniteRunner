using UnityEngine;

public class FireflyCollectible : CollidableObject
{
    [Header("Firefly Settings")]
    [SerializeField] private int scoreValue = 1; 
    [SerializeField] private float bobbingSpeed = 3f;

    [Header("Lévitation")]
    [SerializeField] private float minY = 0.15f;
    [SerializeField] private float maxY = 1f;

    private float _centerLocalY;
    private float _bobbingRange;

    private void Start()
    {
        _centerLocalY = (minY + maxY) / 2f;
        _bobbingRange = (maxY - minY) / 2f;

        _centerLocalY += Random.Range(-0.1f, 0.1f);
    }

    private void Update()
    {
        float newYOffset = Mathf.Sin(Time.time * bobbingSpeed) * _bobbingRange;

        Vector3 newLocalPos = transform.localPosition;
        newLocalPos.y = _centerLocalY + newYOffset;
        transform.localPosition = newLocalPos;
    }

    public override void OnPlayerHit(PlayerCollisionController player)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
            Debug.Log($"<color=yellow>FIREFLY : +{scoreValue} !</color>");
        }

        Destroy(gameObject);
    }
}