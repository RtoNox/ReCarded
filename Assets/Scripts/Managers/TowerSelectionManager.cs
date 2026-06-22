using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSelectionManager : MonoBehaviour
{
    [Header("References")]
    public TowerStatsPanelUI towerStatsPanelUI;
    public TowerPlacementManager towerPlacementManager;

    [Header("Selection")]
    public LayerMask towerLayer;

    private Tower selectedTower;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TrySelectTower();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeselectTower();
        }
    }

    void TrySelectTower()
    {
        if (towerPlacementManager != null && towerPlacementManager.IsPlacingTower())
            return;

        if (IsPointerOverUI())
            return;

        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPosition, towerLayer);

        if (hit == null)
        {
            DeselectTower();
            return;
        }

        Tower tower = hit.GetComponent<Tower>();

        if (tower == null)
        {
            tower = hit.GetComponentInParent<Tower>();
        }

        if (tower != null)
        {
            SelectTower(tower);
        }
        else
        {
            DeselectTower();
        }
    }

    void SelectTower(Tower tower)
    {
        if (selectedTower != null)
        {
            selectedTower.SetSelected(false);
        }

        selectedTower = tower;
        selectedTower.SetSelected(true);

        if (towerStatsPanelUI != null)
        {
            towerStatsPanelUI.ShowTower(selectedTower);
        }
    }

    public void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.SetSelected(false);
        }

        selectedTower = null;

        if (towerStatsPanelUI != null)
        {
            towerStatsPanelUI.HidePanel();
        }
    }

    public Tower GetSelectedTower()
    {
        return selectedTower;
    }

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject();
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f;

        return worldPosition;
    }
}