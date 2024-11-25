using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public bool isInCombat = false; // Tracks if the player is in combat mode
    public float attackCooldown = 1f; // Time between attacks
    private bool canAttack = true; // Tracks if the player can attack

    [Header("Animator")]
    private Animator animator;

    [Header("Combat Keybinds")]
    public KeyCode toggleCombatKey = KeyCode.C; // Key to toggle combat mode
    public KeyCode attackKey = KeyCode.Mouse0; // Key for attacking (e.g., left mouse button)

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        HandleCombatToggle();
        HandleAttack();
    }

    private void HandleCombatToggle()
    {
        // Toggle combat mode
        if (Input.GetKeyDown(toggleCombatKey))
        {
            isInCombat = !isInCombat;
            animator.SetBool("IsInCombat", isInCombat); // Update animator parameter
        }
    }

    private void HandleAttack()
    {
        if (isInCombat && Input.GetKeyDown(attackKey) && canAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        // Trigger attack animation
        animator.SetTrigger("Attack_01");

        // Prevent spamming attacks by enforcing a cooldown
        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    // Optional: Call these methods from external scripts or events to enter/exit combat
    public void EnterCombat()
    {
        isInCombat = true;
        animator.SetBool("IsInCombat", true);
    }

    public void ExitCombat()
    {
        isInCombat = false;
        animator.SetBool("IsInCombat", false);
    }
}
