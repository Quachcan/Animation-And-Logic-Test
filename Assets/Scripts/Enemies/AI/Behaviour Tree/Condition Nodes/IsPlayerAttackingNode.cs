using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayerAttackingNode : Node
{
    private EnemyAI_BT enemyAI_;
    private PlayerCombat playerCombat;

    public IsPlayerAttackingNode(EnemyAI_BT enemyAI_)
    {
        this.enemyAI_ = enemyAI_;
        playerCombat = enemyAI_.player.GetComponent<PlayerCombat>();
    }

    public override NodeState Evaluate()
    {
        if (playerCombat != null && playerCombat.isAttacking)
        {
            return NodeState.Success;
        }
        return NodeState.Failure;
    }
}
