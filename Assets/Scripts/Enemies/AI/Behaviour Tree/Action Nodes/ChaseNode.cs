using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseNode : Node
{
    private EnemyAI_BT enemyAI_;

    public ChaseNode(EnemyAI_BT enemyAI_)
    {
        this.enemyAI_ = enemyAI_;
    }

    public override NodeState Evaluate()
    {
        enemyAI_.agent.SetDestination(enemyAI_.player.position);
        enemyAI_.animator.SetBool("IsWalking", false);
        enemyAI_.animator.SetBool("IsStrafing", false);
        enemyAI_.animator.SetBool("IsRunning", true);
        enemyAI_.agent.speed = 5f;

        state = NodeState.Running;
        return state;
    }
}
