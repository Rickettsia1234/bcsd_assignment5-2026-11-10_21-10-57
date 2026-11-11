using System.Collections;
using UnityEngine;

public enum EnemyDestroyType { Kill = 0, Arrive }

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int gold = 10;
    [SerializeField]
    private Transform hudPoint;
    [SerializeField]
    private Transform rotator;

    private WayPoint[] wayPoints;
    private int currentIndex = 0;
    private MovementTransform movementTransform;
    private EnemySpawner enemySpawner;

    public Transform HUDPoint => hudPoint;

    public void Setup(EnemySpawner enemySpawner, WayPoint[] wayPoints)
    {
        movementTransform = GetComponent<MovementTransform>();
        this.enemySpawner = enemySpawner;
        this.wayPoints = wayPoints;

        transform.position = wayPoints[currentIndex].position;

        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        NextMoveTo();

        while (true)
        {
            rotator.Rotate(Vector3.forward * 10);

            if (Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * movementTransform.MoveSpeed)
            {
                NextMoveTo();
            }

            yield return null;
        }
    }

    private void NextMoveTo()
    {
        if (currentIndex < wayPoints.Length - 1)
        {
            transform.position = wayPoints[currentIndex].position;
            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            movementTransform.MoveTo(direction);
        }
        else
        {
            OnDie(EnemyDestroyType.Arrive);
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        enemySpawner.DestroyEnemy(type, this, gold);
    }
}