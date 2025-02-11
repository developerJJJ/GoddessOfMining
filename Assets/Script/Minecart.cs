using UnityEngine;
using TMPro; // Required for TextMeshPro

public class Minecart : MonoBehaviour
{
    public int oreCount = 0;
    public int sellThreshold = 1;
    public int goldPerOre = 10;
    public TextMeshPro oreCountDisplay;

    void Start()
    {
        Debug.Log("<color=blue>Minecart Start() called. Initial oreCount: " + oreCount + "</color>"); // Debug log in Start
        UpdateOreCountDisplay();
    }

    public void AddOre(int amount)
    {
        oreCount += amount;
        Debug.Log("<color=green>AddOre() called. Ore added: " + amount + ", New oreCount: " + oreCount + "</color>"); // Debug log in AddOre
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
        Debug.Log("<color=yellow>SellOre() called. Ore sold. oreCount reset to: " + oreCount + "</color>"); // Debug log in SellOre
        UpdateOreCountDisplay();
    }

    void UpdateOreCountDisplay()
    {
        Debug.Log("<color=cyan>UpdateOreCountDisplay() called. Current oreCount: " + oreCount + "</color>"); // Debug log in UpdateOreCountDisplay
        if (oreCountDisplay != null)
        {
            oreCountDisplay.text = "Ore: " + oreCount.ToString();
            Debug.Log("TextMeshPro text updated to: " + oreCountDisplay.text); // Log after text update
        }
        else
        {
            Debug.LogError("Ore Count Display TextMeshPro is not assigned in Minecart script!");
        }
    }
}