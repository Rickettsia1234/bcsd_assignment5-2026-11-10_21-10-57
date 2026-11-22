using UnityEngine;

public class Slow : MonoBehaviour
{
    private TowerWeapon towerWeapon;

    private void Awake()
    {
        towerWeapon = GetComponentInParent<TowerWeapon>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( !collision.CompareTag("Enemy") ) return;

        var enemy = collision.GetComponent<Enemy>();
        enemy.TimeOffset += enemy.TimeOffset * towerWeapon.Slow;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ( !collision.CompareTag("Enemy") ) return;

        collision.GetComponent<Enemy>().ResetTimeOffset();
    }
}