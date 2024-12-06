using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    private List<Node> nodes;

    public Sequence(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Evaluate()
    {
        bool anyNodeRunning = false;

        foreach (Node node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.Running:
                    anyNodeRunning = true;
                    continue;
                case NodeState.Success:
                    continue;
                case NodeState.Failure:
                    state = NodeState.Failure;
                    return state;
            }
        }

        state = anyNodeRunning ? NodeState.Running : NodeState.Success;
        return state;
    }
}
