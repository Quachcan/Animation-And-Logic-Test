using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StrafeNode : Node
{
    private EnemyAI_BT enemyAI;
    private float strafeDuration = 2f;
    private float strafeTimer;
    private float strafeDistance;
    private int strafeDirection; // -1 cho trái, 0 cho lùi, 1 cho phải

    public StrafeNode(EnemyAI_BT enemyAI)
    {
        this.enemyAI = enemyAI;
        strafeDistance = enemyAI.strafeRange - 0.5f;
    }

    public override NodeState Evaluate()
    {
        if (enemyAI.isActionLocked || enemyAI.enemyHealth.isStunned)
        {
            state = NodeState.Failure;
            return state;
        }

        if (strafeTimer <= 0)
        {
            float randomValue = Random.value;
            if (randomValue < 0.33f)
                strafeDirection = -1; // Trái
            else if (randomValue < 0.66f)
                strafeDirection = 1; // Phải
            else
                strafeDirection = 0; // Lùi lại

            strafeTimer = strafeDuration;
        }

        Vector3 directionToPlayer = (enemyAI.transform.position - enemyAI.player.position).normalized;
        Vector3 strafeDirectionVector = Vector3.zero;

        // if (strafeDirection == -1)
        // {
        //     // Left
        //     strafeDirectionVector = Vector3.Cross(directionToPlayer, Vector3.up).normalized;
        // }
        // else if (strafeDirection == 1)
        // {
        //     // Right
        //     strafeDirectionVector = -Vector3.Cross(directionToPlayer, Vector3.up).normalized;
        // }
        // else if (strafeDirection == 0)
        // {
        //     // Backward
        //     strafeDirectionVector = directionToPlayer;
        // }

        // enemyAI.agent.Move(strafeDirectionVector * enemyAI.agent.speed * Time.deltaTime);

        Vector3 targetPosition = enemyAI.player.position + directionToPlayer * strafeDistance + strafeDirectionVector * strafeDistance;


        enemyAI.agent.isStopped = false;
        enemyAI.agent.SetDestination(targetPosition);
        enemyAI.agent.speed = 1.5f;
        
        enemyAI.transform.LookAt(new Vector3(enemyAI.player.position.x, enemyAI.transform.position.y, enemyAI.player.position.z));
        
        strafeTimer -= Time.deltaTime;

        enemyAI.animator.SetFloat("StrafeDirection", strafeDirection);
        enemyAI.animator.SetBool("IsStrafing", true);

        state = NodeState.Running;
        return state;
    }
}