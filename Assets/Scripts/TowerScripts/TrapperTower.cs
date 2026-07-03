using UnityEngine;

public class TrapperTower : MonoBehaviour
{
    [Header("Trap Placement")]
    public GameObject trapPrefab;
    public LayerMask enemyPathLayer;
    public LayerMask trapBlockedLayer;

    public float trapPlacementRange = 3f;
    public float trapPlacementInterval = 4f;
    public int maxActiveTraps = 3;

    [Header("Trap Stats")]
    public int trapDamage = 10;
    public bool trapsDestroyAfterTrigger = true;

    [Header("Placement Attempts")]
    public int placementAttempts = 20;
    public float pathCheckRadius = 0.2f;
    public float trapCheckRadius = 0.3f;

    [Header("Range Visual")]
    public LineRenderer rangeLine;
    public int rangeCircleSegments = 80;
    public float rangeLineWidth = 0.05f;

    private float placementCooldown = 0f;
    private int activeTrapCount = 0;

    void Awake()
    {
        SetupRangeLine();
    }

    void Update()
    {
        placementCooldown -= Time.deltaTime;

        if (placementCooldown <= 0f)
        {
            TryPlaceTrap();
            placementCooldown = trapPlacementInterval;
        }
    }

    void TryPlaceTrap()
    {
        if (trapPrefab == null)
        {
            Debug.LogWarning(name + " has no trap prefab assigned!");
            return;
        }

        if (activeTrapCount >= maxActiveTraps)
            return;

        for (int i = 0; i < placementAttempts; i++)
        {
            Vector3 randomPosition = GetRandomPointInRange();

            if (!IsValidTrapPosition(randomPosition))
                continue;

            GameObject trapObject = Instantiate(
                trapPrefab,
                randomPosition,
                Quaternion.identity
            );

            Trap trap = trapObject.GetComponent<Trap>();

            if (trap != null)
            {
                trap.Initialize(trapDamage, trapsDestroyAfterTrigger);
            }

            TrapLifetime trapLifetime = trapObject.GetComponent<TrapLifetime>();

            if (trapLifetime == null)
            {
                trapLifetime = trapObject.AddComponent<TrapLifetime>();
            }

            trapLifetime.Initialize(this);

            activeTrapCount++;

            return;
        }
    }

    Vector3 GetRandomPointInRange()
    {
        Vector2 randomCircle = Random.insideUnitCircle * trapPlacementRange;

        Vector3 randomPosition = transform.position + new Vector3(
            randomCircle.x,
            randomCircle.y,
            0f
        );

        randomPosition.z = 0f;

        return randomPosition;
    }

    bool IsValidTrapPosition(Vector3 position)
    {
        Collider2D pathHit = Physics2D.OverlapCircle(
            position,
            pathCheckRadius,
            enemyPathLayer
        );

        if (pathHit == null)
            return false;

        Collider2D blockedHit = Physics2D.OverlapCircle(
            position,
            trapCheckRadius,
            trapBlockedLayer
        );

        return blockedHit == null;
    }

    public void NotifyTrapRemoved()
    {
        activeTrapCount--;

        if (activeTrapCount < 0)
        {
            activeTrapCount = 0;
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

            float x = Mathf.Cos(angle) * trapPlacementRange;
            float y = Mathf.Sin(angle) * trapPlacementRange;

            rangeLine.SetPosition(i, new Vector3(x, y, 0f));
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, trapPlacementRange);
    }
}