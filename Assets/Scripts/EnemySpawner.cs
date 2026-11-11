using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject enemyHPPrefab;
    [SerializeField]
    private Transform hpUIParent;
    [SerializeField]
    private float spawnTime;
    [SerializeField]
    private Transform[] wayPoints;
    [SerializeField]
    private PlayerHP playerHP;
    [SerializeField]
    private PlayerGold playerGold;

    private List<Enemy> enemyList;

    public List<Enemy> EnemyList => enemyList;

    private void Awake()
    {
        enemyList = new List<Enemy>();
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            GameObject clone = Instantiate(enemyPrefab);
            Enemy enemy = clone.GetComponent<Enemy>();

            WayPoint[] points = new WayPoint[wayPoints.Length];
            for (int i = 0; i < wayPoints.Length; i++)
            {
                points[i] = wayPoints[i].GetComponent<WayPoint>();
            }

            enemy.Setup(this, points);
            enemyList.Add(enemy);

            SpawnEnemyHPSlider(clone);

            yield return new WaitForSeconds(spawnTime);
        }
    }

    private void SpawnEnemyHPSlider(GameObject enemyObject)
    {
        GameObject sliderClone = Instantiate(enemyHPPrefab, hpUIParent);
        
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        EnemyHP enemyHP = enemyObject.GetComponent<EnemyHP>();

        sliderClone.GetComponent<FollowTargetUI>().Setup(enemy.HUDPoint);
        sliderClone.GetComponent<EnemyHPViewer>().Setup(enemyHP);
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        if (type == EnemyDestroyType.Arrive)
        {
            playerHP.TakeDamage(1);
        }
        else if (type == EnemyDestroyType.Kill)
        {
            playerGold.CurrentGold += gold;
        }

        enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }
}