using UnityEngine;

public class Projectile : MonoBehaviour
{
    private MovementTransform movementTransform;
    private Transform target;
    private float damage;

    public void Setup(Transform target, float damage)
    {
        movementTransform = GetComponent<MovementTransform>();
        this.target = target;
        this.damage = damage;
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            movementTransform.MoveTo(direction);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        if (collision.transform == target)
        {
            collision.GetComponent<EnemyHP>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}