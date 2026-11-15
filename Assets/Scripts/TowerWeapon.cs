using UnityEngine;
using System.Collections;

public enum WeaponState { SearchClosestTarget = 0, AttackToTarget }

public class TowerWeapon : MonoBehaviour
{
    [SerializeField]
    private TowerTemplateSO towerTemplate;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private GameObject projectilePrefab;

    private EnemySpawner enemySpawner;
    private SpriteRenderer spriteRenderer;
    private PlayerGold playerGold;
    private Transform attackTarget = null;
    private Tile ownerTile;
    private WeaponState weaponState;
    private int level;

    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    public int Level => level + 1;
    public int MaxLevel => towerTemplate.weapon.Length;

    public void Setup(EnemySpawner enemySpawner, PlayerGold playerGold, Tile ownerTile)
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = ownerTile;

        ChangeState(WeaponState.SearchClosestTarget);
    }

    public void ChangeState(WeaponState newState)
    {
        StopCoroutine(weaponState.ToString());
        weaponState = newState;
        StartCoroutine(weaponState.ToString());
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

    private IEnumerator SearchClosestTarget()
    {
        float closestDistSqr = Mathf.Infinity;

        while ( true )
        {
            for ( int i = 0; i < enemySpawner.EnemyList.Count; ++ i )
            {
                float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
                if ( distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr )
                {
                    closestDistSqr = distance;
                    attackTarget = enemySpawner.EnemyList[i].transform;
                }
            }

            if ( attackTarget != null )
            {
                ChangeState(WeaponState.AttackToTarget);
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
                ChangeState(WeaponState.SearchClosestTarget);
                break;
            }

            float distance = Vector3.Distance(attackTarget.position, transform.position);
            if ( distance > towerTemplate.weapon[level].range )
            {
                attackTarget = null;
                ChangeState(WeaponState.SearchClosestTarget);
                break;
            }

            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            SpawnProjectile();
        }
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        clone.GetComponent<Projectile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
    }

    public bool Upgrade()
    {
        if ( playerGold.CurrentGold < towerTemplate.weapon[level+1].cost )
        {
            return false;
        }

        level ++;
        spriteRenderer.sprite = TowerSprite;
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        return true;
    }

    public void Sell()
    {
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        ownerTile.IsBuildTower = false;
        Destroy(gameObject);
    }
}