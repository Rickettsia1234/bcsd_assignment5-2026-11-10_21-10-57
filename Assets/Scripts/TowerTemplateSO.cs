using UnityEngine;

[CreateAssetMenu]
public class TowerTemplateSO : ScriptableObject
{
    public GameObject towerPrefab;
    public Weapon[] weapon;

    [System.Serializable]
    public struct Weapon
    {
        public Sprite sprite;
        public float damage;
        public float rate;
        public float range;
        public int cost;
        public int sell;
    }
}