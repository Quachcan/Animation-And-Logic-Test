using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Combo Attacks")]
    public List<Combo> combos; 

    private Animator animator;
    private float lastComboTime;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        lastComboTime = -float.MaxValue;
        PerformRandomCombo();
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
}
