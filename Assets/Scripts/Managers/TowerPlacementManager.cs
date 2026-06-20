using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class TowerPlacementManager : MonoBehaviour
{
    [Header("Tower Settings")]
    public GameObject basicTowerPrefab;
    public int basicTowerCost = 50;
    public int towerLimit = 5;

    [Header("Placement Settings")]
    public LayerMask blockedPlacementLayer;
    public float placementCheckRadius = 0.4f;

    [Header("Placement Preview")]
    public GameObject basicTowerPreviewPrefab;
    public PathPlacementHighlighter pathPlacementHighlighter;

    [Header("UI")]
    public TMP_Text towerLimitText;
    public TMP_Text placementMessageText;
    public Button basicTowerButton;
    public GameObject waveCallPanel;

    [Header("Message Settings")]
    public float messageDisplayTime = 3f;

    private Coroutine messageCoroutine;

    public event Action<bool> OnPlacementStateChanged;

    private bool isPlacingTower = false;
    private int placedTowerCount = 0;

    private GameObject currentPreviewObject;
    private TowerPlacementPreview currentPreview;

    void Start()
    {
        UpdateUI();

        if (placementMessageText != null)
        {
            placementMessageText.text = "";
        }
    }

    void Update()
    {
        if (!isPlacingTower)
            return;

        UpdatePlacementPreview();

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceTower();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    public void StartPlacingBasicTower()
    {
        if (basicTowerPrefab == null)
        {
            Debug.LogWarning("No basic tower prefab assigned!");
            return;
        }

        if (placedTowerCount >= towerLimit)
        {
            ShowMessage("Tower limit reached!");
            return;
        }

        if (CurrencyManager.Instance == null)
        {
            Debug.LogWarning("No CurrencyManager found in scene!");
            return;
        }

        if (!CurrencyManager.Instance.CanAfford(basicTowerCost))
        {
            ShowMessage("Not enough cash!");
            return;
        }

        SetPlacementState(true);

        CreatePlacementPreview();

        if (pathPlacementHighlighter != null)
        {
            pathPlacementHighlighter.ShowPathHighlight();
        }

        ShowMessage("Click to place tower. Right-click to cancel.");
    }

    void CreatePlacementPreview()
    {
        DestroyPlacementPreview();

        if (basicTowerPreviewPrefab == null)
            return;

        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        currentPreviewObject = Instantiate(
            basicTowerPreviewPrefab,
            mouseWorldPosition,
            Quaternion.identity
        );

        currentPreview = currentPreviewObject.GetComponent<TowerPlacementPreview>();

        Tower towerStats = basicTowerPrefab.GetComponent<Tower>();

        if (currentPreview != null && towerStats != null)
        {
            currentPreview.SetRange(towerStats.range);
        }
    }

    void UpdatePlacementPreview()
    {
        if (currentPreviewObject == null)
            return;

        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        currentPreviewObject.transform.position = mouseWorldPosition;

        bool canPlace = CanPlaceAt(mouseWorldPosition);

        if (currentPreview != null)
        {
            currentPreview.SetPlacementValid(canPlace);
        }
    }

    void TryPlaceTower()
    {
        Vector3 worldPosition = GetMouseWorldPosition();

        if (!CanPlaceAt(worldPosition))
        {
            ShowMessage("Can't place tower there!");
            return;
        }

        if (placedTowerCount >= towerLimit)
        {
            ShowMessage("Tower limit reached!");
            CancelPlacement();
            return;
        }

        if (CurrencyManager.Instance == null)
        {
            Debug.LogWarning("No CurrencyManager found in scene!");
            CancelPlacement();
            return;
        }

        if (!CurrencyManager.Instance.SpendCash(basicTowerCost))
        {
            ShowMessage("Not enough cash!");
            CancelPlacement();
            return;
        }

        Instantiate(basicTowerPrefab, worldPosition, Quaternion.identity);

        placedTowerCount++;

        SetPlacementState(false);

        DestroyPlacementPreview();

        if (pathPlacementHighlighter != null)
        {
            pathPlacementHighlighter.HidePathHighlight();
        }

        ClearMessage();
        UpdateUI();
    }

    bool CanPlaceAt(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(
            position,
            placementCheckRadius,
            blockedPlacementLayer
        );

        return hit == null;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f;

        return worldPosition;
    }

    void CancelPlacement()
    {
        SetPlacementState(false);

        DestroyPlacementPreview();

        if (pathPlacementHighlighter != null)
        {
            pathPlacementHighlighter.HidePathHighlight();
        }

        ClearMessage();
    }

    void DestroyPlacementPreview()
    {
        if (currentPreviewObject != null)
        {
            Destroy(currentPreviewObject);
        }

        currentPreviewObject = null;
        currentPreview = null;
    }

    void SetPlacementState(bool placing)
    {
        isPlacingTower = placing;
        OnPlacementStateChanged?.Invoke(isPlacingTower);

        if (basicTowerButton != null)
        {
            basicTowerButton.interactable = !isPlacingTower;
        }

        if (waveCallPanel != null)
        {
            waveCallPanel.SetActive(!isPlacingTower);
        }
    }

    void UpdateUI()
    {
        if (towerLimitText != null)
        {
            towerLimitText.text = "Towers: " + placedTowerCount + " / " + towerLimit;
        }
    }

    void ShowMessage(string message)
    {
        if (placementMessageText == null)
            return;

        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }

        placementMessageText.text = message;

        if (!string.IsNullOrEmpty(message))
        {
            Debug.Log(message);
            messageCoroutine = StartCoroutine(ClearMessageAfterDelay());
        }
    }

    IEnumerator ClearMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDisplayTime);

        if (placementMessageText != null)
        {
            placementMessageText.text = "";
        }

        messageCoroutine = null;
    }

    void ClearMessage()
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }

        if (placementMessageText != null)
        {
            placementMessageText.text = "";
        }
    }

    public int GetPlacedTowerCount()
    {
        return placedTowerCount;
    }

    public int GetTowerLimit()
    {
        return towerLimit;
    }

    public bool IsPlacingTower()
    {
        return isPlacingTower;
    }
}