using System.Collections;
using UnityEngine;

public enum TowerState { SearchTarget = 0, AttackToTarget }

public class TowerWeapon : MonoBehaviour
{
    [SerializeField]
    private GameObject      projectilePrefab;
    [SerializeField]
    private Transform       spawnPoint;
    [SerializeField]
    private float           attackRate = 0.5f;
    [SerializeField]
    private float           attackRange = 2.0f;
    [SerializeField]
    private float           attackDamage = 1;

    private int             level = 1;
    private TowerState      towerState = TowerState.SearchTarget;
    private Transform       attackTarget = null;
    private EnemySpawner    enemySpawner;

    public float Damage => attackDamage;
    public float Rate   => attackRate;
    public float Range  => attackRange;
    public int   Level  => level;

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

    private void Update()
    {
        if ( attackTarget != null )
        {
            RotateToTarget();
        }
    }

    private void RotateToTarget()
    {
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private IEnumerator SearchTarget()
    {
        while ( true )
        {
            float closestDistSqr = Mathf.Infinity;
            for ( int i = 0; i < enemySpawner.EnemyList.Count; ++ i )
            {
                float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
                if ( distance <= attackRange && distance <= closestDistSqr )
                {
                    closestDistSqr = distance;
                    attackTarget = enemySpawner.EnemyList[i].transform;
                }
            }

            if ( attackTarget != null )
            {
                ChangeState(TowerState.AttackToTarget);
            }

            yield return null;
        }
    }

    private IEnumerator AttackToTarget()
    {
        while ( true )
        {
            if ( attackTarget == null )
            {
                ChangeState(TowerState.SearchTarget);
                break;
            }

            float distance = Vector3.Distance(attackTarget.position, transform.position);
            if ( distance > attackRange )
            {
                attackTarget = null;
                ChangeState(TowerState.SearchTarget);
                break;
            }

            yield return new WaitForSeconds(attackRate);

            SpawnProjectile();
        }
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        clone.GetComponent<Projectile>().Setup(attackTarget, attackDamage);
    }
}