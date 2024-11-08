using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManager : MonoBehaviour
{
    private int maxMana = 10;
    private int _currentMana;
    private int manaRegenAmount = 1;  // Amount of mana regenerated
    private float manaRegenRate = 4f;

    public delegate void ManaBarHandler(int currentMana, int maxMana); 
    public static event ManaBarHandler OnManaChange;

    public int getCurrentMana() {  return _currentMana; }

    void Awake()
    {
        _currentMana = maxMana;
        // Debug.Log(_currentMana);
    }

    // Regenerate mana over time
    public void RegenerateMana()
    {
        // Regenerate only if mana is below the maximum
        if (_currentMana < maxMana)
        {
            _currentMana += manaRegenAmount;
            Debug.Log($"Mana after regen: {_currentMana}");
        }
    }

    public bool TrySummon(int mana)
    {
        if (_currentMana >= mana)
        {
            _currentMana -= mana;
            Debug.Log($"Mana after summoning: {_currentMana}");
            OnManaChange?.Invoke(_currentMana, maxMana);
            return true;
        }

        return false;
    }

    void OnEnable()
    {
        DragDrop.OnSummon += TrySummon; // Subscribe to the event
    }

    void OnDisable()
    {
        DragDrop.OnSummon -= TrySummon; // Unsubscribe to avoid memory leaks
    }

}
