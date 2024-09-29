using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private MeshRenderer floorMeshRenderer;

    [HideInInspector] public bool Beginning = false;
    [HideInInspector] public bool Ending = false;


    [Header("Speed Settings")]

    public float startingSpeed = 0.2f;
    public float speedIncreasePerSecond = 0.01f;
    public float scoreMultiplier = 2f;

    [Header("UI")] public TextMeshProUGUI scoreText;
    public GameObject gameEndScreen;

    [Header("Obstacle Spawn")] public float MintimeDelayObstacle = 1f;
    public float MaxtimeDelayObstacle = 1f;
    public float obstacleSpeedMultiple = 3f;
    [Space]
    public GameObject[] allGroundObstacles;
    public GameObject[] allFlyObstacles;
    [Space]
    public Transform GroundObstaclesSpawnPoint;
    public Transform FlyObstaclesSpawnPoint;

    private List<GameObject> allCurrentObstacles = new List<GameObject>();

    [Header("SPX")][SerializeField] private AudioSource audio;
    [SerializeField] private AudioClip SFX;

    [Header("Shield Settings")]
    public GameObject shieldPrefab;
    public float shieldSpawnChance = 0.1f;
    private float currentSpeed;

    private int highScore = 0;
    private float currentScore = 0;

    private float timeNextObstacle = 1f;

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

    public void ShowGameEndScreen()
    {
        gameEndScreen.SetActive(true);
    }

    private void Update()
    {
        if (Beginning && !Ending)
        {
            timeNextObstacle -= Time.deltaTime * currentSpeed;

            if (timeNextObstacle <= 0)
            {
                timeNextObstacle = UnityEngine.Random.Range(MintimeDelayObstacle, MaxtimeDelayObstacle);

                // Tạo chướng ngại vật
                if (UnityEngine.Random.value <= shieldSpawnChance) // Xác suất xuất hiện khiên
                {
                    Vector3 shieldSpawnPosition = GroundObstaclesSpawnPoint.position + new Vector3(0, 2f, 0);
                    GameObject newShield = Instantiate(shieldPrefab, shieldSpawnPosition, Quaternion.identity);
                    allCurrentObstacles.Add(newShield);
                }
                else if (currentScore >= 500)
                {
                    if (UnityEngine.Random.value > 0.8f)
                    {
                        GameObject newObstacle = Instantiate(allFlyObstacles[UnityEngine.Random.Range(0, allFlyObstacles.Length)], FlyObstaclesSpawnPoint.position, Quaternion.identity);
                        allCurrentObstacles.Add(newObstacle);
                    }
                    else
                    {
                        GameObject newObstacle = Instantiate(allGroundObstacles[UnityEngine.Random.Range(0, allGroundObstacles.Length)], GroundObstaclesSpawnPoint.position, Quaternion.identity);
                        allCurrentObstacles.Add(newObstacle);
                    }
                }
                else
                {
                    GameObject newObstacle = Instantiate(allGroundObstacles[UnityEngine.Random.Range(0, allGroundObstacles.Length)], GroundObstaclesSpawnPoint.position, Quaternion.identity);
                    allCurrentObstacles.Add(newObstacle);
                }
            }

            for (int i = allCurrentObstacles.Count - 1; i >= 0; i--)
            {
                if (allCurrentObstacles[i] != null)
                {
                    allCurrentObstacles[i].transform.Translate(new Vector3(-currentSpeed * Time.deltaTime * obstacleSpeedMultiple, 0, 0));
                }
                else
                {
                    allCurrentObstacles.RemoveAt(i);
                }
                // Destroy if out of frame
                if (allCurrentObstacles[i].transform.position.x < -15)
                {
                    Destroy(allCurrentObstacles[i]);
                }
            }

            currentSpeed += Time.deltaTime * speedIncreasePerSecond;
            floorMeshRenderer.material.mainTextureOffset += new Vector2(currentSpeed * Time.deltaTime, y: 0);
            currentScore += currentSpeed * Time.deltaTime * scoreMultiplier;

            if (Mathf.RoundToInt(currentScore) > highScore)
            {
                highScore = Mathf.RoundToInt(currentScore);
                PlayerPrefs.SetInt("HighScore", highScore);
            }

            UpdateScoreUI();
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private void UpdateScoreUI()
    {
        scoreText.SetText($"HI {highScore.ToString("D5")} {Mathf.RoundToInt(currentScore).ToString("D5")}");
    }
}
