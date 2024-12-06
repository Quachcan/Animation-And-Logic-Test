using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Combo Attacks")]
    public List<Combo> combos; 
    private float lastComboTime;

    [Header("Combat Setting")]
    public float attackDamage;
    public float attackRange;
    public LayerMask whatIsPlayer;
    public bool isStunned = false;
    private float stunTimer;

    public GameObject sword;
    private EnemyAI_BT enemyAI;

    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        enemyAI = GetComponent<EnemyAI_BT>();
        lastComboTime = -float.MaxValue;
    }

    private void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            enemyAI.isActionLocked = false;
            Debug.Log("Enemy recover from stun");
        }
    }

    public void DealDamage()
    {
        Collider[] hitPlayer = Physics.OverlapSphere(sword.transform.position, attackRange, whatIsPlayer);
        Debug.Log($"Player in range: {hitPlayer.Length}");

        HashSet<IDamageable> damagedTarget = new HashSet<IDamageable>();

        foreach (Collider target in hitPlayer)
        {
            IDamageable damageable = target.GetComponentInParent<IDamageable>();
            if (damageable != null && !damagedTarget.Contains(damageable))
            {
                damagedTarget.Add(damageable);

                damageable.NotifyDamageTaken(attackRange);
                Debug.Log($"Dealt {attackDamage} damage to {target.name}");
            }
            else 
            {
                Debug.Log($"No PlayerHealth script found on {target.name}");
            }
        }
    }

    public void PerformRandomCombo()
    {
        if (Time.time - lastComboTime >= combos[0].comboCoolDownTime)
        {
            if (combos.Count > 0)
            {
                Combo randomCombo = combos[Random.Range(0, combos.Count)];
                StartCoroutine(ExecuteCombo(randomCombo));

                lastComboTime = Time.time;
            }
        }
        
    }

    private IEnumerator ExecuteCombo(Combo combo)
    {

        foreach (string attackTrigger in combo.attackTriggers)
        {
            animator.SetTrigger(attackTrigger);  

            yield return new WaitForSeconds(combo.comboTime);
        }

        yield return new WaitForSeconds(combo.comboCoolDownTime);
    }

    private void Parry()
    {
        animator.SetTrigger("Parry");

    }
    public void Stun(float duration)
{
    if (isStunned) return;

    isStunned = true;
    stunTimer = duration;
    animator.SetTrigger("Stunned");
    enemyAI.isActionLocked = true;
    Debug.Log("Enemy is stunned!");
}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(sword.transform.position, attackRange);
        Gizmos.color = Color.green;
    }
}
