using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Canvas _loadingScreen;
    private Image _loading;

    void Start()
    {
        _loading = _loadingScreen.GetComponentInChildren<Image>();
        _playButton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        _loadingScreen.gameObject.SetActive(true); // Ensure the panel is active
        _loading.color = new Color(0, 0, 0, 0); // Set initial transparent state

        _loading.DOFade(1, 1f) // Fade to black
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // Load the scene after fade-out completes
                SceneManager.LoadScene(1);

                // Fade back in after the new scene is loaded
                _loading.DOFade(0, 0)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        _loadingScreen.gameObject.SetActive(false); // Hide the panel after fade-in
                    });
            });
    }
}
