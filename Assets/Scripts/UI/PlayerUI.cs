using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    Slider healthBar;

    [SerializeField]
    Text scoreText;
    int score;

    public static Action OnScoreUpdate;

    private void Awake()
    {
        OnScoreUpdate += ScoreIncrement;
    }

    void ScoreIncrement()
    {
        score += 100;
        scoreText.text = score.ToString();
    }
}