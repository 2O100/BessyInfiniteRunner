using UnityEngine;

public class CounterObstacle : CollidableObject
{
    [Header("Reflect Settings")]
    [SerializeField] private float _returnSpeed = 40f;

    private bool _isCountered = false;
    private Transform _target;

    private void Update()
    {
        // Si la bouse est renvoyée, elle se déplace vers le boss
        if (_isCountered && _target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.position, _returnSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // IMPORTANT : On ne vérifie le tag Boss QUE si la bouse a été contrée
        if (_isCountered && other.CompareTag("Boss"))
        {
            Debug.Log("<color=green>[SUCCESS] Impact sur le Boss !</color>");
            ApplyDamageToBoss();
        }
    }

    public override void OnPlayerHit(PlayerCollisionController player)
    {
        // Si la bouse est déjà en train de repartir, on ignore les autres collisions
        if (_isCountered) return;

        // On vérifie si le joueur attaque
        if (player.Movement != null && player.Movement.IsAttacking)
        {
            HandleCounter();
        }
        else
        {
            // Si le joueur n'attaque pas, il prend un dégât NORMAL
            Debug.Log("<color=red>[FAIL] Le joueur s'est pris la bouse !</color>");
            player.ApplyDamageToPlayer(); // Assure-toi que cette fonction existe dans ton PlayerCollisionController
            Destroy(gameObject);
        }
    }

    private void HandleCounter()
    {
        // On récupère la cible dans le GameManager
        if (GameManager.Instance != null && GameManager.Instance.bossShootPoint != null)
        {
            _target = GameManager.Instance.bossShootPoint;
            _isCountered = true;

            Debug.Log("<color=yellow>[Counter] Bouse renvoyée vers : </color>" + _target.name);

            // On arrête la rotation visuelle pour montrer l'impact du coup
            if (TryGetComponent(out DungBallRotation rot)) rot.enabled = false;

            // On désactive le collider temporairement et on le réactive 
            // pour être sûr qu'il ne re-touche pas le joueur immédiatement
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
                Invoke("EnableCollider", 0.1f);
            }
        }
    }

    private void EnableCollider() => GetComponent<Collider>().enabled = true;

    private void ApplyDamageToBoss()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log("<color=cyan>[Counter] Envoi du point de dégât au GameManager...</color>");
            GameManager.Instance.TakeBossDamage(1);
        }
        else
        {
            // Si tu vois ce message en rouge, c'est que ton GameManager est mal configuré
            Debug.LogError("[Counter] ERREUR : Le GameManager.Instance est introuvable !");
        }

        // Destroy(gameObject); // (Mets-le en commentaire temporairement)
    }
    private void OnDestroy()
    {
        if (_isCountered)
        {
            Debug.Log("<color=orange>[DEBUG] La bouse contrée vient d'être détruite !</color>");
        }
    }
}