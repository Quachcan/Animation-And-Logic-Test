using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{

    public float maxHealth;
    public float maxPostureHealth;
    public float currentHealth;
    public float currentPostureHealth;
    public float postureRecoveryRate =5f;
    public bool isStunned = false;


    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
        currentPostureHealth = 0f;
    }

    void Update()
    {
        RecoverFromStun();
        RecoverPosture();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        IncreasePosture(amount / 2);

        //Todo: Blood effect

        if (currentHealth <= 0)
        {
            Die();
        }
    }

        public void IncreasePosture(float amount)
    {
        currentPostureHealth += amount;
        currentPostureHealth = Mathf.Clamp(currentPostureHealth, 0, maxPostureHealth);

        if (currentPostureHealth >= maxPostureHealth)
        {
            GetComponent<EnemyCombat>().Stun(1.5f);
            isStunned = true;
            currentPostureHealth = 0; 
        }
    }

        private void RecoverPosture()
    {
        if (currentPostureHealth > 0f)
        {
            currentPostureHealth -= postureRecoveryRate * Time.deltaTime;
            currentPostureHealth = Mathf.Clamp(currentPostureHealth, 0f, maxPostureHealth);
        }
    }

    public void RecoverFromStun()
    {
        isStunned = false;
    }
    public void NotifyDamageTaken(float amount)
    {
        Debug.Log($"NotifyDamageTaken called");
        GameManager.Instance.DamageProcess(this, amount);
    }

    private void Die()
    {
        //Todo: Die effect
        animator.SetTrigger("Dead");
        Invoke(nameof(DestroyGameObject), 2f);
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
