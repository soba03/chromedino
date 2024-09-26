using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private MeshRenderer floorMeshRenderer;

    [HideInInspector]public bool Beginning = false;
    [Header ("Speed Settings")]
    public float startingSpeed = 0.2f;
    public float speedIncreasePerSecond = 0.02f;
    public float scoreMultiplier = 2f;
    private float currentSpeed;

    [Header ("UI")] public TextMeshProUGUI scoreText;
    private int highScore = 0;
    private float currentScore = 0;
    private void Awake()
    {
        Instance = this;
        currentSpeed = startingSpeed;

        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
        }
        UpdateScoreUI();
    }
    private void Update()
    {
        if (Beginning)
        {
            currentSpeed += Time.deltaTime * speedIncreasePerSecond;
            floorMeshRenderer.material.mainTextureOffset += new Vector2(currentSpeed * Time.deltaTime, 0);
            currentScore += currentSpeed * Time.deltaTime * scoreMultiplier;
            if (Mathf.RoundToInt(currentScore) > highScore)
            {    
                highScore = Mathf.RoundToInt(currentScore);
                PlayerPrefs.SetInt("Highscore", highScore);
            }
            UpdateScoreUI();
        }
    }
    private void UpdateScoreUI()
    {
        scoreText.SetText($"HI {highScore.ToString("D5")} {Mathf.RoundToInt(currentScore).ToString("D5")}");
    }
}
