using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayerInAttackRangeNode : Node
{
    private EnemyAI_BT enemyAI_;

    public IsPlayerInAttackRangeNode(EnemyAI_BT enemyAI_)
    {
        this.enemyAI_ = enemyAI_;
    }

    public override NodeState Evaluate()
    {
        return enemyAI_.IsPlayerInAttackRange() ? NodeState.Success : NodeState.Failure;
    }
}
