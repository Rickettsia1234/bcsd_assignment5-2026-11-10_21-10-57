using UnityEngine;
using System.Collections;

public enum EnemyDestroyType { Kill = 0, Arrive }

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Transform hudPoint;
    [SerializeField]
    private float timeOffset = 1f;
    [SerializeField]
    private int gold = 10;

    private Transform[] wayPoints;
    private int wayPointCount;
    private int currentIndex = 0;
    private EnemySpawner enemySpawner;
    private float baseTimeOffset;

    public Transform HUDPoint => hudPoint;
    public float TimeOffset
    {
        get => timeOffset;
        set => timeOffset = Mathf.Min(value, 5.0f);
    }

    public void Setup(EnemySpawner enemySpawner, Transform[] wayPoints)
    {
        baseTimeOffset = timeOffset;
        this.enemySpawner = enemySpawner;

        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        transform.position = wayPoints[currentIndex].position;

        currentIndex ++;

        StartCoroutine(nameof(Process));
    }

    private IEnumerator Process()
    {
        while ( true )
        {
            yield return StartCoroutine(MoveAToB(transform.position, wayPoints[currentIndex].position));

            if ( currentIndex < wayPointCount - 1 ) currentIndex ++;
            else
            {
                gold = 0;

                OnDie(EnemyDestroyType.Arrive);
            }
        }
    }

    private IEnumerator MoveAToB(Vector3 start, Vector3 end)
    {
        float percent = 0f;
        float moveTime = Vector3.Distance(start, end) * timeOffset;

        while ( percent < 1f )
        {
            percent += Time.deltaTime / moveTime;
            transform.position = Vector3.Lerp(start, end, percent);

            yield return null;
        }
    }

    public void ResetTimeOffset()
    {
        timeOffset = baseTimeOffset;
    }

    public void OnDie(EnemyDestroyType type)
    {
        enemySpawner.DestroyEnemy(type, this, gold);
    }
}