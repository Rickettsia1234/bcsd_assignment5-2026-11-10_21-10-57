using UnityEngine;
using System.Collections;

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

    private EnemySpawner enemySpawner;
    private Transform attackTarget = null;

    public void Setup(EnemySpawner enemySpawner)
    {
        this.enemySpawner = enemySpawner;
        StartCoroutine(nameof(ChangeTarget));
        StartCoroutine(nameof(AttackToTarget));
    }

    private void Update()
    {
        if ( attackTarget != null )
        {
            RotateToTarget();
        }
    }

    private IEnumerator ChangeTarget()
    {
        while ( true )
        {
            Transform closestTarget = SearchClosestTarget();

            if ( closestTarget != null )
            {
                attackTarget = closestTarget;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator AttackToTarget()
    {
        while ( true )
        {
            if ( attackTarget != null )
            {
                SpawnProjectile();
            }

            yield return new WaitForSeconds(attackRate);
        }
    }

    private Transform SearchClosestTarget()
    {
        float minDistance = Mathf.Infinity;
        Transform closestTarget = null;

        for ( int i = 0; i < enemySpawner.EnemyList.Count; i++ )
        {
            float distance = Vector3.Distance(transform.position, enemySpawner.EnemyList[i].transform.position);

            if ( distance <= attackRange && distance < minDistance )
            {
                minDistance = distance;
                closestTarget = enemySpawner.EnemyList[i].transform;
            }
        }

        return closestTarget;
    }

    private void RotateToTarget()
    {
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;

        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        clone.GetComponent<Projectile>().Setup(attackTarget);
    }
}