using UnityEngine;
using UnityEngine.UI;

public class AttachedCardIconUI : MonoBehaviour
{
    [Header("UI")]
    public Image cardIconImage;

    public void SetIcon(Sprite icon)
    {
        if (cardIconImage == null)
            return;

        cardIconImage.sprite = icon;
        cardIconImage.enabled = icon != null;
    }
}