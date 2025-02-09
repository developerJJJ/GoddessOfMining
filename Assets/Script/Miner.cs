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

            if (direction != Vector2.zero && animator != null)
            {
                animator.Play("WalkingAnimation");
            }
        }

        if (isMovingToMinecart)
        {
            rb.linearVelocity = minecartDirection * moveSpeed;
            // Debug.Log("Velocity set in FixedUpdate: " + rb.linearVelocity);
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

        for (int i = 0; i < hitsToMine; i++)
        {
            // if (animator != null)
            // {
            //     animator.Play("MiningAnimation");
            // }

            yield return new WaitForSeconds(miningTime / hitsToMine);
        }

        isMining = false;
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