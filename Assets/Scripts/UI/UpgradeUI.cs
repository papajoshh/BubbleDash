using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class UpgradeUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject upgradePanel;
    public Button upgradeMenuButton;
    public Button closeUpgradeButton;
    
    [Header("Upgrade Item UI")]
    public Transform upgradeContainer;
    public GameObject upgradeItemPrefab; // Will create a prefab for this
    
    [Header("Info Display")]
    public TextMeshProUGUI totalCoinsText;
    public TextMeshProUGUI selectedUpgradeInfo;
    
    private List<UpgradeItemUI> upgradeItems = new List<UpgradeItemUI>();
    private UpgradeSystem upgradeSystem;
    private CoinSystem coinSystem;
    
    void Start()
    {
        upgradeSystem = UpgradeSystem.Instance;
        coinSystem = CoinSystem.Instance;
        
        // Setup buttons
        if (upgradeMenuButton != null)
            upgradeMenuButton.onClick.AddListener(OpenUpgradeMenu);
            
        if (closeUpgradeButton != null)
            closeUpgradeButton.onClick.AddListener(CloseUpgradeMenu);
        
        // Subscribe to events
        if (upgradeSystem != null)
        {
            upgradeSystem.OnUpgradePurchased += OnUpgradePurchased;
            upgradeSystem.OnUpgradeFailed += OnUpgradeFailed;
        }
        
        if (coinSystem != null)
        {
            coinSystem.OnCoinsChanged += UpdateCoinDisplay;
        }
        
        // Initialize UI
        if (upgradePanel != null)
            upgradePanel.SetActive(false);
            
        CreateUpgradeItems();
        UpdateCoinDisplay(coinSystem?.GetCurrentCoins() ?? 0);
    }
    
    void CreateUpgradeItems()
    {
        if (upgradeSystem == null || upgradeContainer == null) return;
        
        // Clear existing items
        foreach (var item in upgradeItems)
        {
            if (item != null && item.gameObject != null)
                Destroy(item.gameObject);
        }
        upgradeItems.Clear();
        
        // Create upgrade items
        var upgrades = upgradeSystem.GetAllUpgrades();
        foreach (var upgrade in upgrades)
        {
            GameObject itemObj = CreateUpgradeItemObject(upgrade);
            if (itemObj != null)
            {
                UpgradeItemUI itemUI = itemObj.GetComponent<UpgradeItemUI>();
                if (itemUI != null)
                {
                    upgradeItems.Add(itemUI);
                    itemUI.Initialize(upgrade, this);
                }
            }
        }
        
        // Force layout rebuild after all items are created
        if (upgradeContainer != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(upgradeContainer.GetComponent<RectTransform>());
        }
    }
    
    GameObject CreateUpgradeItemObject(Upgrade upgrade)
    {
        GameObject itemObj;
        
        if (upgradeItemPrefab != null)
        {
            // Use prefab
            itemObj = Instantiate(upgradeItemPrefab, upgradeContainer);
        }
        else
        {
            // Fallback: Create upgrade item UI manually if prefab doesn't exist
            Debug.LogWarning("UpgradeItemPrefab is null, creating UI manually");
            
            itemObj = new GameObject($"UpgradeItem_{upgrade.id}");
            itemObj.transform.SetParent(upgradeContainer, false);
            
            // Add Image background
            Image bgImage = itemObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            // Add UpgradeItemUI component
            UpgradeItemUI itemUI = itemObj.AddComponent<UpgradeItemUI>();
            
            // Create child elements
            CreateUpgradeItemChildren(itemObj, itemUI);
        }
        
        return itemObj;
    }
    
    void CreateUpgradeItemChildren(GameObject parent, UpgradeItemUI itemUI)
    {
        RectTransform parentRect = parent.GetComponent<RectTransform>();
        
        // Name Text
        GameObject nameObj = new GameObject("NameText");
        nameObj.transform.SetParent(parent.transform, false);
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.text = "Upgrade Name";
        nameText.fontSize = 16f;
        nameText.fontStyle = FontStyles.Bold;
        nameText.color = Color.white;
        RectTransform nameRect = nameText.rectTransform;
        nameRect.anchorMin = new Vector2(0f, 0.6f);
        nameRect.anchorMax = new Vector2(0.7f, 1f);
        nameRect.offsetMin = new Vector2(10f, 0f);
        nameRect.offsetMax = new Vector2(-10f, -5f);
        itemUI.nameText = nameText;
        
        // Description Text
        GameObject descObj = new GameObject("DescriptionText");
        descObj.transform.SetParent(parent.transform, false);
        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.text = "Upgrade description";
        descText.fontSize = 12f;
        descText.color = Color.gray;
        RectTransform descRect = descText.rectTransform;
        descRect.anchorMin = new Vector2(0f, 0.1f);
        descRect.anchorMax = new Vector2(0.7f, 0.6f);
        descRect.offsetMin = new Vector2(10f, 5f);
        descRect.offsetMax = new Vector2(-10f, 0f);
        itemUI.descriptionText = descText;
        
        // Level Text
        GameObject levelObj = new GameObject("LevelText");
        levelObj.transform.SetParent(parent.transform, false);
        TextMeshProUGUI levelText = levelObj.AddComponent<TextMeshProUGUI>();
        levelText.text = "Lv. 0/10";
        levelText.fontSize = 14f;
        levelText.color = Color.cyan;
        levelText.alignment = TextAlignmentOptions.Center;
        RectTransform levelRect = levelText.rectTransform;
        levelRect.anchorMin = new Vector2(0.7f, 0.5f);
        levelRect.anchorMax = new Vector2(0.85f, 1f);
        levelRect.offsetMin = Vector2.zero;
        levelRect.offsetMax = Vector2.zero;
        itemUI.levelText = levelText;
        
        // Buy Button
        GameObject buttonObj = new GameObject("BuyButton");
        buttonObj.transform.SetParent(parent.transform, false);
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);
        Button buyButton = buttonObj.AddComponent<Button>();
        RectTransform buttonRect = buttonImage.rectTransform;
        buttonRect.anchorMin = new Vector2(0.85f, 0.1f);
        buttonRect.anchorMax = new Vector2(0.98f, 0.9f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;
        itemUI.buyButton = buyButton;
        
        // Button Text
        GameObject buttonTextObj = new GameObject("ButtonText");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "$50";
        buttonText.fontSize = 12f;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        RectTransform buttonTextRect = buttonText.rectTransform;
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        itemUI.costText = buttonText;
    }
    
    public void OpenUpgradeMenu()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
            RefreshUpgradeItems();
            
            // Animate panel entrance
            upgradePanel.transform.localScale = Vector3.zero;
            upgradePanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
            
            // Animate items appearing
            AnimateItemsEntrance();
            
            // Pause game without showing pause panel
            if (GameManager.Instance != null)
                GameManager.Instance.PauseGameSilent();
        }
    }
    
    public void CloseUpgradeMenu()
    {
        if (upgradePanel != null)
        {
            // Animate panel exit
            upgradePanel.transform.DOScale(0f, 0.2f)
                .SetEase(Ease.InBack)
                .SetUpdate(true)
                .OnComplete(() => {
                    upgradePanel.SetActive(false);
                    
                    // Resume game
                    if (GameManager.Instance != null)
                        GameManager.Instance.ResumeGame();
                });
        }
    }
    
    void AnimateItemsEntrance()
    {
        // Animate each upgrade item with a slight delay
        for (int i = 0; i < upgradeItems.Count; i++)
        {
            if (upgradeItems[i] != null && upgradeItems[i].gameObject != null)
            {
                GameObject item = upgradeItems[i].gameObject;
                
                // Start invisible and slide in from right
                item.transform.localPosition = new Vector3(300f, item.transform.localPosition.y, 0);
                item.transform.DOLocalMoveX(0f, 0.3f)
                    .SetDelay(0.05f * i)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(true);
                
                // Fade in
                CanvasGroup canvasGroup = item.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    canvasGroup = item.AddComponent<CanvasGroup>();
                
                canvasGroup.alpha = 0f;
                canvasGroup.DOFade(1f, 0.3f).SetDelay(0.05f * i).SetUpdate(true);
            }
        }
    }
    
    void RefreshUpgradeItems()
    {
        foreach (var item in upgradeItems)
        {
            if (item != null)
                item.RefreshUI();
        }
    }
    
    void UpdateCoinDisplay(int coins)
    {
        if (totalCoinsText != null)
            totalCoinsText.text = $"Coins: {coins}";
    }
    
    public void PurchaseUpgrade(string upgradeId)
    {
        if (upgradeSystem != null)
        {
            upgradeSystem.PurchaseUpgrade(upgradeId);
        }
    }
    
    void OnUpgradePurchased(Upgrade upgrade)
    {
        RefreshUpgradeItems();
        
        if (selectedUpgradeInfo != null)
        {
            selectedUpgradeInfo.text = $"{upgrade.name} upgraded to Level {upgrade.currentLevel}!";
            selectedUpgradeInfo.color = Color.green;
            
            // Success animation
            selectedUpgradeInfo.transform.DOScale(1.2f, 0.3f).SetLoops(2, LoopType.Yoyo).SetUpdate(true);
            
            // Coins animation
            if (totalCoinsText != null)
            {
                totalCoinsText.transform.DOShakeScale(0.5f, 0.3f).SetUpdate(true);
            }
        }
    }
    
    void OnUpgradeFailed(string reason)
    {
        if (selectedUpgradeInfo != null)
        {
            selectedUpgradeInfo.text = reason;
            selectedUpgradeInfo.color = Color.red;
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (upgradeSystem != null)
        {
            upgradeSystem.OnUpgradePurchased -= OnUpgradePurchased;
            upgradeSystem.OnUpgradeFailed -= OnUpgradeFailed;
        }
        
        if (coinSystem != null)
        {
            coinSystem.OnCoinsChanged -= UpdateCoinDisplay;
        }
    }
}