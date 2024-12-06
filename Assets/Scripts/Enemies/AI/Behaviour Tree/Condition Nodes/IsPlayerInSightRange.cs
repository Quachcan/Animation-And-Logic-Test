using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayerInSightRange : Node
{
    private EnemyAI_BT enemyAI_;

    public IsPlayerInSightRange(EnemyAI_BT enemyAI_)
    {
        this.enemyAI_ = enemyAI_;
    }
    public override NodeState Evaluate()
    {
        return enemyAI_.IsPlayerInSightRange() ? NodeState.Success : NodeState.Failure;
    }
}
