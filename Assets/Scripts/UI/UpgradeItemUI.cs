using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UpgradeItemUI : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costText;
    public Button buyButton;
    
    private Upgrade upgrade;
    private UpgradeUI upgradeUI;
    
    public void Initialize(Upgrade upgrade, UpgradeUI ui)
    {
        this.upgrade = upgrade;
        this.upgradeUI = ui;
        
        // Setup button
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => upgradeUI.PurchaseUpgrade(upgrade.id));
        }
        
        RefreshUI();
    }
    
    public void RefreshUI()
    {
        if (upgrade == null) return;
        
        // Update texts
        if (nameText != null)
            nameText.text = upgrade.name;
            
        if (descriptionText != null)
            descriptionText.text = upgrade.description;
            
        if (levelText != null)
            levelText.text = $"Lv. {upgrade.currentLevel}/{upgrade.maxLevel}";
            
        if (costText != null)
        {
            if (upgrade.IsMaxLevel())
            {
                costText.text = "MAX";
                costText.color = Color.yellow;
            }
            else
            {
                int cost = upgrade.GetCost();
                costText.text = $"${cost}";
                costText.color = Color.white;
            }
        }
        
        // Update button state
        if (buyButton != null)
        {
            bool canPurchase = !upgrade.IsMaxLevel() && 
                               (CoinSystem.Instance?.CanAfford(upgrade.GetCost()) ?? false);
            
            buyButton.interactable = canPurchase;
            
            Image buttonImage = buyButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                if (upgrade.IsMaxLevel())
                    buttonImage.color = Color.gray;
                else if (canPurchase)
                    buttonImage.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Green
                else
                    buttonImage.color = new Color(0.8f, 0.2f, 0.2f, 1f); // Red
            }
        }
    }
}