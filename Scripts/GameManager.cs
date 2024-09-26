using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private MeshRenderer floorMeshRenderer;

    [HideInInspector]public bool Beginning = false;
    [Header ("Speed Settings")]
    public float startingSpeed = 0.2f;
    public float speedIncreasePerSecond = 0.02f;

    private float currentSpeed;

    private void Awake()
    {
        Instance = this;
        currentSpeed = startingSpeed;
    }
    private void Update()
    {
        if (Beginning)
        {
            currentSpeed += Time.deltaTime * speedIncreasePerSecond;
            floorMeshRenderer.material.mainTextureOffset += new Vector2(currentSpeed * Time.deltaTime, 0);
        }
    }
}
