using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    [SerializeField] private Avatar avatar;

    // Start is called before the first frame update
    public void InitializeSlider()
    {
        slider.maxValue = avatar.getMaxHealth();
        slider.value = avatar.getCurrentHealth();
    }

    void OnEnable()
    {
        Avatar.OnDamageTaken += UpdateHealthBar; // Subscribe to the event
    }

    void OnDisable()
    {
        Avatar.OnDamageTaken -= UpdateHealthBar; // Unsubscribe to avoid memory leaks
    }

    public void UpdateHealthBar()
    {
        slider.value = avatar.getCurrentHealth();
    }

}
