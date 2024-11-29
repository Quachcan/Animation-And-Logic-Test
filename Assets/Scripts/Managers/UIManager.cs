using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void UpdatePlayerHealthUI(float currentHealth, float maxHealth)
    {

    }

    public void UpdateEnemyHealthUI(float currentHealth, float maxHealth)
    {

    }
}
