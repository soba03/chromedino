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

    [HideInInspector]public bool Beginning = false;
    [HideInInspector] public bool Ending = false;

    [Header("Speed Settings")]
    public float startingSpeed = 0.2f;
    public float speedIncreasePerSecond = 0.01f;
    public float scoreMultiplier = 2f;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject gameEndScreen;

    [Header("Obstacle Spawn")]
    public float MintimeDelayObstacle = 1f;
    public float MaxtimeDelayObstacle = 1f;
    public float obstacleSpeedMultiple = 3f;
    [Space]
    public GameObject[] allGroundObstacles;
    public GameObject[] allFlyObstacles;
    [Space]
    public Transform GroundObstaclesSpawnPoint;
    public Transform FlyObstaclesSpawnPoint;

    private List<GameObject> allCurrentObstacles = new List<GameObject>();
    private List<GameObject> allCurrentShields = new List<GameObject>();

    [Header("SPX")]
    [SerializeField] private AudioSource audio;
    [SerializeField] private AudioClip SFX;

    [Header("Shield Settings")]
    public GameObject shieldPrefab;
    public float shieldSpawnChance = 0.1f;
    private float currentSpeed;

    private int highScore = 0;
    private float currentScore = 0;
    private float timeNextObstacle = 1f;

    [Header("Letter Settings")]
    public GameObject[] letterPrefabs; 
    private int currentLetterIndex = 0; 
    private List<GameObject> allCurrentLetters = new List<GameObject>(); 
    public int ticketCount = 0; 

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

            bool spawnLetter = UnityEngine.Random.value <= 0.3f; 
            
            if (!spawnLetter) 
            {
                if (UnityEngine.Random.value <= shieldSpawnChance)
                {
                    Vector3 shieldSpawnPosition = GroundObstaclesSpawnPoint.position + new Vector3(0, 2f, 0);
                    GameObject newShield = Instantiate(shieldPrefab, shieldSpawnPosition, Quaternion.identity);
                    allCurrentShields.Add(newShield);
                }
                else if (currentScore >= 50)
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
            else 
            {
                Vector3 letterSpawnPosition = GroundObstaclesSpawnPoint.position + new Vector3(0, 2f, 0);
                GameObject newLetter = Instantiate(letterPrefabs[currentLetterIndex], letterSpawnPosition, Quaternion.identity);
                allCurrentLetters.Add(newLetter);
            }
        }

            for (int i = allCurrentObstacles.Count - 1; i >= 0; i--)
            {
                if (allCurrentObstacles[i] != null)
                {
                    allCurrentObstacles[i].transform.Translate(new Vector3(-currentSpeed * Time.deltaTime * obstacleSpeedMultiple, 0, 0));
                    if (allCurrentObstacles[i].transform.position.x < -20f)
                    {
                        Destroy(allCurrentObstacles[i]);
                        allCurrentObstacles.RemoveAt(i);
                    }
                }
                else
                {
                    allCurrentObstacles.RemoveAt(i);
                }
            }

            for (int i = allCurrentShields.Count - 1; i >= 0; i--)
            {
                if (allCurrentShields[i] != null)
                {
                    allCurrentShields[i].transform.Translate(new Vector3(-currentSpeed * Time.deltaTime * obstacleSpeedMultiple, 0, 0));
                    if (allCurrentShields[i].transform.position.x < -20f)
                    {
                        Destroy(allCurrentShields[i]);
                        allCurrentShields.RemoveAt(i);
                    }
                }
                else
                {
                    allCurrentShields.RemoveAt(i);
                }
            }

            for (int i = allCurrentLetters.Count - 1; i >= 0; i--)
            {
                if (allCurrentLetters[i] != null)
                {
                    allCurrentLetters[i].transform.Translate(new Vector3(-currentSpeed * Time.deltaTime * obstacleSpeedMultiple, 0, 0));
                    if (allCurrentLetters[i].transform.position.x < -20f)
                    {
                        Destroy(allCurrentLetters[i]);
                        allCurrentLetters.RemoveAt(i);
                    }
                }
                else
                {
                    allCurrentLetters.RemoveAt(i);
                }
            }

            currentSpeed += Time.deltaTime * speedIncreasePerSecond;
            floorMeshRenderer.material.mainTextureOffset += new Vector2(currentSpeed * Time.deltaTime, y: 0);
            currentScore += currentSpeed * Time.deltaTime * scoreMultiplier;

            if (Mathf.RoundToInt(currentScore) > highScore)
            {
                highScore = Mathf.RoundToInt(currentScore);
                PlayerPrefs.SetInt("Highscore", highScore);
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

    public void CollectLetter(GameObject letter)
    {
        if (letter.name.Contains(letterPrefabs[currentLetterIndex].name))
        {
            currentLetterIndex++;

            if (currentLetterIndex >= letterPrefabs.Length) 
            {
                ticketCount++;
                currentLetterIndex = 0; 
                Debug.Log("You've collected a ticket! Total tickets: " + ticketCount);
            }
        }

        Destroy(letter);
    }
}
