using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Button _replayButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _quitButton;

    [SerializeField] private Canvas _loadingScreen;
    private Image _loading;

    void Start()
    {
        _loading = _loadingScreen.GetComponentInChildren<Image>();
        _replayButton.onClick.AddListener(Replay);
        _nextButton.onClick.AddListener(NextLevel);
        _quitButton.onClick.AddListener(Quit);
    }

    void Replay()
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

    void NextLevel() { }

    void Quit() 
    {
        // Start the fade-out animation
        _loadingScreen.gameObject.SetActive(true); // Ensure the panel is active
        _loading.color = new Color(0, 0, 0, 0); // Set initial transparent state

        _loading.DOFade(1, 1f) // Fade to black
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // Load the scene after fade-out completes
                SceneManager.LoadScene(0);

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
