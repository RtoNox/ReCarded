using UnityEngine;
using TMPro;

public class TowerStatRowUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text statText;

    public void SetStat(string statName, string statValue)
    {
        if (statText != null)
        {
            statText.text = statName + ": " + statValue;
        }
    }
}