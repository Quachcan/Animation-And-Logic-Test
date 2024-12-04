using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private EnemyCombat enemyCombat;
    // Start is called before the first frame update
    void Awake()
    {
        enemyCombat = GetComponent<EnemyCombat>();
    }

    // Update is called once per frame
    public void TriggerDealDamage()
    {
        // Gọi phương thức DealDamage từ script EnemyCombat
        if (enemyCombat != null)
        {
            enemyCombat.DealDamage();
        }
    }
}
