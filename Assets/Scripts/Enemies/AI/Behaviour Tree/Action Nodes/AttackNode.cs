using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackNode : Node
{
    private EnemyAI_BT enemyAI_;
    private bool isAttacking;

    public AttackNode(EnemyAI_BT enemyAI_)
    {
        this.enemyAI_ = enemyAI_;
    }

    public override NodeState Evaluate()
    {
        float distanceToPlayer = Vector3.Distance(enemyAI_.transform.position, enemyAI_.player.position);

        if (distanceToPlayer <= enemyAI_.attackRange)
        {
            enemyAI_.agent.SetDestination(enemyAI_.transform.position);
        }
        else
        {
            enemyAI_.agent.ResetPath();
        }

        if (!isAttacking)
        {
            isAttacking = true;
            enemyAI_.enemyCombat.PerformRandomCombo();
            //enemyAI_.animator.SetTrigger("Attack");
            enemyAI_.Invoke(nameof(ResetAttack), 1.5f);
        }

        state = NodeState.Running;
        return state;
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }
}
