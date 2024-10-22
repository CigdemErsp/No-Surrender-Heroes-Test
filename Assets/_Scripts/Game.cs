using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour
{
    private int score = 0;
    [SerializeField] private TMP_Text scoreText;

    void OnEnable()
    {
        Avatar.ScoreUp += UpScore; // Subscribe to the event
    }

    void OnDisable()
    {
        Avatar.ScoreUp -= UpScore; // Unsubscribe to avoid memory leaks
    }

    public void UpScore(int point)
    {
        score += point;

        string tmp = $"Score: {score}";
        UpdateScoreText(tmp); // Update the health text with the current info
    }

    void UpdateScoreText(string text = "")
    {
        if (scoreText != null)
        {
            scoreText.text = string.IsNullOrEmpty(text) ? $"Score: {score}" : text;
        }
    }
}
