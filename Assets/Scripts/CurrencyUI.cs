using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text cashText;

    void Start()
    {
        UpdateUI();
        CurrencyManager.Instance.OnCashChanged += UpdateUI;
    }

    void UpdateUI()
    {
        if (cashText == null || CurrencyManager.Instance == null)
            return;

        cashText.text = "Cash: $" + CurrencyManager.Instance.GetCurrentCash();
    }
}