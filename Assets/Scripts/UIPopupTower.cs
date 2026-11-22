using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class UIPopupTower : MonoBehaviour
{
    [SerializeField]
    private Image imageTower;
    [SerializeField]
    private TextMeshProUGUI textDamage;
    [SerializeField]
    private TextMeshProUGUI textRate;
    [SerializeField]
    private TextMeshProUGUI textRange;
    [SerializeField]
    private TextMeshProUGUI textLevel;
    [SerializeField]
    private TextMeshProUGUI textUpgradeCost;
    [SerializeField]
    private TextMeshProUGUI textSellCost;
    [SerializeField]
    private TowerAttackRange towerAttackRange;
    [SerializeField]
    private Button buttonUpgrade;
    [SerializeField]
    private SystemTextViewer systemTextViewer;

    private TowerWeapon currentTower;

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
        if ( currentTower.WeaponType == WeaponType.Cannon || currentTower.WeaponType == WeaponType.Laser )
        {
            imageTower.rectTransform.sizeDelta = new Vector2(88, 59);
            textDamage.text = $"Damage : {currentTower.Damage}"
                            + $"<color=red>+{currentTower.AddedDamage}</color>";
        }
        else
        {
            imageTower.rectTransform.sizeDelta = new Vector2(59, 59);

            if ( currentTower.WeaponType == WeaponType.Slow )
                textDamage.text = $"Slow : {currentTower.Slow * 100}%";
            else if ( currentTower.WeaponType == WeaponType.Buff )
                textDamage.text = $"Buff : {currentTower.Buff * 100}%";
        }

        imageTower.sprite = currentTower.TowerSprite;
        textRate.text = $"Rate : {currentTower.Rate}";
        textRange.text = $"Range : {currentTower.Range}";
        textLevel.text = $"Level : {currentTower.Level}";
        textUpgradeCost.text = $"{currentTower.UpgradeCost}";
        textSellCost.text = $"{currentTower.SellCost}";

        buttonUpgrade.interactable = currentTower.Level < currentTower.MaxLevel;
    }

    public void OnClickEventTowerUpgrade()
    {
        bool isSuccess = currentTower.Upgrade();

        if ( isSuccess == true )
        {
            UpdateTowerData();
            towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
        }
        else
        {
            systemTextViewer.PrintText(MESSAGE.MONEY);
        }
    }

    public void OnClickEventTowerSell()
    {
        currentTower.Sell();
        OffPopup();
    }
}