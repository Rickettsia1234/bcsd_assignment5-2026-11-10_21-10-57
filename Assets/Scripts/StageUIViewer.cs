using UnityEngine;
using TMPro;

public class StageUIViewer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textPlayerHP;
    [SerializeField]
    private PlayerHP playerHP;
    [SerializeField]
    private TextMeshProUGUI textPlayerGold;
    [SerializeField]
    private PlayerGold playerGold;

    private void Update()
    {
        textPlayerHP.text = playerHP.CurrentHP.ToString();
        textPlayerGold.text = playerGold.CurrentGold.ToString();
    }
}