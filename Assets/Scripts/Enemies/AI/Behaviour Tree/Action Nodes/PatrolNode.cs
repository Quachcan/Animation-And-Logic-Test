using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolNode : Node
{
    private EnemyAI_BT enemyAI_;
    private Vector3 patrolPoint;
    private bool patrolPointSet;
    private float walkPointRange = 10f;

    public PatrolNode(EnemyAI_BT enemyAI_)
    {
        this.enemyAI_ = enemyAI_;
    }

    public override NodeState Evaluate()
    {
        if (!patrolPointSet) 
        {
            SearchWalkPoint();
        }

        if (patrolPointSet)
        {
            enemyAI_.agent.SetDestination(patrolPoint);
            enemyAI_.animator.SetBool("IsWalking", true);
            enemyAI_.agent.speed = 1f;

            Vector3 distanceToWalkPoint = enemyAI_.transform.position - patrolPoint;
            if  (distanceToWalkPoint.magnitude < 1f)
            {
                patrolPointSet = false;
            }

            state = NodeState.Running;
            return state;
        }

        state = NodeState.Failure;
        return state;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        patrolPoint = new Vector3(enemyAI_.transform.position.x + randomX, enemyAI_.transform.position.y, enemyAI_.transform.position.z + randomZ);

        patrolPointSet = true;
    }
}
