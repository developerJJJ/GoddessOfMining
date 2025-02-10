using UnityEngine;
using TMPro; // Required for TextMeshPro

public class GameManager : MonoBehaviour
{
    public int goldAmount = 0;
    public TextMeshProUGUI goldDisplay; // Drag your TextMeshPro UI element here in the Inspector
    public static GameManager instance; // Singleton instance

    void Awake()
    {
        // Singleton pattern setup
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one GameManager exists
            return;
        }

        UpdateGoldDisplay(); // Initialize display at start
    }

    public void IncreaseGold(int amount)
    {
        goldAmount += amount;
        UpdateGoldDisplay();
    }

    void UpdateGoldDisplay()
    {
        if (goldDisplay != null)
        {
            goldDisplay.text = "Gold: " + goldAmount.ToString();
        }
        else
        {
            Debug.LogError("Gold Display TextMeshPro is not assigned in GameManager script!");
        }
    }
}