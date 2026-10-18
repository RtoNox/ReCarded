using UnityEngine;

/// <summary>
/// A purely visual circle for showing the area covered by an AoE effect.
/// This component does not perform damage, collision checks, or other gameplay logic.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class AreaEffectVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    /// <summary>
    /// Sizes and colors this visual. The supplied radius is in world units.
    /// Pass duration <= 0 to use the prefab's default lifetime.
    /// </summary>
    public void Initialize(float radius, Color color, float duration = -1f)
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            Debug.LogWarning(name + " needs a SpriteRenderer with a sprite assigned.", this);
            Destroy(gameObject);
            return;
        }

        float spriteDiameter = spriteRenderer.sprite.bounds.size.x;
        if (spriteDiameter <= 0f)
        {
            Debug.LogWarning(name + " has a sprite with an invalid width.", this);
            Destroy(gameObject);
            return;
        }

        float diameter = Mathf.Max(0f, radius) * 2f;
        float scale = diameter / spriteDiameter;
        transform.localScale = new Vector3(scale, scale, 1f);
        spriteRenderer.color = color;

        float lifetime = duration > 0f ? duration : 0.5f;
        if (lifetime > 0f)
        {
            Destroy(gameObject, lifetime);
        }
    }

    /// <summary>
    /// Spawns and configures an AreaEffectVisual. If no prefab is assigned, it safely does nothing.
    /// </summary>
    public static void Spawn(
        GameObject prefab,
        Vector3 worldPosition,
        float radius,
        Color color,
        float duration = 0)
    {
        if (prefab == null)
        {
            return;
        }

        GameObject visualObject = Instantiate(prefab, worldPosition, Quaternion.identity);
        AreaEffectVisual visual = visualObject.GetComponent<AreaEffectVisual>();

        if (visual == null)
        {
            Debug.LogWarning(prefab.name + " needs an AreaEffectVisual component.");
            Destroy(visualObject);
            return;
        }

        visual.Initialize(radius, color, duration);
    }
}
