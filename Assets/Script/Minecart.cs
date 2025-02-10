using UnityEngine;
using TMPro; // Required for TextMeshPro

public class Minecart : MonoBehaviour
{
    public int oreCount = 0;
    public int sellThreshold = 1; // Set to 1 for testing, you can increase later
    public int goldPerOre = 10;
    public TextMeshProUGUI oreCountDisplay; // Drag your TextMeshPro UI element here in the Inspector

    void Start()
    {
        UpdateOreCountDisplay(); // Initialize display at start
    }

    public void AddOre(int amount)
    {
        oreCount += amount;
        UpdateOreCountDisplay();

        if (oreCount >= sellThreshold)
        {
            SellOre();
        }
    }

    public void SellOre()
    {
        GameManager.instance.IncreaseGold(oreCount * goldPerOre);
        oreCount = 0;
        UpdateOreCountDisplay();
        Debug.Log("Ore sold! Gold increased.");
    }

    void UpdateOreCountDisplay()
    {
        if (oreCountDisplay != null)
        {
            oreCountDisplay.text = "Ore: " + oreCount.ToString();
        }
        else
        {
            Debug.LogError("Ore Count Display TextMeshPro is not assigned in Minecart script!");
        }
    }
}