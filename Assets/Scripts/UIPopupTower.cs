using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class UIPopupTower : MonoBehaviour
{
    [SerializeField]
    private Image           imageTower;
    [SerializeField]
    private TextMeshProUGUI textDamage;
    [SerializeField]
    private TextMeshProUGUI textRate;
    [SerializeField]
    private TextMeshProUGUI textRange;
    [SerializeField]
    private TextMeshProUGUI textLevel;
    [SerializeField]
    private TowerAttackRange towerAttackRange;

    private TowerWeapon     currentTower;

    private void Awake()
    {
        OffPopup();
    }

    private void Update()
    {
        if ( Keyboard.current.escapeKey.wasPressedThisFrame )
        {
            OffPopup();
        }
    }

    public void OnPopup(Transform towerWeapon)
    {
        currentTower = towerWeapon.GetComponent<TowerWeapon>();
        UpdateTowerData();
        gameObject.SetActive(true);
        towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
    }

    public void OffPopup()
    {
        gameObject.SetActive(false);
        towerAttackRange.OffAttackRange();
    }

    private void UpdateTowerData()
    {
        textDamage.text = $"Damage : {currentTower.Damage}";
        textRate.text   = $"Rate : {currentTower.Rate}";
        textRange.text  = $"Range : {currentTower.Range}";
        textLevel.text  = $"Level : {currentTower.Level}";
    }
}