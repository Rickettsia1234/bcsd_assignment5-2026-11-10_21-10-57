using UnityEngine;
using System.Collections;

public enum WeaponType { Cannon = 0, Laser, Slow, Buff }
public enum WeaponState { SearchClosestTarget = 0, TryAttackCannon, TryAttackLaser }

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    [SerializeField]
    private TowerTemplateSO towerTemplate;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private WeaponType weaponType;

    [Header("Cannon")]
    [SerializeField]
    private GameObject projectilePrefab;

    [Header("Laser")]
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private Transform hitEffect;
    [SerializeField]
    private LayerMask targetLayer;

    private TowerSpawner towerSpawner;
    private EnemySpawner enemySpawner;
    private SpriteRenderer spriteRenderer;
    private PlayerGold playerGold;
    private Transform attackTarget = null;
    private Tile ownerTile;
    private WeaponState weaponState;
    private int level = 0;
    private float addedDamage;
    private int buffLevel;

    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    public float Slow => towerTemplate.weapon[level].slow;
    public float Buff => towerTemplate.weapon[level].buff;
    public int Level => level + 1;
    public int MaxLevel => towerTemplate.weapon.Length;
    public int UpgradeCost => Level < MaxLevel ? towerTemplate.weapon[level + 1].cost : 0;
    public int SellCost => towerTemplate.weapon[level].sell;
    public WeaponType WeaponType => weaponType;
    public float AddedDamage
    {
        get => addedDamage;
        set => addedDamage = Mathf.Max(0, value);
    }
    public int BuffLevel
    {
        get => buffLevel;
        set => buffLevel = Mathf.Max(0, value);
    }

    public void Setup(TowerSpawner towerSpawner, EnemySpawner enemySpawner, PlayerGold playerGold, Tile ownerTile)
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        this.towerSpawner = towerSpawner;
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = ownerTile;

        if ( weaponType == WeaponType.Laser ) DisableLaser();

        if ( weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser )
        {
            ChangeState(WeaponState.SearchClosestTarget);
        }
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
        while ( true )
        {
            attackTarget = FindClosestAttackTarget();

            if ( attackTarget != null )
            {
                if ( weaponType == WeaponType.Cannon )
                {
                    ChangeState(WeaponState.TryAttackCannon);
                }
                else if ( weaponType == WeaponType.Laser )
                {
                    ChangeState(WeaponState.TryAttackLaser);
                }
            }

            yield return null;
        }
    }

    private IEnumerator TryAttackCannon()
    {
        while ( true )
        {
            if ( IsPossibleToAttackTarget() == false )
            {
                ChangeState(WeaponState.SearchClosestTarget);
                break;
            }

            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        EnableLaser();

        while ( true )
        {
            if ( IsPossibleToAttackTarget() == false )
            {
                DisableLaser();
                ChangeState(WeaponState.SearchClosestTarget);
                break;
            }

            SpawnLaser();

            yield return null;
        }
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        float damage = towerTemplate.weapon[level].damage + AddedDamage;
        clone.GetComponent<Projectile>().Setup(attackTarget, damage);
    }

    public bool Upgrade()
    {
        if ( playerGold.CurrentGold < towerTemplate.weapon[level + 1].cost )
        {
            return false;
        }

        level ++;
        spriteRenderer.sprite = TowerSprite;
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        if ( weaponType == WeaponType.Laser )
        {
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }

        towerSpawner.OnBuffAllBuffTowers();

        return true;
    }

    public void Sell()
    {
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        ownerTile.IsBuildTower = false;
        Destroy(gameObject);

        towerSpawner.ResetBuff();
    }

    public void OnBuffAroundTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for ( int i = 0; i < towers.Length; ++ i )
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            if ( weapon.BuffLevel > Level ) continue;

            if ( Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range )
            {
                if ( weapon.WeaponType == WeaponType.Cannon || weapon.WeaponType == WeaponType.Laser )
                {
                    weapon.AddedDamage = weapon.Damage * (towerTemplate.weapon[level].buff);
                    Debug.Log($"{weapon.AddedDamage} = {weapon.Damage} * {towerTemplate.weapon[level].buff}");

                    weapon.BuffLevel = Level;
                }
            }
        }
    }

    private Transform FindClosestAttackTarget()
    {
        float closestDistSqr = Mathf.Infinity;

        for ( int i = 0; i < enemySpawner.EnemyList.Count; ++ i )
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
            if ( distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr )
            {
                closestDistSqr = distance;
                attackTarget = enemySpawner.EnemyList[i].transform;
            }
        }

        return attackTarget;
    }

    private bool IsPossibleToAttackTarget()
    {
        if ( attackTarget == null )
        {
            return false;
        }

        float distance = Vector3.Distance(attackTarget.position, transform.position);
        if ( distance > towerTemplate.weapon[level].range )
        {
            attackTarget = null;
            return false;
        }

        return true;
    }

    private void EnableLaser()
    {
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }

    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }

    private void SpawnLaser()
    {
        Vector3 direction = (attackTarget.position - spawnPoint.position).normalized;
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction, towerTemplate.weapon[level].range, targetLayer);

        for ( int i = 0; i < hit.Length; ++ i )
        {
            if ( hit[i].transform == attackTarget )
            {
                lineRenderer.SetPosition(0, spawnPoint.position);
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                hitEffect.position = hit[i].point;

                float damage = towerTemplate.weapon[level].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHP>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }
}