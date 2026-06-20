using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [Header("Currency")]
    public int startingCash = 100;

    private int currentCash;

    public event Action OnCashChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        currentCash = startingCash;
        OnCashChanged?.Invoke();
    }

    public bool CanAfford(int amount)
    {
        return currentCash >= amount;
    }

    public bool SpendCash(int amount)
    {
        if (!CanAfford(amount))
            return false;

        currentCash -= amount;
        OnCashChanged?.Invoke();

        return true;
    }

    public void AddCash(int amount)
    {
        currentCash += amount;
        OnCashChanged?.Invoke();
    }

    public int GetCurrentCash()
    {
        return currentCash;
    }
}