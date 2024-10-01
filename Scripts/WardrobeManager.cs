using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeManager : MonoBehaviour
{
    public static WardrobeManager Instance;

    [Header("Outfits")]
    public GameObject[] outfits; 

    [Header("Dino Reference")]
    public GameObject dino; 

    private int currentOutfitIndex = -1; 
    private const string OUTFIT_PREF_KEY = "SelectedOutfit"; 
    private const string OUTFIT_UNLOCK_PREFIX = "OutfitUnlocked_"; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        for (int i = 0; i < outfits.Length; i++)
        {
            if (PlayerPrefs.HasKey(OUTFIT_UNLOCK_PREFIX + i))
            {
                bool isUnlocked = PlayerPrefs.GetInt(OUTFIT_UNLOCK_PREFIX + i) == 1;
                outfits[i].SetActive(isUnlocked);
            }
            else
            {
                outfits[i].SetActive(false);
            }
        }

        if (PlayerPrefs.HasKey(OUTFIT_PREF_KEY))
        {
            int savedOutfitIndex = PlayerPrefs.GetInt(OUTFIT_PREF_KEY);
            WearOutfit(savedOutfitIndex); 
        }
    }

    public void WearOutfit(int outfitIndex)
    {
        if (currentOutfitIndex == outfitIndex)
        {
            RemoveCurrentOutfit();
            return;
        }

        if (currentOutfitIndex != -1)
        {
            outfits[currentOutfitIndex].SetActive(true); 
        }

        outfits[outfitIndex].SetActive(true);
        currentOutfitIndex = outfitIndex;

        UpdateDinoOutfit(outfitIndex);

        PlayerPrefs.SetInt(OUTFIT_PREF_KEY, outfitIndex);
        PlayerPrefs.Save();
    }
    public void UnlockOutfit(int outfitIndex)
    {
        if (outfitIndex >= 0 && outfitIndex < outfits.Length)
        {
            outfits[outfitIndex].SetActive(true);
            PlayerPrefs.SetInt(OUTFIT_UNLOCK_PREFIX + outfitIndex, 1); 
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Chỉ số outfitIndex không hợp lệ để mở khóa.");
        }
    }

    public void RemoveCurrentOutfit()
    {
        if (currentOutfitIndex != -1)
        {
            outfits[currentOutfitIndex].SetActive(true); 
            currentOutfitIndex = -1;

            RemoveDinoOutfit();

            PlayerPrefs.DeleteKey(OUTFIT_PREF_KEY);
        }
    }

    private void UpdateDinoOutfit(int outfitIndex)
    {
        foreach (Transform child in dino.transform)
        {
            child.gameObject.SetActive(false);
        }

        if (dino.transform.childCount > outfitIndex)
        {
            dino.transform.GetChild(outfitIndex).gameObject.SetActive(true);
        }
    }

    private void RemoveDinoOutfit()
    {
        foreach (Transform child in dino.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
