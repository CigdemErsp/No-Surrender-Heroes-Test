using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class ManaSlider : MonoBehaviour
{
    public UnityEngine.UI.Slider slider;
    private float manaRegenRate = 4f; // Sync with ManaManager's regen rate
    private Tween sliderTween;
    private ManaManager manaManager; // Reference to the ManaManager
    [SerializeField] private UnityEngine.UI.Image _level;

    private Sprite[] sprites;
    private Sprite _levelZero;

    private void Start()
    {
        manaManager = FindObjectOfType<ManaManager>(); // Get reference to ManaManager
        InitializeSlider();
        StartSliderFillAnimation(10);

        sprites = Resources.LoadAll<Sprite>("numbers");
        _levelZero = Resources.Load<Sprite>("0");
        // Debug.Log($"sprite {sprites[1].name}");
    }

    private void OnEnable()
    {
        ManaManager.OnManaChange += UpdateManaSlider;
    }

    private void OnDisable()
    {
        ManaManager.OnManaChange -= UpdateManaSlider;
    }

    private void InitializeSlider()
    {
        // Set slider max to match ManaManager's max mana value
        slider.maxValue = 10;
        slider.value = manaManager.getCurrentMana(); // Start at current mana
    }

    private void StartSliderFillAnimation(float currentMana)
    {
        if (slider.value < slider.maxValue) // Only start animation if not at max
        {
            // Kill any existing tween to ensure only one is active
            if (sliderTween != null && sliderTween.IsActive())
            {
                sliderTween.Kill();
            }

            sliderTween = slider.DOValue(slider.value + 1, manaRegenRate)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental) // Restart "restarts" it with same value
            .OnStepComplete(() =>
            {
                // Regenerate mana every time a single unit is filled
                manaManager.RegenerateMana();
                ChangeManaLevel((int)slider.value);
            });
            
        }
        
    }

    private void UpdateManaSlider(int currentMana, int maxMana)
    {
        slider.value = currentMana; // Update the slider's value to reflect current mana
        ChangeManaLevel(currentMana);

        // Stop the animation if current mana reaches max
        if (currentMana >= maxMana)
        {
            sliderTween.Kill(); // Stop the animation
        }
        else
        {
            StartSliderFillAnimation(currentMana); // Restart animation if it was interrupted
        }
    }

    private void ChangeManaLevel(int currentMana)
    {
        if(currentMana == 0)
        {
            _level.sprite = _levelZero;
        }
        else
        {
            _level.sprite = sprites[currentMana - 1];
        }
    } 
}