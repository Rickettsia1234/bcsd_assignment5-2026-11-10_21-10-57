using System.Collections;
using UnityEngine;

public enum TowerState { SearchTarget = 0, AttackToTarget }

public class TowerWeapon : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private float attackRate = 0.5f;
    [SerializeField]
    private float attackRange = 2.0f;
    [SerializeField]
    private float attackDamage = 1.0f;
    [SerializeField]
    private Transform targetTransform;

    private TowerState towerState = TowerState.SearchTarget;
    private EnemySpawner enemySpawner;

    public float AttackRange => attackRange;

    public void Setup(EnemySpawner enemySpawner)
    {
        this.enemySpawner = enemySpawner;
        ChangeState(TowerState.SearchTarget);
    }

    public void ChangeState(TowerState newState)
    {
        StopCoroutine(towerState.ToString());
        towerState = newState;
        StartCoroutine(towerState.ToString());
    }

    private IEnumerator SearchTarget()
    {
        while (true)
        {
            float closestDistSqr = Mathf.Infinity;
            for (int i = 0; i < enemySpawner.EnemyList.Count; ++i)
            {
                float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
                if (distance <= attackRange && distance < closestDistSqr)
                {
                    closestDistSqr = distance;
                    targetTransform = enemySpawner.EnemyList[i].transform;
                }
            }

            if (targetTransform != null)
            {
                ChangeState(TowerState.AttackToTarget);
            }

            yield return null;
        }
    }

    private IEnumerator AttackToTarget()
    {
        while (true)
        {
            if (targetTransform == null)
            {
                ChangeState(TowerState.SearchTarget);
                break;
            }

            float distance = Vector3.Distance(targetTransform.position, transform.position);
            if (distance > attackRange)
            {
                targetTransform = null;
                ChangeState(TowerState.SearchTarget);
                break;
            }

            RotateToTarget();

            yield return new WaitForSeconds(attackRate);

            SpawnProjectile();
        }
    }

    private void RotateToTarget()
    {
        Vector3 dx = targetTransform.position - transform.position;
        float degree = Mathf.Atan2(dx.y, dx.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        clone.GetComponent<Projectile>().Setup(targetTransform, attackDamage);
    }
}