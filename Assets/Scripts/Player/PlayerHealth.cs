using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth;
    public float maxPostureHealth;
    public float currentHealth;
    private float currentPostureHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentPostureHealth = maxPostureHealth;
        NotifyDamageTaken(0);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        //Todo: Blood Effect

        if (currentHealth <= 0)

        { 
            Die();
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
