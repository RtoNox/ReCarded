using UnityEngine;

public class TowerPlacementPreview : MonoBehaviour
{
    [Header("Preview Visuals")]
    public SpriteRenderer[] spriteRenderers;
    public LineRenderer rangeLine;

    [Header("Colors")]
    public Color validColor = new Color(1f, 1f, 1f, 0.45f);
    public Color invalidColor = new Color(1f, 0.2f, 0.2f, 0.45f);
    public Color rangeValidColor = new Color(1f, 1f, 1f, 0.8f);
    public Color rangeInvalidColor = new Color(1f, 0.2f, 0.2f, 0.8f);

    [Header("Range Circle")]
    public int circleSegments = 80;
    public float rangeLineWidth = 0.05f;

    private float currentRange = 3f;

    void Awake()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        SetupRangeLine();
    }

    void SetupRangeLine()
    {
        if (rangeLine == null)
            return;

        rangeLine.useWorldSpace = false;
        rangeLine.loop = true;
        rangeLine.positionCount = circleSegments;
        rangeLine.startWidth = rangeLineWidth;
        rangeLine.endWidth = rangeLineWidth;
    }

    public void SetRange(float range)
    {
        currentRange = range;
        DrawRangeCircle();
    }

    public void SetPlacementValid(bool isValid)
    {
        Color spriteColor = isValid ? validColor : invalidColor;
        Color lineColor = isValid ? rangeValidColor : rangeInvalidColor;

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = spriteColor;
            }
        }

        if (rangeLine != null)
        {
            rangeLine.startColor = lineColor;
            rangeLine.endColor = lineColor;
        }
    }

    void DrawRangeCircle()
    {
        if (rangeLine == null)
            return;

        rangeLine.positionCount = circleSegments;

        for (int i = 0; i < circleSegments; i++)
        {
            float angle = ((float)i / circleSegments) * Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * currentRange;
            float y = Mathf.Sin(angle) * currentRange;

            rangeLine.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}