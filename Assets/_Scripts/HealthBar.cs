using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    [SerializeField] private Unit unit;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    // Start is called before the first frame update
    public void InitializeSlider()
    {
        slider.maxValue = unit.maxHealth;
        slider.value = unit.currentHealth;
    }

    void OnEnable()
    {
        Avatar.OnDamageTaken += UpdateHealthBar; // Subscribe to the event
        Turret.OnDamageTaken += UpdateHealthBar;
    }

    void OnDisable()
    {
        Avatar.OnDamageTaken -= UpdateHealthBar; // Unsubscribe to avoid memory leaks
        Turret.OnDamageTaken -= UpdateHealthBar;
    }

    public void UpdateHealthBar()
    {
        slider.DOValue(unit.currentHealth, 0.5f);
    }

    void Update()
    {
        slider.transform.LookAt(transform.position + _camera.transform.forward);
    }
}
