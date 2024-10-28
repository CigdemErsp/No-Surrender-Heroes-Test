using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Avatar;

public class ManaManager : MonoBehaviour
{
    private int maxMana = 10;
    private int _currentMana;
    private int manaRegenAmount = 1;  // Amount of mana regenerated
    private float manaRegenRate = 4f;

    void Start()
    {
        _currentMana = maxMana;
        StartCoroutine(RegenerateMana());
        Debug.Log(_currentMana);
    }

    // Regenerate mana over time
    IEnumerator RegenerateMana()
    {
        while (true)
        {
            if (_currentMana < maxMana)
            {
                _currentMana += manaRegenAmount;
                Debug.Log($"Regenerate {_currentMana}");
            }
            yield return new WaitForSeconds(manaRegenRate);
        }
    }

    public bool TrySummon(int mana)
    {
        if (_currentMana >= mana)
        {
            _currentMana -= mana;
            Debug.Log($"Mana after summoning: {_currentMana}");
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
