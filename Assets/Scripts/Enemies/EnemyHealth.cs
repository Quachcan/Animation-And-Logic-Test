using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{

    public float maxHealth;
    public float maxPostureHealth;
    public float currentHealth;
    private float currentPostureHealth;


    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
        currentPostureHealth = maxPostureHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount/1.5f;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        currentPostureHealth -= amount;

        //Todo: Blood effect

        if (currentHealth <= 0)
        {
            Die();
        }
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
