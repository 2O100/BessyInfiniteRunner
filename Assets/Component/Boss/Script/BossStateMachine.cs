using UnityEngine;
using System.Collections;

public class BossStateMachine : MonoBehaviour
{
    public enum BossState { Waiting, Attacking, Victory, Escaped }
    public BossState currentState = BossState.Waiting;

    [Header("Références")]
    public PlayerMovementController player;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public GameObject friendToRelease;

    [Header("Paramètres de Temps")]
    public float timeBetweenFights = 120f;
    public float fightDuration = 180f;
    public float timeBetweenProjectiles = 4f;

    [Header("Statistiques")]
    public int maxHealth = 10;
    private int currentHealth;
    public float speedMultiplier = 1.5f;

    private float timer = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        if (friendToRelease != null) friendToRelease.SetActive(false);
    }

    void Update()
    {
        if (currentState == BossState.Waiting)
        {
            timer += Time.deltaTime;
            if (timer >= timeBetweenFights)
            {
                StartCoroutine(BossFightSequence());
            }
        }
    }

    IEnumerator BossFightSequence()
    {
        currentState = BossState.Attacking;
        currentHealth = maxHealth;
        float fightTimer = 0f;
        Time.timeScale = speedMultiplier;

        while (fightTimer < fightDuration && currentHealth > 0)
        {
            Shoot();
            yield return new WaitForSeconds(timeBetweenProjectiles);
            fightTimer += timeBetweenProjectiles;
        }

        Time.timeScale = 1.0f;

        if (currentHealth <= 0)
        {
            currentState = BossState.Victory;
            if (friendToRelease != null) friendToRelease.SetActive(true);
        }
        else
        {
            currentState = BossState.Escaped;
        }

        yield return new WaitForSeconds(3f);
        timer = 0;
        currentState = BossState.Waiting;
    }

    void Shoot()
    {
        int randomLane = Random.Range(0, player._sideTarget.Length);
        Vector3 targetPos = player._sideTarget[randomLane].position;

        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        proj.GetComponent<BossProjectile>().Setup(targetPos, randomLane, this);
    }

    public void TakeDamage()
    {
        currentHealth--;
    }
}