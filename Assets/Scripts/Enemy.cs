using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float timeOffset = 1f;

    private Transform[] wayPoints;
    private int wayPointCount;
    private int currentIndex = 0;

    public void Setup(Transform[] wayPoints)
    {
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        transform.position = wayPoints[currentIndex].position;
        currentIndex++;

        StartCoroutine(nameof(Process));
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward, 360 * Time.deltaTime);
    }

    private IEnumerator Process()
    {
        while ( true )
        {
            yield return StartCoroutine(MoveToAB(transform.position, wayPoints[currentIndex].position));

            if ( currentIndex < wayPointCount - 1 )
            {
                currentIndex++;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator MoveToAB(Vector3 start, Vector3 end)
    {
        float percent = 0F;
        float moveTime = Vector3.Distance(start, end) * timeOffset;

        while ( percent < 1F )
        {
            percent += Time.deltaTime / moveTime;
            transform.position = Vector3.Lerp(start, end, percent);

            yield return null;
        }
    }
}