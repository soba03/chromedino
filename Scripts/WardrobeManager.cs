using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeManager : MonoBehaviour
{
    [Header("Outfits")]
    public GameObject[] outfits; // Mảng chứa các trang phục

    [Header("Dino Reference")]
    public GameObject dino; // Reference to the Dino in the game scene

    private int currentOutfitIndex = -1; // Chỉ số trang phục hiện tại
    private const string OUTFIT_PREF_KEY = "SelectedOutfit"; // Khóa để lưu trang phục

    private void Start()
    {
        // Kiểm tra và áp dụng trang phục đã lưu khi khởi động
        if (PlayerPrefs.HasKey(OUTFIT_PREF_KEY))
        {
            int savedOutfitIndex = PlayerPrefs.GetInt(OUTFIT_PREF_KEY);
            WearOutfit(savedOutfitIndex); // Áp dụng trang phục đã lưu
        }
    }

    // Hàm gọi khi nhấn nút để mặc trang phục
    public void WearOutfit(int outfitIndex)
    {
        // Nếu trang phục đang được chọn là trang phục hiện tại, thì cởi nó ra
        if (currentOutfitIndex == outfitIndex)
        {
            RemoveCurrentOutfit(); // Cởi trang phục nếu trang phục đã được mặc
            return; // Thoát ra khỏi hàm, không làm gì thêm
        }

        // Nếu đang mặc trang phục khác, ẩn trang phục cũ
        if (currentOutfitIndex != -1)
        {
            outfits[currentOutfitIndex].SetActive(false);
        }

        // Mặc trang phục mới
        outfits[outfitIndex].SetActive(true);
        currentOutfitIndex = outfitIndex; 

        // Cập nhật trang phục cho Dino
        UpdateDinoOutfit(outfitIndex);

        // Lưu trang phục đã chọn vào PlayerPrefs
        PlayerPrefs.SetInt(OUTFIT_PREF_KEY, outfitIndex);
        PlayerPrefs.Save();
    }

    // Hàm gọi khi tháo trang phục hiện tại
    public void RemoveCurrentOutfit()
    {
        if (currentOutfitIndex != -1)
        {
            outfits[currentOutfitIndex].SetActive(false); 
            currentOutfitIndex = -1;

            RemoveDinoOutfit();

            // Xóa trang phục đã lưu trong PlayerPrefs
            PlayerPrefs.DeleteKey(OUTFIT_PREF_KEY);
        }
    }

    // Cập nhật trang phục của Dino theo chỉ số outfitIndex
    private void UpdateDinoOutfit(int outfitIndex)
    {
        // Ẩn tất cả các trang phục của Dino trước khi áp dụng mới
        foreach (Transform child in dino.transform)
        {
            child.gameObject.SetActive(false); 
        }

        // Hiển thị trang phục tương ứng với outfitIndex
        if (dino.transform.childCount > outfitIndex)
        {
            dino.transform.GetChild(outfitIndex).gameObject.SetActive(true);
        }
    }

    // Tháo trang phục của Dino
    private void RemoveDinoOutfit()
    {
        foreach (Transform child in dino.transform)
        {
            child.gameObject.SetActive(false); 
        }
    }
}
