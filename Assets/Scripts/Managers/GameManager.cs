using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, Paused, GameOver}
    public GameState CurrentState { get; private set; }
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


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void ChangeState (GameState newState)
    {
        CurrentState = newState;

        switch (CurrentState)
        { 
            case GameState.MainMenu:
                Debug.Log("Game State: Main Menu");
                break;
            case GameState.Playing:
                Debug.Log("Game State: Playing");
                break;
            case GameState.Paused:
                Debug.Log("Game State: Paused");
                break;
            case GameState.GameOver:
                Debug.Log("Game State: Game Over");
                break;
        }
    }

    public void PauseGame()
    {
        ChangeState(GameState.Paused);
        Time.timeScale = 0;
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
        Time.timeScale = 1f;
    }

    public void OnPlayerHealthChanged(float currentHealth, float maxHealth)
    {
       // UIManager.Instance.UpdatePlayerHealthUI(currentHealth, maxHealth);
    }

    public void OnPlayerDeath()
    {
        Debug.Log("GameOver!");
       // UIManager.Instance.ShowGameOverScreen();
    }

    public void OnEnemyHealthChanged(float currentHealth, float maxHealth)
    {
        
    }

    public void OnEnemyDeath(EnemyHealth enemy)
    {

    }

    public void DamageProcess(IDamageable target, float amount)
    {
        if (target != null)
        {
            target.TakeDamage(amount);

        }
        else
        {
            Debug.LogError("ProcessingDamage : Target is null");
        }


        if (target is PlayerHealth player)
        {
           UIManager.Instance.UpdatePlayerHealthUI(player.currentHealth, player.maxHealth);
        }
        else if (target is EnemyHealth enemy) 
        {
           UIManager.Instance.UpdateEnemyHealthUI(enemy.currentHealth,  enemy.maxHealth);

        }
    }
}
