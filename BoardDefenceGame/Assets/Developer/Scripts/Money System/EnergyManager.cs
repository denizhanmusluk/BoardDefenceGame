using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnergyManager : Singleton<EnergyManager>
{
    [Header("UI References")]
    [SerializeField] private Slider energyCooldownSlider;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private Button itemBuyButton;
    [SerializeField] private TextMeshProUGUI itemCostText;

    [Header("Settings")]
    [SerializeField] private float energyPerTick = 1f;
    [SerializeField] private float energyRegenRate = 1f;
    [SerializeField] private float baseItemCost = 5f;

    [Header("Slot References")]
    [SerializeField] private List<SlotBuyButton> slotBuyButtons = new();

    private float _currentEnergy;
    private float _currentItemCost;
    private int _itemBuyCount;
    private Coroutine _regenCoroutine;

    protected override void Awake()
    {
        base.Awake();
        _currentEnergy = Globals.currentEnergy;
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        if (_regenCoroutine != null) StopCoroutine(_regenCoroutine);
        _regenCoroutine = StartCoroutine(RegenEnergyRoutine());
        UpdateEnergyUI();
    }

    private IEnumerator RegenEnergyRoutine()
    {
        energyCooldownSlider.value = 0f;

        while (true)
        {
            float regenSpeed = energyRegenRate;
            float regenTime = 1f / regenSpeed;
            float counter = 0f;

            while (counter < 1f)
            {
                counter += Time.deltaTime / regenTime;
                energyCooldownSlider.value = Mathf.Clamp01(counter);
                yield return null;
            }

            _currentEnergy += energyPerTick;
            Globals.currentEnergy = _currentEnergy;
            UpdateEnergyUI();
        }
    }

    public void BuyItem()
    {
        if (_currentEnergy < _currentItemCost || SlotManager.Instance.allSlotIsFull)
            return;

        _itemBuyCount++;
        _currentEnergy -= _currentItemCost;
        Globals.currentEnergy = _currentEnergy;

        ItemCreatorSubPanel.Instance.ItemCreator(GetItemLevel());
        UpdateEnergyUI();
    }

    private int GetItemLevel()
    {
        int level = Globals.currentLevelIndex / 5;
        return Mathf.Clamp(level, 0, 4);
    }

    public void UpdateEnergyUI()
    {
        _currentItemCost = (baseItemCost + _itemBuyCount / 2f);
        string energyStr = _currentEnergy % 1 == 0 ? _currentEnergy.ToString("F0") : _currentEnergy.ToString("F1");

        energyText.text = energyStr;
        itemCostText.text = _currentItemCost.ToString("F0");
        itemBuyButton.interactable = _currentEnergy >= _currentItemCost;

        foreach (var slot in slotBuyButtons)
            slot.ButtonActiveCheck();
    }

    public void ModifyEnergy(float amount)
    {
        float prev = _currentEnergy;
        _currentEnergy = Mathf.Max(0, _currentEnergy + amount);
        Globals.currentEnergy = _currentEnergy;

        StopAllCoroutines();
        StartCoroutine(SmoothEnergyTransition(prev, _currentEnergy));
    }

    private IEnumerator SmoothEnergyTransition(float from, float to)
    {
        float t = 0f;
        const float speed = 6f;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            float current = Mathf.Lerp(from, to, t);
            energyText.text = current.ToString("F0");
            yield return null;
        }

        UpdateEnergyUI();
        _regenCoroutine = StartCoroutine(RegenEnergyRoutine());
    }
}
