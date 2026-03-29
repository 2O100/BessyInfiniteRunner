using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    private Vector3 targetPosition;
    private int targetLaneIndex;
    private BossStateMachine boss;
    private bool isReflected = false;

    public float speed = 20f;

    public void Setup(Vector3 dest, int lane, BossStateMachine bossRef)
    {
        targetPosition = dest;
        targetLaneIndex = lane;
        boss = bossRef;
    }

    public void Reflect() // Cette fonction sera appelée par ton PlayerCollisionController
    {
        isReflected = true;
    }

    void Update()
    {
        if (!isReflected)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f) Destroy(gameObject, 0.5f);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, boss.transform.position, speed * 2f * Time.deltaTime);
            if (Vector3.Distance(transform.position, boss.transform.position) < 0.5f)
            {
                boss.TakeDamage();
                Destroy(gameObject);
            }
        }
    }
}