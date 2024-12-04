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

    public GameObject sword;

    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        lastComboTime = -float.MaxValue;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(sword.transform.position, attackRange);
        Gizmos.color = Color.green;
    }
}
