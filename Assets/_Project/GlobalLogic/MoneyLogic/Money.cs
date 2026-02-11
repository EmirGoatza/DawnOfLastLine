using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PlayerMoney : MonoBehaviour
{
    [SerializeField] private int startingAmount = 0;

    private int currentAmount;

    public UnityEvent<int> OnMoneyChanged;
    [SerializeField] private TextMeshProUGUI waveText;

    public int CurrentAmount
    {
        get => currentAmount;
        private set
        {
            currentAmount = Mathf.Max(0, value);
            OnMoneyChanged?.Invoke(currentAmount);
            waveText.text = CurrentAmount.ToString();
        }
    }

    void Awake()
    {
        CurrentAmount = startingAmount;
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;
        CurrentAmount += amount;
    }

    public bool Spend(int amount)
    {
        if (amount <= 0) return false;
        if (CurrentAmount < amount) return false;

        CurrentAmount -= amount;
        return true;
    }

    public void ResetMoney()
    {
        CurrentAmount = startingAmount;
    }
}
