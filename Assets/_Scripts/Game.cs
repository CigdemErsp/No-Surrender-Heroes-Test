using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour
{
    public delegate void HeroUpgradable(int score);
    public static event HeroUpgradable OnHeroUpgradable;

    private int score = 0;
    [SerializeField] private TMP_Text scoreText;

    private bool heroUpgradable = false;

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
    
}
