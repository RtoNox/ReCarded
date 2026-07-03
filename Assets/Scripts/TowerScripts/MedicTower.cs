using UnityEngine;

public class MedicTower : MonoBehaviour
{
    [Header("Medic Stats")]
    public float healRange = 3f;
    public float healInterval = 2f;

    [Tooltip("0.05 = heals 5% of target tower max HP.")]
    public float healPercent = 0.05f;

    [Header("Targeting")]
    public LayerMask towerLayer;

    [Header("Range Visual")]
    public LineRenderer rangeLine;
    public int rangeCircleSegments = 80;
    public float rangeLineWidth = 0.05f;

    private float healCooldown = 0f;

    void Awake()
    {
        SetupRangeLine();
    }

    void Update()
    {
        healCooldown -= Time.deltaTime;

        if (healCooldown <= 0f)
        {
            HealNearbyTowers();
            healCooldown = healInterval;
        }
    }

    void HealNearbyTowers()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            healRange,
            towerLayer
        );

        foreach (Collider2D hit in hits)
        {
            TowerHealth towerHealth = hit.GetComponentInParent<TowerHealth>();

            if (towerHealth == null)
                continue;

            if (towerHealth.gameObject == gameObject)
                continue;

            towerHealth.HealPercent(healPercent);
        }
    }

    void SetupRangeLine()
    {
        if (rangeLine == null)
            return;

        rangeLine.useWorldSpace = false;
        rangeLine.loop = true;
        rangeLine.positionCount = rangeCircleSegments;
        rangeLine.startWidth = rangeLineWidth;
        rangeLine.endWidth = rangeLineWidth;

        DrawRangeCircle();
        rangeLine.gameObject.SetActive(false);
    }

    void DrawRangeCircle()
    {
        if (rangeLine == null)
            return;

        for (int i = 0; i < rangeCircleSegments; i++)
        {
            float angle = ((float)i / rangeCircleSegments) * Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * healRange;
            float y = Mathf.Sin(angle) * healRange;

            rangeLine.SetPosition(i, new Vector3(x, y, 0f));
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, healRange);
    }
}