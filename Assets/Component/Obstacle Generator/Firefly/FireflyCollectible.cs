using UnityEngine;

public class FireflyCollectible : CollidableObject
{
    [Header("Settings")]
    [SerializeField] private int _scoreValue = 1;

    [Header("Natural Floating")]
    [SerializeField] private float _minHeight = 0.8f;
    [SerializeField] private float _maxHeight = 2.2f;
    [SerializeField] private float _speed = 1.5f;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject _collectEffect;

    private float _randomOffset;

    private void Start()
    {
        // On ajoute un petit décalage aléatoire pour que toutes les 
        // lucioles ne montent pas en même temps, c'est plus organique.
        _randomOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    private void Update()
    {
        // 1. Calcul de la progression (entre 0 et 1)
        // On utilise un sinus pour obtenir une accélération/décélération naturelle
        float t = (Mathf.Sin(Time.time * _speed + _randomOffset) + 1f) / 2f;

        // 2. Lissage supplémentaire (SmoothStep) pour un effet encore plus "cotonneux"
        // Cela rend les virages en haut et en bas très doux
        float smoothedT = t * t * (3f - 2f * t);

        // 3. Application entre Min et Max
        float newY = Mathf.Lerp(_minHeight, _maxHeight, smoothedT);

        // Application au transform local
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
    }

    public override void OnPlayerHit(PlayerCollisionController player)
    {
        if (EventSystem.EventSystemInstance != null)
        {
            EventSystem.EventSystemInstance.TriggerFireflyCollected(_scoreValue);
        }

        if (_collectEffect != null)
        {
            Instantiate(_collectEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}