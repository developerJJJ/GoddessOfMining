using UnityEngine;

public class Minecart : MonoBehaviour // Make sure it's public!
{
    public int oreStored = 0; // Or whatever data you need

    public void AddOre(int amount)
    {
        oreStored += amount;
        Debug.Log("Minecart now has: " + oreStored + " ore.");
    }

    // Add other methods as needed (e.g., for unloading ore)
}