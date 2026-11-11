using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;
    [SerializeField]
    private int towerBuildGold = 50;
    [SerializeField]
    private EnemySpawner enemySpawner;
    [SerializeField]
    private PlayerGold playerGold;

    public void SpawnTower(Transform tileTransform)
    {
        Tile tile = tileTransform.GetComponent<Tile>();

        if (tile.IsBuildTower == true) return;

        if (playerGold.CurrentGold < towerBuildGold) return;

        tile.IsBuildTower = true;
        playerGold.CurrentGold -= towerBuildGold;

        GameObject clone = Instantiate(towerPrefab, tileTransform.position, Quaternion.identity);
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner);
    }
}