using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int goldAmount = 0;
    public TextMeshProUGUI goldDisplay; // Drag your TextMeshPro UI element here in the Inspector
    private static GameManager instance; // Singleton instance
    private static readonly object lockObject = new object();
    public static GameManager Instance
    {
        get
        {
            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = new GameObject("GameManager").AddComponent<GameManager>();
                }
                return instance;
            }
        }
    }

    // Upgrade Buttons (Drag these in from the Inspector)
    public Button upgradeMiningSpeedButton;
    public Button upgradeMovingSpeedButton;
    public Button hireNewMinerButton;

    // Upgrade Costs
    public int miningSpeedUpgradeCost = 5;
    public int movingSpeedUpgradeCost = 7;
    public int newMinerCost = 10;

    // Upgrade Levels (or Multipliers) and Miner Count
    private int miningSpeedLevel = 1;
    private int movingSpeedLevel = 1;
    private int numberOfMiners = 1;

    // Mining Speed Upgrade Percentage
    public float miningSpeedIncreasePercentage = 0.1f; // 10% increase per level
    // Moving Speed Upgrade Percentage
    public float movingSpeedIncreasePercentage = 0.1f; // 10% increase per level

    // Prefab for Miner
    public GameObject minerPrefab;
    public Transform minerSpawnPoint; // Drag a Transform in the scene where miners should spawn

    // Enum to identify upgrade types for generalization
    public enum UpgradeType
    {
        MiningSpeed,
        MovingSpeed,
        NewMiner
    }

    void Awake()
    {
        // Singleton pattern setup
        lock (lockObject)
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        UpdateGoldDisplay(); // Initialize gold display
        UpdateUpgradeButtonInteractability(); // Initialize button interactability
    }

    void Start()
    {
        // Add button click listeners, now calling the generalized PerformUpgrade function
        upgradeMiningSpeedButton.onClick.AddListener(() => PerformUpgrade(UpgradeType.MiningSpeed));
        upgradeMovingSpeedButton.onClick.AddListener(() => PerformUpgrade(UpgradeType.MovingSpeed));
        hireNewMinerButton.onClick.AddListener(() => PerformUpgrade(UpgradeType.NewMiner));

        // Initial miner spawn on game start
        SpawnNewMiner(); // Start with one miner at the beginning
    }

    public void IncreaseGold(int amount)
    {
        goldAmount += amount;
        UpdateGoldDisplay();
        UpdateUpgradeButtonInteractability(); // Update button interactability whenever gold changes
    }

    private void UpdateGoldDisplay()
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

    public void PerformUpgrade(UpgradeType upgradeType)
    {
        int cost = 0;
        string upgradeName = "";
        ref int levelOrCountVariable = ref miningSpeedLevel; // Default initialization
        ref int costVariableToUpdate = ref miningSpeedUpgradeCost; // Default initialization

        switch (upgradeType)
        {
            case UpgradeType.MiningSpeed:
                cost = miningSpeedUpgradeCost;
                upgradeName = "Mining Speed";
                levelOrCountVariable = ref miningSpeedLevel;
                costVariableToUpdate = ref miningSpeedUpgradeCost;
                break;
            case UpgradeType.MovingSpeed:
                cost = movingSpeedUpgradeCost;
                upgradeName = "Moving Speed";
                levelOrCountVariable = ref movingSpeedLevel;
                costVariableToUpdate = ref movingSpeedUpgradeCost;
                break;
            case UpgradeType.NewMiner:
                cost = newMinerCost;
                upgradeName = "New Miner";
                levelOrCountVariable = ref numberOfMiners;
                costVariableToUpdate = ref newMinerCost;
                break;
            default:
                throw new System.ArgumentException("Unknown Upgrade Type!");
        }

        if (goldAmount >= cost)
        {
            goldAmount -= cost;

            switch (upgradeType)
            {
                case UpgradeType.MiningSpeed:
                    IncreaseMiningSpeed(); // Call mining speed increase logic
                    break;
                case UpgradeType.MovingSpeed:
                    IncreaseMovingSpeed(); // Call moving speed increase logic
                    break;
                case UpgradeType.NewMiner:
                    SpawnNewMiner(); // Call new miner spawn logic
                    break;
            }
            levelOrCountVariable++; // Increment the correct level/count variable
            UpdateGoldDisplay();
            UpdateUpgradeButtonInteractability();
            Debug.Log($"{upgradeName} Upgraded! Level/Count: " + levelOrCountVariable);

            costVariableToUpdate = CalculateNextUpgradeCost(costVariableToUpdate); // Update the specific cost variable
        }
        else
        {
            Debug.Log($"Not enough gold to upgrade {upgradeName}!");
        }
    }

    // --- Upgrade Effect Functions ---

    private void IncreaseMiningSpeed()
    {
        // Apply mining speed increase to all existing miners
        Miner[] miners = FindObjectsOfType<Miner>(); // Assuming you have a Miner script
        foreach (Miner miner in miners)
        {
            miner.IncreaseMiningSpeed(miningSpeedIncreasePercentage); // Call a method in Miner script
        }
        Debug.Log("Mining Speed Increased for all miners!");
    }

    private void IncreaseMovingSpeed()
    {
        // Apply moving speed increase to all existing miners
        Miner[] miners = FindObjectsOfType<Miner>(); // Assuming you have a Miner script
        foreach (Miner miner in miners)
        {
            miner.IncreaseMovingSpeed(movingSpeedIncreasePercentage); // Call a method in Miner script
        }
        Debug.Log("Moving Speed Increased for all miners!");
    }

    private void SpawnNewMiner()
    {
        if (minerPrefab != null && minerSpawnPoint != null)
        {
            Instantiate(minerPrefab, minerSpawnPoint.position, Quaternion.identity);
            numberOfMiners++; // Increment miner count here as well for consistency
            Debug.Log("New Miner Spawned!");
        }
        else
        {
            Debug.LogError("Miner Prefab or Spawn Point not assigned in GameManager!");
        }
    }


    // --- Helper Functions ---

    // Helper function to calculate increased upgrade cost
    private int CalculateNextUpgradeCost(int currentCost)
    {
        return Mathf.RoundToInt(currentCost * 1.5f); // Increase cost by 50% each upgrade
    }

    // Update button interactability based on gold amount
    private void UpdateUpgradeButtonInteractability()
    {
        bool canAffordMiningSpeed = goldAmount >= miningSpeedUpgradeCost;
        bool canAffordMovingSpeed = goldAmount >= movingSpeedUpgradeCost;
        bool canAffordNewMiner = goldAmount >= newMinerCost;

        upgradeMiningSpeedButton.interactable = canAffordMiningSpeed;
        upgradeMovingSpeedButton.interactable = canAffordMovingSpeed;
        hireNewMinerButton.interactable = canAffordNewMiner;
    }

    // Getters for upgrade levels and miner count (for use by other scripts)
    public int GetMiningSpeedLevel()
    {
        return miningSpeedLevel;
    }

    public int GetMovingSpeedLevel()
    {
        return movingSpeedLevel;
    }

    public int GetNumberOfMiners()
    {
        return numberOfMiners;
    }
}