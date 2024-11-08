using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    [SerializeField] private Avatar avatar;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

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
        slider.DOValue(avatar.getCurrentHealth(), 0.5f);
    }

    void Update()
    {
        slider.transform.LookAt(transform.position + _camera.transform.forward);
    }
}
