using System.Collections;
using UnityEngine;

public class DoorAi : MonoBehaviour
{
    [Header("Simple Enemy AI Spawn Settings")]
    private int Number;
    [SerializeField] private int DifficultyLevel, AIPower;
    [SerializeField] private float Interval, CooldownDuration;

    [Header("Enemy Detection and Spawn Variables")]
    private bool isSpawnOn;
    [SerializeField] private bool EnemySpawned = false;

    [Header("Delete after visuals have been implemented")]
    [SerializeField] private Renderer SpawnPointColor;
    [SerializeField] private Color SpawnedColor, OGColor;

    [Header("Interaction Variables")]
    [SerializeField] public bool isPlayerInDetectionRadius = false;
    [SerializeField] public bool isPlayerNearStopRadius = false;
    [SerializeField] private float DetectionRadius, StopRadius, SpawnCooldown;
    private bool playerWasInRadius = false;
    public Transform player; //  <-- The dumb ass Pill

    void Awake()
    {
        // SETUP RNG, Get object color to test responsiveness of code
        isSpawnOn = true;
        SpawnPointColor = GetComponent<Renderer>();
        OGColor = SpawnPointColor.material.color;
        StartRNGCycle();
    }

    // Player detection script
    void PlayerDetection()
    {
        if (player) // <-- Idiot Pill 
        {
            // Getting the distance of le Pill from door
            float PlayerDistance = Vector3.Distance(transform.position, player.position);

            // Check if le Pill is within the detection radius
            if (PlayerDistance <= DetectionRadius)
            {
                isPlayerInDetectionRadius = true;
                playerWasInRadius = true;

                // If the player is near the door and an enemy is spawned, stop the RNG cycle
                if (EnemySpawned)
                {
                    StopRNGCycle();
                    Debug.LogError("Enemy is Spawned, Player Near, Stopping AI");
                }
            }
            else
            {
                if (isPlayerInDetectionRadius && playerWasInRadius && !EnemySpawned && !isSpawnOn)
                {
                    Debug.Log("Player has left the radius after enemy despawned");
                }

                isPlayerInDetectionRadius = false;

                // If the player leaves the detection radius and no enemy is spawned, resume the RNG cycle
                if (!EnemySpawned)
                {
                    StartRNGCycle();
                }
            }

            // Check if the player is within the stop radius
            if (PlayerDistance <= StopRadius)
            {
                isPlayerNearStopRadius = true;
            }
            else
            {
                isPlayerNearStopRadius = false;
            }
        }
    }

    // Start the RNG cycle
    void StartRNGCycle()
    {
        if (isSpawnOn && !IsInvoking(nameof(RandomNumber)) && !isPlayerInDetectionRadius )
        {
            InvokeRepeating(nameof(RandomNumber), 0, Interval);
        }
    }

    // Stop the RNG cycle
    void StopRNGCycle()
    {
        if (IsInvoking(nameof(RandomNumber)))
        {
            CancelInvoke(nameof(RandomNumber));
        }
    }

    void RandomNumber()
    {
        // Check if the spawn is enabled, and if the enemy is already there, then generate random number
        if (!EnemySpawned && isSpawnOn)
        {
            Number = Random.Range(0, DifficultyLevel);
            Debug.Log("Random number is: " + Number);
            Check();
        }
    }

    void Check()
    {
        // If number is lower or equal, spawn enemy, else repeat cycle. Check if spawned beforehand. If spawned, change color
        if (Number <= AIPower && !EnemySpawned)
        {
            Debug.LogError("ENEMY HAS SPAWNED!");
            EnemySpawned = true;
            SpawnPointColor.material.color = SpawnedColor;

            // Stop the RNG cycle once an enemy spawns.

            StopRNGCycle();
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(CooldownDuration);
        SpawnPointColor.material.color = OGColor;

        Debug.LogError("Enemy despawned!");
        EnemySpawned = false;
    }

    private void OnDrawGizmos()
    {
        // Start the player detect script
        PlayerDetection();

        // Player in the general area around the door
        Color detectionColor = isPlayerInDetectionRadius ? Color.green : Color.blue;
        Gizmos.color = detectionColor;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);
        // Player next to Door
        Color stopColor = isPlayerNearStopRadius ? Color.white : Color.red;
        Gizmos.color = stopColor;
        Gizmos.DrawWireSphere(transform.position, StopRadius);
    }

    void Update()
    {
        PlayerDetection();
    }
}
