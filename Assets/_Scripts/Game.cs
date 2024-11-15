using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Game : MonoBehaviour
{
    public delegate void HeroUpgradable(int score);
    public static event HeroUpgradable OnHeroUpgradable;

    public delegate void GameEnd();
    public static event GameEnd OnGameEnd;

    private int score = 0;
    [SerializeField] private TMP_Text scoreText;

    private bool heroUpgradable = false;

    [SerializeField] private GameObject _victoryScene;
    [SerializeField] private GameObject _defeatScene;

    void OnEnable()
    {
        Avatar.ScoreUp += UpScore; // Subscribe to the event
        DragDrop.OnHeroUpgraded += DecreaseScore;
    }

    void OnDisable()
    {
        Avatar.ScoreUp -= UpScore; // Unsubscribe to avoid memory leaks
        DragDrop.OnHeroUpgraded -= DecreaseScore;
    }

    public void UpScore(int point)
    {
        score += point;

        string tmp = $"Score: {score}";
        UpdateScoreText(tmp); // Update the score text with the current info
        OnHeroUpgradable?.Invoke(score);

        if(score >= 20) // victory
        {
            OnGameEnd?.Invoke();
            Victory();
        }
    }

    void UpdateScoreText(string text = "")
    {
        if (scoreText != null)
        {
            scoreText.text = string.IsNullOrEmpty(text) ? $"Score: {score}" : text;
        }
    }

    void DecreaseScore(int point)
    {
        score -= point;
        string tmp = $"Score: {score}";
        UpdateScoreText(tmp); // Update the score text with the current info
        OnHeroUpgradable?.Invoke(score);
    }

    // Victory
    void Victory()
    {
        GameObject _gameScreen = GameObject.FindGameObjectWithTag("GameCanvas");
        _gameScreen.SetActive(false);

        // Set initial scale to 0 for animation effect
        _victoryScene.transform.localScale = Vector3.zero;
        _victoryScene.SetActive(true);

        // Smooth scale-in effect for victory scene
        _victoryScene.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutQuad).SetDelay(0.3f);

        // Find the score text and update it
        GameObject _victoryScore = GameObject.FindGameObjectWithTag("VictoryScore");
        TMP_Text _victoryScoreText = _victoryScore.GetComponent<TMP_Text>();
        _victoryScoreText.text = $"{score}";

        float delay = 0f;
        // Fade-in all images in the victory scene
        Image[] allImages = _victoryScene.GetComponentsInChildren<Image>();

        // Loop through each Image and fade in
        foreach (Image image in allImages)
        {
            // Ensure image starts with alpha 0, in case it's not set up in the scene
            Color imageColor = image.color;
            imageColor.a = 0;
            image.color = imageColor;

            image.DOFade(1, 0.5f).SetEase(Ease.InOutQuad).SetDelay(delay); // Delay to appear after victory scene
            delay += 0.3f;
        }

        DestroyAllGameObjects();

        // Set initial alpha to 0 and fade in the score text (using color property for TMP_Text)
        Color currentColor = _victoryScoreText.color;
        currentColor.a = 0;  // Start with alpha 0
        _victoryScoreText.color = currentColor;  // Apply initial color with alpha 0

        _victoryScoreText.DOFade(1, 0.5f).SetEase(Ease.InOutQuad).SetDelay(delay); // Fade-in after delay
    }

    // Function to destroy all game objects
    void DestroyAllGameObjects()
    {
        // Find all avatars and enemies and destroy them
        GameObject[] team1 = GameObject.FindGameObjectsWithTag("Team 1");
        GameObject[] team2 = GameObject.FindGameObjectsWithTag("Team 2");

        foreach (GameObject avatar in team1)
        {
            Destroy(avatar);
        }

        foreach (GameObject enemy in team2)
        {
            Destroy(enemy);
        }
    }

}
