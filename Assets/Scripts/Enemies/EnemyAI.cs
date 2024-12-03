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
    }

    void Update()
    {
        isPlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(!isPlayerInSightRange && !isPlayerInAttackRange)
        {
            Patrolling();
        }
        if (isPlayerInSightRange && !isPlayerInAttackRange)
        {
            Chasing();
        }
        if (isPlayerInSightRange && isPlayerInAttackRange)
        {
            Attacking();
        }
    }

    private void Patrolling()
    {
        if(!walkPointSet)
        {
            SearchWalkPoints();
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
        agent.SetDestination(player.position);
    }

    private void Attacking()
    {

        agent.SetDestination(player.position);

        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void DealDamage()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);  
    }
}
