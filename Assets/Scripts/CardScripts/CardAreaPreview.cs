using UnityEngine;

public class CardAreaPreview : MonoBehaviour
{
    [Header("Line Renderer")]
    public LineRenderer lineRenderer;

    [Header("Circle Settings")]
    public int circleSegments = 80;
    public float lineWidth = 0.05f;

    void Awake()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        SetupLine();
        Hide();
    }

    void SetupLine()
    {
        if (lineRenderer == null)
            return;

        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = circleSegments;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    public void Show(float radius)
    {
        if (lineRenderer == null)
            return;

        gameObject.SetActive(true);
        DrawCircle(radius);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetWorldPosition(Vector3 position)
    {
        position.z = 0f;
        transform.position = position;
    }

    void DrawCircle(float radius)
    {
        if (lineRenderer == null)
            return;

        lineRenderer.positionCount = circleSegments;

        for (int i = 0; i < circleSegments; i++)
        {
            float angle = ((float)i / circleSegments) * Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}