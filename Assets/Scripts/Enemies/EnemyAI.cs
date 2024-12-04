using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    private Enemy enemy;
    private AnimationManager animationManager;
    private Animator animator;
    private EnemyCombat enemyCombat;

    [Header("Patrolling")]
    public Vector3 walkPoint;
    public float walkPointRange;
    private bool walkPointSet;

    [Header("Attacking")]
    public float timeBetweenAttacks;
    private bool alreadyAttacked;

    [Header("State")]
    public float sightRange, attackRange;
    private bool isPlayerInSightRange, isPlayerInAttackRange;

    void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponentInParent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        animationManager = new AnimationManager(animator);
        enemyCombat = GetComponentInChildren<EnemyCombat>();
        agent.stoppingDistance = attackRange - 1f;
        attackRange = enemyCombat.attackRange;
    }

    void Update()
    {
        isPlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(!isPlayerInSightRange && !isPlayerInAttackRange)
        {
            Patrolling();
        }
        else if (isPlayerInSightRange && !isPlayerInAttackRange)
        {
            Chasing();
        }
        else if (isPlayerInSightRange && isPlayerInAttackRange)
        {
            Attacking();
        }
        else
        {
            Idle();
        }
    }

    private void Patrolling()
    {
        if(!walkPointSet)
        {
            SearchWalkPoints();
            animationManager.SetBool("IsWalking", true);
            animationManager.SetBool("IsRunning", false);
            agent.speed = 1f;
        }
        if(walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoints()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast( walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void Chasing()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            agent.SetDestination(player.position);
            animationManager.SetBool("IsRunning", true);
            animationManager.SetBool("IsWalking", false);
            agent.speed = 4.5f;
        }
        else
        {
            agent.ResetPath();
            animationManager.SetBool("IsRunning", false);
        }
    }

    private void Attacking()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            agent.SetDestination(transform.position);
        }
        else
        {
            agent.ResetPath();
        }

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);

        if (!alreadyAttacked)
        {
            alreadyAttacked = true;

            enemyCombat.PerformRandomCombo();

            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void Idle()
    {
        agent.ResetPath();
        animationManager.SetBool("IsWalking", false);
        animationManager.SetBool("IsRunning", false);
        agent.speed = 0f;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);  
    }
}
