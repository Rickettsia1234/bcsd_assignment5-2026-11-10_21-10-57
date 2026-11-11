using System.Collections;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    [SerializeField]
    private float maxHP = 10;
    private float currentHP;
    private bool isDead = false;

    private SpriteRenderer spriteRenderer;
    private Enemy enemy;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    private void Awake()
    {
        currentHP = maxHP;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemy = GetComponent<Enemy>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead == true) return;

        currentHP -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        if (currentHP <= 0)
        {
            isDead = true;
            enemy.OnDie(EnemyDestroyType.Kill);
        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        Color color = spriteRenderer.color;

        color.a = 0.4f;
        spriteRenderer.color = color;

        yield return new WaitForSeconds(0.05f);

        color.a = 1.0f;
        spriteRenderer.color = color;
    }
}