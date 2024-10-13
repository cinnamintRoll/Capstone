using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Idle, MoveToPoint, ChasePlayer, Attack }
    public enum EnemyType { Melee, Ranged, Sniper }

    public EnemyState currentState = EnemyState.Idle;
    public EnemyType enemyType = EnemyType.Melee; // Default type is melee

    public Transform player;
    public Transform pointToMove;
    public float chaseRange = 10f;
    public float attackRange = 2f; // Melee attack range
    public float rangedAttackRange = 15f; // Ranged or sniper attack range
    public float attackCooldown = 1.5f;

    private NavMeshAgent navMeshAgent;
    private float lastAttackTime = 0f;

    // Variables to control chase update intervals
    private float chaseUpdateTimer = 0f;
    public float minChaseUpdateInterval = 0.2f; // Minimum time between chase updates
    public float maxChaseUpdateInterval = 0.5f; // Maximum time between chase updates
    private float chaseUpdateInterval;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetRandomChaseUpdateInterval(); // Set a random initial update interval
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdle();
                break;
            case EnemyState.MoveToPoint:
                HandleMoveToPoint();
                break;
            case EnemyState.ChasePlayer:
                HandleChasePlayer();
                break;
            case EnemyState.Attack:
                HandleAttack();
                break;
        }
    }

    // Transition to a new state
    void TransitionToState(EnemyState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case EnemyState.Idle:
                navMeshAgent.isStopped = true;
                break;
            case EnemyState.MoveToPoint:
                navMeshAgent.isStopped = false;
                MoveToPoint();
                break;
            case EnemyState.ChasePlayer:
                navMeshAgent.isStopped = false;
                break;
            case EnemyState.Attack:
                navMeshAgent.isStopped = true;
                break;
        }
    }

    // Handle Idle State
    void HandleIdle()
    {
        // If the player comes within range, start chasing them
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            TransitionToState(EnemyState.ChasePlayer);
        }
    }

    // Handle MoveToPoint State
    void HandleMoveToPoint()
    {
        float distanceToDestination = Vector3.Distance(transform.position, pointToMove.position);

        if (distanceToDestination <= 1f)
        {
            TransitionToState(EnemyState.Attack); // Reached destination, switch to attack
        }
    }

    // Handle ChasePlayer State
    void HandleChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Only update the destination at intervals to optimize performance
        chaseUpdateTimer += Time.deltaTime;
        if (chaseUpdateTimer >= chaseUpdateInterval)
        {
            chaseUpdateTimer = 0f; // Reset the timer
            SetRandomChaseUpdateInterval(); // Randomize the next interval
            navMeshAgent.SetDestination(player.position); // Update destination
        }

        if (enemyType == EnemyType.Melee && distanceToPlayer <= attackRange)
        {
            TransitionToState(EnemyState.Attack); // Melee attack when close
        }
        else if (enemyType == EnemyType.Ranged && distanceToPlayer <= rangedAttackRange)
        {
            TransitionToState(EnemyState.Attack); // Ranged attack at distance
        }
        else if (enemyType == EnemyType.Sniper && distanceToPlayer <= rangedAttackRange)
        {
            TransitionToState(EnemyState.Attack); // Sniper attack at long range
        }
    }

    // Handle Attack State
    void HandleAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (Time.time > lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            PerformAttack();
        }

        // If player is too far for the current attack type, transition back to chase
        if (enemyType == EnemyType.Melee || enemyType == EnemyType.Sniper)
        {
            TransitionToState(EnemyState.Idle);
        }
    }

    // Enemy moves to a designated point
    void MoveToPoint()
    {
        navMeshAgent.SetDestination(pointToMove.position);
    }

    // Perform attack logic based on the enemy type
    void PerformAttack()
    {
        switch (enemyType)
        {
            case EnemyType.Melee:
                Debug.Log("Enemy performs a melee attack!");
                // Add melee attack logic (damage, animations, etc.)
                break;
            case EnemyType.Ranged:
                Debug.Log("Enemy performs a ranged attack!");
                // Add ranged attack logic (shoot projectiles, etc.)
                ShootProjectile();
                break;
            case EnemyType.Sniper:
                Debug.Log("Enemy performs a sniper attack!");
                // Add sniper attack logic (high-damage shot, longer cooldown, etc.)
                ShootSniperProjectile();
                break;
        }
    }

    // Simulate shooting a projectile (for ranged and sniper enemies)
    void ShootProjectile()
    {
        // Implement projectile instantiation here (simple ranged attack)
        Debug.Log("Enemy shoots a projectile at the player!");
    }

    // Simulate a sniper shot (higher damage or slower, more precise attack)
    void ShootSniperProjectile()
    {
        // Implement sniper shot logic here
        Debug.Log("Enemy snipes the player!");
    }

    // Public function to set the enemy's state to move to a point
    public void GoToPoint(Transform targetPoint)
    {
        pointToMove = targetPoint;
        TransitionToState(EnemyState.MoveToPoint);
    }

    // Public function to make the enemy idle
    public void Idle()
    {
        TransitionToState(EnemyState.Idle);
    }

    // Randomize the chase update interval to avoid synchronized destination updates
    void SetRandomChaseUpdateInterval()
    {
        chaseUpdateInterval = Random.Range(minChaseUpdateInterval, maxChaseUpdateInterval);
    }
}
