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
            for (int i = 0; i < pathHighlightParent.transform.childCount; i++)
            {
                Transform child = pathHighlightParent.transform.GetChild(i);
                SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color color = sr.color;
                    color.a = 0.5f;
                    sr.color = color;
                }
            }
        }
    }

    public void HidePathHighlight()
    {
        if (pathHighlightParent != null)
        {
            for (int i = 0; i < pathHighlightParent.transform.childCount; i++)
            {
                Transform child = pathHighlightParent.transform.GetChild(i);
                SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color color = sr.color;
                    color.a = 0f;
                    sr.color = color;
                }
            }
        }
    }
}