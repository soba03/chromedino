using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject gameEndScreen;

    [Header("Obstacle Spawn")]
    public float MintimeDelayObstacle = 1f;
    public float MaxtimeDelayObstacle = 1f;
    public float obstacleSpeedMultiple = 3f;
    [Space]
    public GameObject[] allBehindGroundObstacles;
    public GameObject[] allGroundObstacles;
    public GameObject[] allFlyObstacles;
    [Space]
    public Transform BehindGroundObstaclesSpawnPoint;
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
    private int ticketCount = 5; 

    [Header("Ticket UI")]
    public TextMeshProUGUI ticketText;
    public Button ticketButton;
    
    [Header("Wardrobe UI")]
    public Button wardrobeButton;
    public GameObject wardrobePanel; 

    [Header("Spin Feature")]
    public GameObject giftPanel; 
    public Button spinButton;    
    public GameObject[] giftBoxes; 
    private bool isSpinning = false; 

    private float elapsedTime  = 0.0f;
    private void Awake()
    {
        StartCoroutine(LogTimeCoroutine());

        Instance = this;
        currentSpeed = startingSpeed;

        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
        }
        if (PlayerPrefs.HasKey("TicketCount"))
        {
            ticketCount = PlayerPrefs.GetInt("TicketCount");
        }

        UpdateScoreUI();
        UpdateTicketUI();

        giftPanel.SetActive(false);
        wardrobePanel.SetActive(false);

        spinButton.onClick.AddListener(SpinGiftBox);
        ticketButton.onClick.AddListener(OnTicketButtonPressed);
        wardrobeButton.onClick.AddListener(OnWardrobeButtonPressed);
    }

    System.Collections.IEnumerator LogTimeCoroutine()
    {
        while (true) // Infinite loop to keep logging time
        {
            Debug.Log("Time spent on screen: " + (int)elapsedTime + " seconds");
            yield return new WaitForSeconds(5f); // Wait for 5 seconds
        }
    }
    public void ShowGameEndScreen()
    {
        gameEndScreen.SetActive(true);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

    if (Beginning && !Ending)
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
                    if (UnityEngine.Random.value > 1.0f)
                    {
                        // flying obstacle
                        GameObject newObstacle = Instantiate(allFlyObstacles[UnityEngine.Random.Range(0, allFlyObstacles.Length)], FlyObstaclesSpawnPoint.position, Quaternion.identity);
                        allCurrentObstacles.Add(newObstacle);
                    }
                    else if (UnityEngine.Random.value > 0.0f)
                    {
                        Debug.Log("Generated a weed");
                        // obstacle from behind
                        GameObject newObstacle = Instantiate(allBehindGroundObstacles[UnityEngine.Random.Range(0, allBehindGroundObstacles.Length)], BehindGroundObstaclesSpawnPoint.position, Quaternion.identity);
                        allCurrentObstacles.Add(newObstacle);
                    }
                    else
                    {
                        // catus
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
                allCurrentObstacles.Add(newLetter);
            }
        }

            for (int i = allCurrentObstacles.Count - 1; i >= 0; i--)
            {
                if (allCurrentObstacles[i] != null)
                {
                    // special obstacle
                    if (allCurrentObstacles[i].CompareTag("BehindObstacle"))
                    {
                        allCurrentObstacles[i].transform.Translate(new Vector3(currentSpeed * Time.deltaTime * obstacleSpeedMultiple, 0, 0));
                        if (allCurrentObstacles[i].transform.position.x > 20f)
                        {
                            Destroy(allCurrentObstacles[i]);
                            allCurrentObstacles.RemoveAt(i);
                        }
                    }
                    else
                    {
                        allCurrentObstacles[i].transform.Translate(new Vector3(-currentSpeed * Time.deltaTime * obstacleSpeedMultiple, 0, 0));
                        if (allCurrentObstacles[i].transform.position.x < -20f)
                        {
                            Destroy(allCurrentObstacles[i]);
                            allCurrentObstacles.RemoveAt(i);
                        }
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
            currentSpeed += Time.deltaTime * speedIncreasePerSecond;
            floorMeshRenderer.material.mainTextureOffset += new Vector2(currentSpeed * Time.deltaTime, y: 0);
            currentScore += currentSpeed * Time.deltaTime * scoreMultiplier;

            if (Mathf.RoundToInt(currentScore) > highScore)
            {
                highScore = Mathf.RoundToInt(currentScore);
                PlayerPrefs.SetInt("HighScore", highScore);
            }

            UpdateScoreUI();
            UpdateTicketUI();
        }
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

    private void UpdateTicketUI()
    {
        ticketText.SetText($"{ticketCount}");
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
                PlayerPrefs.SetInt("TicketCount", ticketCount); 
                PlayerPrefs.Save(); 
            }
        }

        Destroy(letter);
    }

    public void OnTicketButtonPressed()
    {   
        if (giftPanel.activeSelf)
    {
        giftPanel.SetActive(false); 
    }
    else
    {
        giftPanel.SetActive(true);  
    }
    }
    public void OnWardrobeButtonPressed()
    {   
        if (wardrobePanel.activeSelf)
    {
        wardrobePanel.SetActive(false); 
    }
    else
    {
        wardrobePanel.SetActive(true);  
    }
    }
    private void SpinGiftBox()
    {
        if (isSpinning || ticketCount <= 0) return;
        isSpinning = true;
        ticketCount--;
        UpdateTicketUI();
        PlayerPrefs.SetInt("TicketCount", ticketCount);
        PlayerPrefs.Save();
        StartCoroutine(SpinEffect(UnityEngine.Random.Range(0, giftBoxes.Length)));
    }

    private IEnumerator SpinEffect(int finalIndex)
    {
        int currentBoxIndex = 0, totalRounds = 3, totalBoxes = giftBoxes.Length;

        for (int i = 0; i < totalRounds * totalBoxes + finalIndex; i++)
        {
            ResetBoxHighlight(currentBoxIndex);
            currentBoxIndex = i % totalBoxes;
            HighlightBox(currentBoxIndex);
            yield return new WaitForSeconds(Mathf.Lerp(0.1f, 0.5f, (float)i / (totalRounds * totalBoxes + finalIndex)));
        }

        StartCoroutine(ShowResult(finalIndex));
        isSpinning = false;
    }
    private void HighlightBox(int index)
    {
        giftBoxes[index].transform.localScale = Vector3.one * 1.2f; 
    }
    private void ResetBoxHighlight(int index)
    {
        giftBoxes[index].transform.localScale = Vector3.one; 
    }
    private IEnumerator ShowResult(int finalIndex)
    {
        yield return new WaitForSeconds(3f);
        switch (finalIndex)
        {
            case 1: UnlockWardrobeButton(1); break;
            case 2: UnlockWardrobeButton(5); break;
            case 3: ShowNoRewardMessage(); break;
            case 4: UnlockWardrobeButton(3); break;
            case 5: ShowNoRewardMessage(); break;
            case 6: UnlockWardrobeButton(2); break;
            case 7: UnlockWardrobeButton(4); break;
            case 8: UnlockWardrobeButton(0); break;
            case 9: ShowNoRewardMessage(); break;
            default: Debug.LogError("Invalid finalIndex!"); break;
        }
    }

    private void UnlockWardrobeButton(int buttonIndex)
    {   
        WardrobeManager.Instance.UnlockOutfit(buttonIndex);
    }

    private void ShowNoRewardMessage() => Debug.Log("No reward this time.");
}
