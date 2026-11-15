using UnityEngine;
using TMPro;

public class StageUIViewer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textPlayerHP;
    [SerializeField]
    private TextMeshProUGUI textPlayerGold;
    [SerializeField]
    private TextMeshProUGUI textWave;
    [SerializeField]
    private TextMeshProUGUI textEnemyCount;
    [SerializeField]
    private PlayerHP playerHP;
    [SerializeField]
    private PlayerGold playerGold;
    [SerializeField]
    private WaveSystem waveSystem;
    [SerializeField]
    private EnemySpawner enemySpawner;

    private void Update()
    {
        textPlayerHP.text = $"{playerHP.CurrentHP}/{playerHP.MaxHP}";
        textPlayerGold.text = $"{playerGold.CurrentGold}";
        textWave.text = $"{waveSystem.CurrentWave}/{waveSystem.MaxWave}";
        textEnemyCount.text = $"{enemySpawner.CurrentEnemyCount}/{enemySpawner.MaxEnemyCount}";
    }
}