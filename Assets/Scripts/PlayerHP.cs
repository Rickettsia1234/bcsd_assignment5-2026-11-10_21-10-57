using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    [SerializeField]
    private float maxHP = 20;
    private float currentHP;

    [SerializeField]
    private UIScreenRed screenUI;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        if (screenUI != null)
        {
            screenUI.PlayHitAnimation();
        }

        if (currentHP <= 0)
        {
        }
    }
}