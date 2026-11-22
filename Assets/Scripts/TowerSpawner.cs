using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private TowerTemplateSO[] towerTemplate;
    [SerializeField]
    private PlayerGold playerGold;
    [SerializeField]
    private EnemySpawner enemySpawner;
    [SerializeField]
    private SystemTextViewer systemTextViewer;
    [SerializeField]
    private ObjectDetector objectDetector;

    private bool isOnTowerButton = false;
    private GameObject followTowerClone = null;
    private int towerType;

    public void ReadyToSpawnTower(int type)
    {
        towerType = type;

        if ( isOnTowerButton == true ) return;

        if ( towerTemplate[towerType].weapon[0].cost > playerGold.CurrentGold )
        {
            systemTextViewer.PrintText(MESSAGE.MONEY);
            return;
        }

        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);
        followTowerClone.GetComponent<ObjectFollowMousePosition>().Setup(objectDetector);

        isOnTowerButton = true;

        StartCoroutine(nameof(OnTowerCancelProcess));
    }

    public void TrySpawnTower(Transform tileTransform)
    {
        if ( isOnTowerButton == false ) return;

        Tile tile = tileTransform.GetComponent<Tile>();

        if ( tile.IsBuildTower == true )
        {
            systemTextViewer.PrintText(MESSAGE.BUILD);
            return;
        }

        isOnTowerButton = false;
        tile.IsBuildTower = true;

        playerGold.CurrentGold -= towerTemplate[towerType].weapon[0].cost;

        Vector3 position = tileTransform.position + Vector3.back;
        GameObject clone = Instantiate(towerTemplate[towerType].towerPrefab, position, Quaternion.identity);
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner, playerGold, tile);

        Destroy(followTowerClone);
        StopCoroutine(nameof(OnTowerCancelProcess));
    }

    private IEnumerator OnTowerCancelProcess()
    {
        while ( true )
        {
            if ( Keyboard.current.escapeKey.wasPressedThisFrame ||
                 Mouse.current.rightButton.wasPressedThisFrame )
            {
                isOnTowerButton = false;
                Destroy(followTowerClone);
                break;
            }

            yield return null;
        }
    }
}