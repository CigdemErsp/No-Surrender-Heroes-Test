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
    [SerializeField] private TMP_Text enemyScoreText;

    private int enemyScore = 0;

    [SerializeField] private GameObject _gameCanvas;
    [SerializeField] private GameObject _victoryScene;
    [SerializeField] private GameObject _defeatScene;

    void OnEnable()
    {
        Unit.ScoreUp += UpScore; // Subscribe to the event
        DragDrop.OnHeroUpgraded += DecreaseScore;
        Unit.OnSoldierDied += UpEnemyScore;
        Unit.OnTurretDestroy += EndGame;
    }

    void OnDisable()
    {
        Unit.ScoreUp -= UpScore; // Unsubscribe to avoid memory leaks
        DragDrop.OnHeroUpgraded -= DecreaseScore;
        Unit.OnSoldierDied -= UpEnemyScore;
        Unit.OnTurretDestroy -= EndGame;
    }

    void EndGame(string teamTag)
    {
        OnGameEnd?.Invoke();
        if (teamTag == "Team 2")
            Defeat();
        else
            Victory();
    }

    public void UpEnemyScore(int points)
    {
        enemyScore += points;
        // Debug.Log(enemyScore);

        string tmp = $"Score: {enemyScore}";
        UpdateEnemyScoreText(tmp); // Update the score text with the current info
    }

    public void UpScore(int point)
    {
        score += point;

        string tmp = $"Score: {score}";
        UpdateScoreText(tmp); // Update the score text with the current info
        OnHeroUpgradable?.Invoke(score);
    }

    void UpdateScoreText(string text = "")
    {
        if (scoreText != null)
        {
            scoreText.text = string.IsNullOrEmpty(text) ? $"Score: {score}" : text;
        }
    }

    void UpdateEnemyScoreText(string text = "")
    {
        if (enemyScoreText != null)
        {
            enemyScoreText.text = string.IsNullOrEmpty(text) ? $"Enemy Score: {enemyScore}" : text;
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
        _gameCanvas.SetActive(false);

        // Set initial scale to 0 for animation effect
        _victoryScene.transform.localScale = Vector3.zero;
        _victoryScene.SetActive(true);

        // Smooth scale-in effect for victory scene
        _victoryScene.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutQuad).SetDelay(0.3f);

        // Find the score text and update it
        GameObject _victoryScore = GameObject.FindGameObjectWithTag("VictoryScore");
        TMP_Text _victoryScoreText = _victoryScore.GetComponent<TMP_Text>();
        _victoryScoreText.text = $"{score}";

        // Find the score text and update it
        GameObject _enemyScore = GameObject.FindGameObjectWithTag("Enemy Score");
        TMP_Text _enemyScoreText = _enemyScore.GetComponent<TMP_Text>();
        _enemyScoreText.text = $"{enemyScore}";

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

        // Set initial alpha to 0 and fade in the score text (using color property for TMP_Text)
        currentColor = _enemyScoreText.color;
        currentColor.a = 0;  // Start with alpha 0
        _enemyScoreText.color = currentColor;  // Apply initial color with alpha 0

        _enemyScoreText.DOFade(1, 0.5f).SetEase(Ease.InOutQuad).SetDelay(delay); // Fade-in after delay
    }

    // Defeat
    void Defeat()
    {
        _gameCanvas.SetActive(false);

        // Set initial scale to 0 for animation effect
        _defeatScene.transform.localScale = Vector3.zero;
        _defeatScene.SetActive(true);

        // Smooth scale-in effect for victory scene
        _defeatScene.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutQuad).SetDelay(0.3f);

        // Find the score text and update it
        GameObject _defeatScore = GameObject.FindGameObjectWithTag("DefeatScore");
        TMP_Text _defeatSceneText = _defeatScore.GetComponent<TMP_Text>();
        _defeatSceneText.text = $"{score}";

        // Find the score text and update it
        GameObject _enemyScore = GameObject.FindGameObjectWithTag("Enemy Score");
        TMP_Text _enemyScoreText = _enemyScore.GetComponent<TMP_Text>();
        _enemyScoreText.text = $"{enemyScore}";

        float delay = 0f;
        // Fade-in all images in the victory scene
        Image[] allImages = _defeatScene.GetComponentsInChildren<Image>();

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
        Color currentColor = _defeatSceneText.color;
        currentColor.a = 0;  // Start with alpha 0
        _defeatSceneText.color = currentColor;  // Apply initial color with alpha 0

        _defeatSceneText.DOFade(1, 0.5f).SetEase(Ease.InOutQuad).SetDelay(delay); // Fade-in after delay

        // Set initial alpha to 0 and fade in the score text (using color property for TMP_Text)
        currentColor = _enemyScoreText.color;
        currentColor.a = 0;  // Start with alpha 0
        _enemyScoreText.color = currentColor;  // Apply initial color with alpha 0

        _enemyScoreText.DOFade(1, 0.5f).SetEase(Ease.InOutQuad).SetDelay(delay); // Fade-in after delay
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
