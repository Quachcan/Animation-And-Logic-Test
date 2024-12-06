using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayerInStrafeRangeNode : Node
{
    private EnemyAI_BT enemyAI;
    private float strafeRange;

    public IsPlayerInStrafeRangeNode(EnemyAI_BT enemyAI, float strafeRange)
    {
        this.enemyAI = enemyAI;
        this.strafeRange = strafeRange;
    }

    public override NodeState Evaluate()
    {
        float distance = Vector3.Distance(enemyAI.transform.position, enemyAI.player.position);
        if ( distance <= strafeRange && distance > enemyAI.attackRange)
        {
            return NodeState.Success;
        }
        return NodeState.Failure;
    }
}
