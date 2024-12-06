using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth;
    public float maxPostureHealth;
    public float currentHealth;
    public float currentPostureHealth;
    public float postureRecoveryRate = 5f;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentPostureHealth = 0f;
        NotifyDamageTaken(0);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        IncreasePosture(amount / 2);

        //Todo: Blood Effect

        if (currentHealth <= 0)

        { 
            Die();
        }
    }

    public void IncreasePosture(float amount)
    {
        currentPostureHealth += amount;
        currentPostureHealth = Mathf.Clamp(currentPostureHealth,0 , maxPostureHealth);
        
        if (currentPostureHealth >= maxPostureHealth)
        {
            GetComponent<PlayerCombat>().Stun(1.5f);
            currentPostureHealth = 0;
        }
    }

    private void RecoverPosture()
    {
        if (currentPostureHealth > 0f)
        {
            currentPostureHealth -= postureRecoveryRate * Time.deltaTime;
            currentPostureHealth = Mathf.Clamp(currentPostureHealth, 0, maxPostureHealth);
        }
    }

    public void NotifyDamageTaken(float amount)
    {
        GameManager.Instance.DamageProcess(this, amount);
    }

    private void Die()
    {
        //Todo: Die animation, GameOverUi, GameOverState
        GameManager.Instance.OnPlayerDeath();
    }

}
