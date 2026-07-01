using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPreviewUI : MonoBehaviour
{
    [Header("Preview Panel")]
    public GameObject previewPanel;

    [Header("Animated Card Preview")]
    public AnimatedCardArtUI animatedCardArt;

    [Header("Effect Description")]
    public TMP_Text effectDescriptionText;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        DisableRaycastsOnChildren();

        HidePreview();
    }

    void DisableRaycastsOnChildren()
    {
        Graphic[] graphics = GetComponentsInChildren<Graphic>(true);

        foreach (Graphic graphic in graphics)
        {
            graphic.raycastTarget = false;
        }
    }

    public void ShowPreview(CardData cardData)
    {
        if (cardData == null)
            return;

        if (previewPanel != null)
        {
            previewPanel.SetActive(true);
        }

        if (animatedCardArt != null)
        {
            animatedCardArt.SetAnimation(
                cardData.cardAnimationFrames,
                cardData.animationFrameRate
            );
        }

        if (effectDescriptionText != null)
        {
            effectDescriptionText.text = cardData.effectDescription;
        }
    }

    public void HidePreview()
    {
        if (previewPanel != null)
        {
            previewPanel.SetActive(false);
        }
    }
}