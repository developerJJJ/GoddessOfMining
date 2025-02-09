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
    private bool isWalking = false; // Add this flag
    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 minecartDirection;
    private bool isMovingToMinecart = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        oreTarget = FindClosestOre();
    }

    void FixedUpdate()
    {

        if (oreTarget != null && !isMining)
        {
            Vector2 direction = (oreTarget.position - transform.position).normalized;
            rb.AddForce(direction * moveSpeed, ForceMode2D.Force);
            if (animator != null)
            {
                if (rb.linearVelocity.magnitude > 0.1f)
                {
                    animator.SetBool("isWalking", true);
                    isWalking = true; // Set the flag
                }
                else
                {
                    animator.SetBool("isWalking", false);
                    isWalking = false; // Reset the flag
                }
            }
        }

        if (isMovingToMinecart)
        {
            rb.linearVelocity = minecartDirection * moveSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ore"))
        {
            oreTarget = other.transform;
            rb.linearVelocity = Vector2.zero;
            isMining = true;

            animator.SetBool("isMining", true); // Set isMining to true when mining starts
            StartCoroutine(MiningAnimationCoroutine());


            Debug.Log("Miner entered trigger of ore: " + other.gameObject.name);
        }
        else if (other.gameObject.CompareTag("Minecart") && oreQuantity > 0)
        {
            rb.linearVelocity = Vector2.zero;
            Debug.Log("Miner collided with minecart. Depositing ore.");
            DepositOre();
        }
    }

    void Update()
    {
        // No longer needs the distance check for minecart here
    }

    void StartMining()
    {
        isMining = true;
        StartCoroutine(MiningAnimationCoroutine());
    }
    IEnumerator MiningAnimationCoroutine()
    {
        Debug.Log("Coroutine started");

        float totalAnimationTime = 2f; // The total mining duration (2 seconds)
        int totalHits = 3; // The total number of hits
        float timePerHit = totalAnimationTime / totalHits; // Time per hit

        animator.SetBool("isMining", true); // Set animation state

        // Get animation clip duration
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.length > 0)
        {
            animator.speed = (stateInfo.length / timePerHit) * totalHits; // Scale speed
        }
        else
        {
            Debug.LogError("Mining animation length is invalid!");
        }

        for (int i = 0; i < totalHits; i++)
        {
            yield return new WaitForSeconds(timePerHit);
            Debug.Log("Hit " + (i + 1));
        }

        yield return null; // Wait one frame

        isMining = false;
        animator.SetBool("isMining", false); // Stop animation
        animator.speed = 1f; // Reset animation speed

        oreQuantity++;
        MoveToMinecart();
        Debug.Log("MoveToMinecart() called from coroutine");
    }

    void StopMining()
    {
        isMining = false;

        if (animator != null)
        {
            animator.Play("IdleAnimation");
        }
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
            isMovingToMinecart = false; // Important: Stop moving to minecart after deposit

            oreTarget = FindClosestOre();

            if (oreTarget != null)
            {
                Vector2 direction = (oreTarget.position - transform.position).normalized;
                rb.AddForce(direction * moveSpeed, ForceMode2D.Force);
            }
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
        else
        {
            Debug.LogWarning("No ores found in the scene. Make sure they are tagged with 'Ore'.");
        }

        if (closestOre != null)
        {
            Debug.Log("Closest ore found: " + closestOre.name + " at position: " + closestOre.position);
        }
        else
        {
            Debug.LogWarning("No ores found in the scene. Make sure they are tagged with 'Ore'.");
        }
        return closestOre;
    }
}