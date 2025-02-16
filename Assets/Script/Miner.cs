using UnityEngine;
using System.Collections;

public class Miner : MonoBehaviour
{
    public Transform oreTarget;
    public Transform minecart;
    public float moveSpeed = 5f;
    public float miningTime = 2f;
    public int oreQuantity = 0;
    public int hitsToMine = 3;

    private float miningTimer;
    private bool isMining = false;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 minecartDirection;
    private bool isMovingToMinecart = false;
    private SpriteRenderer spriteRenderer;

    private float currentMoveSpeed; // To store current moving speed, affected by upgrades
    private float currentMiningTime; // To store current mining time, affected by upgrades

    private float baseMoveSpeed; // To store the original base move speed
    private float baseMiningTime; // To store the original base mining time

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        oreTarget = FindClosestOre();
        spriteRenderer = GetComponent<SpriteRenderer>();

        baseMoveSpeed = moveSpeed; // Store the original base move speed
        baseMiningTime = miningTime; // Store the original base mining time

        currentMoveSpeed = baseMoveSpeed; // Initialize current move speed with base move speed
        currentMiningTime = baseMiningTime; // Initialize current mining time with base mining time
    }

    void FixedUpdate()
    {
        if (isMovingToMinecart)
        {
            rb.linearVelocity = minecartDirection * currentMoveSpeed;
            animator.SetBool("isWalking", true);
            UpdateSpriteDirection(minecartDirection.x);
        }
        else if (oreTarget != null && !isMining)
        {
            Vector2 direction = (oreTarget.position - transform.position).normalized;
            rb.AddForce(direction * moveSpeed, ForceMode2D.Force);
            animator.SetBool("isWalking", rb.linearVelocity.magnitude > 0.01f);
            UpdateSpriteDirection(direction.x);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    void UpdateSpriteDirection(float horizontalDirection)
    {
        if (horizontalDirection > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalDirection < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ore"))
        {
            oreTarget = other.transform;
            rb.linearVelocity = Vector2.zero;
            StartMining();
            Debug.Log("Miner entered trigger of ore: " + other.gameObject.name);
        }
        else if (other.gameObject.CompareTag("Minecart") && oreQuantity > 0)
        {
            rb.linearVelocity = Vector2.zero;
            Debug.Log("Miner collided with minecart. Depositing ore.");
            DepositOre();
        }
    }

    void StartMining()
    {
        Debug.Log("<color=red>StartMining() function CALLED</color>");
        isMining = true;
        animator.SetBool("isWalking", false);
        UpdateSpriteDirection(0);
        StartCoroutine(MiningAnimationCoroutine());
    }

    IEnumerator MiningAnimationCoroutine()
    {
        Debug.Log("Coroutine started");

        animator.SetBool("isMiningLoopDone", false); // Ensure loop flag is FALSE at start

        for (int i = 0; i < hitsToMine; i++)
        {
            Debug.Log("Hit " + (i + 1) + " - Triggering StartMining animation.");
            animator.SetTrigger("StartMining"); // Trigger StartMining for each hit

            yield return new WaitForSeconds(0.1f); // Give animation time to start

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            while (!stateInfo.IsName("miner_mining"))
            {
                yield return null;
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }

            float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            Debug.Log("Hit " + (i + 1) + " - Animation length: " + animationLength);
            // Delay between hits is now controlled by currentMiningTime
            yield return new WaitForSeconds((currentMiningTime / hitsToMine) - animationLength); // Wait for remaining mining time after animation

            Debug.Log("Hit " + (i + 1) + " - Animation cycle complete.");
        }

        animator.SetBool("isMiningLoopDone", true); // Signal loop is DONE, allow transition to Idle
        oreQuantity++;
        isMining = false;
        MoveToMinecart();
        Debug.Log("MoveToMinecart() called from coroutine");
    }

    void MoveToMinecart()
    {
        Debug.Log("MoveToMinecart() called!");
        if (minecart != null)
        {
            Debug.Log("minecart is NOT null. Moving to minecart.");
            minecartDirection = (minecart.position - transform.position).normalized;
            isMovingToMinecart = true;
        }
        else
        {
            Debug.LogWarning("minecart is null!");
        }
    }

    void DepositOre()
    {
        if (minecart != null)
        {
            minecart.GetComponent<Minecart>().AddOre(oreQuantity);
            oreQuantity = 0;
            Debug.Log("Miner deposited ore. Ore quantity: " + oreQuantity);
            isMovingToMinecart = false;
            oreTarget = FindClosestOre();
        }
    }

    Transform FindClosestOre()
    {
        GameObject[] ores = GameObject.FindGameObjectsWithTag("Ore");
        Transform closestOre = null;
        float closestDistance = Mathf.Infinity;

        if (ores.Length > 0)
        {
            foreach (GameObject ore in ores)
            {
                float distance = Vector2.Distance(transform.position, ore.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestOre = ore.transform;
                }
            }
        }
        return closestOre;
    }

    // --- Upgrade Effect Functions (Called from GameManager) ---

    public void IncreaseMiningSpeed(float percentageIncrease)
    {
        currentMiningTime *= (1f - percentageIncrease); // Reduce current mining time by percentage
        animator.SetFloat("MiningSpeed", baseMiningTime / currentMiningTime); // Update animator parameter to increase speed
        Debug.Log($"Miner Mining Speed Increased! Mining Time reduced to: {currentMiningTime:F2} seconds");
    }

    public void IncreaseMovingSpeed(float percentageIncrease)
    {
        currentMoveSpeed = baseMoveSpeed * (1f + percentageIncrease * GameManager.Instance.GetMovingSpeedLevel()); // Increase move speed based on level
        Debug.Log($"Miner Moving Speed Increased to: {currentMoveSpeed:F2} units/second");
    }
}
