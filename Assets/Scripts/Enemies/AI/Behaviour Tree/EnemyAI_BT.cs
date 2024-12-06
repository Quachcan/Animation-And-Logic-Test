using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI_BT : MonoBehaviour
{
    private Node topNode;
    public NavMeshAgent agent;
    public Transform player;
    public Animator animator;
    public EnemyCombat enemyCombat;
    public EnemyHealth enemyHealth;

    [Header("Settings")]
    public LayerMask whatIsPlayer;
    public float sightRange;
    public float attackRange;
    public float strafeRange;
    public bool isActionLocked = false;



    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        animator = GetComponentInChildren<Animator>();
        enemyCombat = GetComponent<EnemyCombat>();
        enemyHealth = GetComponent<EnemyHealth>();

        ConstructBehaviourTree();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActionLocked || enemyHealth.isStunned)
        {
            agent.isStopped = true;
            return;
        }
        else
        {
            agent.isStopped = false;
            topNode.Evaluate();
        }
    }

    private void ConstructBehaviourTree()
    {
        //Tạo các node điều kiện và hành động
        IsPlayerInAttackRangeNode isPlayerInAttackRangeNode = new IsPlayerInAttackRangeNode(this);
        IsPlayerInSightRange isPlayerInSightRange = new IsPlayerInSightRange(this);
        IsPlayerAttackingNode isPlayerAttackingNode = new IsPlayerAttackingNode(this);
        IsPlayerInStrafeRangeNode isPlayerInStrafeRangeNode = new IsPlayerInStrafeRangeNode(this, strafeRange);

        AttackNode attackNode= new AttackNode(this);
        PatrolNode patrolNode= new PatrolNode(this);
        ChaseNode chaseNode= new ChaseNode(this);
        StrafeNode strafeNode= new StrafeNode(this);
        //ParryNode parryNode= new ParryNode(this);

        //Tạo các Sequence 
        Sequence attackSequence = new Sequence( new List<Node> { isPlayerInAttackRangeNode, strafeNode, attackNode});
        Sequence strafeSequence = new Sequence( new List<Node> { isPlayerInStrafeRangeNode, strafeNode, attackNode});
        Sequence chaseSequence = new Sequence( new List<Node> { isPlayerInSightRange, chaseNode});
        //Sequence parrySequence = new Sequence( new List<Node> {isPlayerAttackingNode, ParryNode});

        //Tạo Selector gốc
        topNode = new Selector( new List<Node> { attackSequence, chaseSequence, patrolNode, strafeSequence});
    }

    public bool IsPlayerInSightRange()
    {
        return Vector3.Distance(transform.position, player.position) <= sightRange;
    }

    public bool IsPlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) <= attackRange;
    }
}
