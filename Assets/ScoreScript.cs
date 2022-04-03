using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreScript : MonoBehaviour
{
    int bestScore;
    int currentScore;

    public static ScoreScript instance;

    public GameObject scoreObj;
    public TextMeshProUGUI scoreText;

    public GameObject bestScoreObj;
    public TextMeshProUGUI bestScoreText;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        bestScore = 0;
        scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
        GameMechanics.onScoreUpdate += AddScore;
        bestScoreText = bestScoreObj.GetComponent<TextMeshProUGUI>();

        if(scoreText == null || bestScoreText == null)
        {
            Debug.LogError("GUI doesn't have reference to score text!");
        }
    }

    public void ClearScore()
    {
        AddScore(-currentScore);
    }

    public void AddScore(int value)
    {
        currentScore += value;
        scoreText.text = currentScore.ToString();

        if(currentScore > bestScore)
        {
            ChangeBestScore();
        }
    }

    void ChangeBestScore()
    {
        bestScore = currentScore;
        bestScoreText.text = bestScore.ToString();
    }
}
