using UnityEngine;
using System.Collections;

public enum EnemyDestroyType { Kill = 0, Arrive }

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float timeOffset = 1f;
    [SerializeField]
    private Transform hudPoint;
    [SerializeField]
    private Transform rotator;
    [SerializeField]
    private int gold = 10;

    private Transform[] wayPoints;
    private int wayPointCount;
    private int currentIndex = 0;
    private EnemySpawner enemySpawner;

    public Transform HUDPoint => hudPoint;
    public int Gold => gold;

    public void Setup(EnemySpawner enemySpawner, Transform[] wayPoints)
    {
        this.enemySpawner = enemySpawner;

        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        transform.position = wayPoints[currentIndex].position;

        currentIndex++;

        StartCoroutine(nameof(Process));
    }

    private void Update()
    {
        rotator.Rotate(Vector3.forward, 360 * Time.deltaTime);
    }

    private IEnumerator Process()
    {
        while (true)
        {
            yield return StartCoroutine(MoveAToB(transform.position, wayPoints[currentIndex].position));

            if (currentIndex < wayPointCount - 1) currentIndex++;
            else
            {
                OnDie(EnemyDestroyType.Arrive);
                yield break;
            }
        }
    }

    private IEnumerator MoveAToB(Vector3 start, Vector3 end)
    {
        float percent = 0f;
        float moveTime = Vector3.Distance(start, end) * timeOffset;

        while (percent < 1f)
        {
            percent += Time.deltaTime / moveTime;
            transform.position = Vector3.Lerp(start, end, percent);

            yield return null;
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        enemySpawner.DestroyEnemy(type, this, gold);
    }
}