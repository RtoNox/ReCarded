using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TowerStatsPanelUI : MonoBehaviour
{
    [Header("References")]
    public TowerSelectionManager towerSelectionManager;

    [Header("Panel")]
    public GameObject panelObject;

    [Header("Tower Info")]
    public TMP_Text towerNameText;
    public Image towerIconImage;

    [Header("Attached Card Icons")]
    public GameObject attachedCardsPanel;
    public Transform attachedCardIconsParent;
    public GameObject attachedCardIconPrefab;

    [Header("Stats")]
    public Transform statRowsParent;
    public GameObject statRowPrefab;

    [Header("Targeting Mode")]
    public Button previousTargetingButton;
    public Button nextTargetingButton;
    public TMP_Text targetingModeText;

    [Header("Sell")]
    public Button sellButton;
    public TMP_Text sellButtonText;

    private Tower selectedTower;

    void Start()
    {
        if (previousTargetingButton != null)
        {
            previousTargetingButton.onClick.AddListener(PreviousTargetingMode);
        }

        if (nextTargetingButton != null)
        {
            nextTargetingButton.onClick.AddListener(NextTargetingMode);
        }

        if (sellButton != null)
        {
            sellButton.onClick.AddListener(SellSelectedTower);
        }

        HidePanel();
    }

    public void ShowTower(Tower tower)
    {
        selectedTower = tower;

        if (selectedTower == null)
        {
            HidePanel();
            return;
        }

        if (panelObject != null)
        {
            panelObject.SetActive(true);
        }

        RefreshPanel();
    }

    public void HidePanel()
    {
        selectedTower = null;

        if (panelObject != null)
        {
            panelObject.SetActive(false);
        }
    }

    void RefreshPanel()
    {
        if (selectedTower == null)
            return;

        UpdateTowerName();
        UpdateTowerIcon();
        UpdateStats();
        UpdateTargetingMode();
        UpdateAttachedCardIcons();
        UpdateSellDisplay();
    }

    void UpdateTowerName()
    {
        if (towerNameText == null)
            return;

        towerNameText.text = selectedTower.towerName;
    }

    void UpdateStats()
    {
        if (statRowsParent == null || statRowPrefab == null)
            return;

        ClearStatRows();

        List<TowerStatDisplayData> stats = selectedTower.GetDisplayStats();

        foreach (TowerStatDisplayData stat in stats)
        {
            GameObject rowObject = Instantiate(statRowPrefab, statRowsParent);

            TowerStatRowUI rowUI = rowObject.GetComponent<TowerStatRowUI>();

            if (rowUI != null)
            {
                rowUI.SetStat(stat.statName, stat.statValue);
            }
        }
    }

    void ClearStatRows()
    {
        if (statRowsParent == null)
            return;

        for (int i = statRowsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(statRowsParent.GetChild(i).gameObject);
        }
    }

    void UpdateTargetingMode()
    {
        if (targetingModeText == null || selectedTower == null)
            return;

        targetingModeText.text = selectedTower.GetTargetingModeName();
    }

    void UpdateSellDisplay()
    {
        if (sellButtonText == null || selectedTower == null)
            return;

        sellButtonText.text = "Sell: $" + selectedTower.GetSellValue();
    }

    void UpdateTowerIcon()
    {
        if (towerIconImage == null || selectedTower == null)
            return;

        towerIconImage.sprite = selectedTower.towerIcon;
        towerIconImage.enabled = selectedTower.towerIcon != null;
    }

    void UpdateAttachedCardIcons()
    {
        if (attachedCardIconsParent == null || attachedCardIconPrefab == null)
            return;

        ClearAttachedCardIcons();

        if (selectedTower == null)
            return;

        if (attachedCardsPanel != null)
        {
            attachedCardsPanel.SetActive(selectedTower.attachedCardIcons.Count > 0);
        }

        foreach (Sprite cardIcon in selectedTower.attachedCardIcons)
        {
            GameObject iconObject = Instantiate(attachedCardIconPrefab, attachedCardIconsParent);

            AttachedCardIconUI iconUI = iconObject.GetComponent<AttachedCardIconUI>();

            if (iconUI != null)
            {
                iconUI.SetIcon(cardIcon);
            }
        }
    }

    void ClearAttachedCardIcons()
    {
        if (attachedCardIconsParent == null)
            return;

        for (int i = attachedCardIconsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(attachedCardIconsParent.GetChild(i).gameObject);
        }
    }

    void PreviousTargetingMode()
    {
        if (selectedTower == null)
            return;

        selectedTower.PreviousTargetingMode();
        UpdateTargetingMode();
    }

    void NextTargetingMode()
    {
        if (selectedTower == null)
            return;

        selectedTower.NextTargetingMode();
        UpdateTargetingMode();
    }

    void SellSelectedTower()
    {
        if (selectedTower == null)
            return;

        Tower towerToSell = selectedTower;

        if (towerSelectionManager != null)
        {
            towerSelectionManager.DeselectTower();
        }
        else
        {
            HidePanel();
        }

        towerToSell.Sell();
    }
}