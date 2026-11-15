using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private TowerTemplateSO towerTemplate;
    [SerializeField]
    private PlayerGold playerGold;
    [SerializeField]
    private EnemySpawner enemySpawner;
    [SerializeField]
    private SystemTextViewer systemTextViewer;

    public void TrySpawnTower(Transform tileTransform)
    {
        if ( towerTemplate.weapon[0].cost > playerGold.CurrentGold )
        {
            systemTextViewer.PrintText(MESSAGE.MONEY);
            return;
        }

        Tile tile = tileTransform.GetComponent<Tile>();

        if ( tile.IsBuildTower == true )
        {
            systemTextViewer.PrintText(MESSAGE.BUILD);
            return;
        }

        tile.IsBuildTower = true;
        playerGold.CurrentGold -= towerTemplate.weapon[0].cost;
        Vector3 position = tileTransform.position + Vector3.back;
        GameObject clone = Instantiate(towerTemplate.towerPrefab, position, Quaternion.identity);
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner, playerGold, tile);
    }
}