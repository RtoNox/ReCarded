using UnityEngine;

public class PathPlacementHighlighter : MonoBehaviour
{
    [Header("Path Highlight")]
    public GameObject pathHighlightParent;

    void Start()
    {
        HidePathHighlight();
    }

    public void ShowPathHighlight()
    {
        if (pathHighlightParent != null)
        {
            pathHighlightParent.SetActive(true);
        }
    }

    public void HidePathHighlight()
    {
        if (pathHighlightParent != null)
        {
            pathHighlightParent.SetActive(false);
        }
    }
}