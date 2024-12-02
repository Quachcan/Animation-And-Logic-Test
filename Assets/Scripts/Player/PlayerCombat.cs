using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackCooldown = 1f; 
    public float attackRange;
    public float attackDamage;
    private bool canAttack = true;
    public LayerMask whatIsEnemy;

    [Header("Combo System")]
    public int maxCombo = 3;
    private int currentCombo = 0;
    private float comboResetTime = 2f;
    private float lastAttackTime;
    private List<string> comboTriggers = new List<string> { "Attack_1", "Attack_2", "Attack_3" };

    [Header("Defend System")]
    private bool isInParryWindow;
    private float defenseStartTime;
    private float parryWindowDuration = 0.5f;
    private bool isDefending = false;

    [Header("Parry System")]
    private bool canDeflect = true;
    public float deflectCoolDown = 0.1f;
    public float deflectWindown = 0.3f;
    public GameObject deflectEffect;

    [Header("Animator")]
    private Animator animator;

    [Header("Combat Keybinds")]
    public KeyCode toggleCombatKey = KeyCode.C; 
    public KeyCode attackKey = KeyCode.Mouse0; 
    public KeyCode deflectKey = KeyCode.Mouse1;

    [Header("Referrences")]
    public GameObject sword;

    [Header("Components")]
    private AnimationManager animationManager;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        animationManager = new AnimationManager(animator);
    }

    private void Update()
    {
        HandleAttack();
        HandleDefense();

        if (Time.time - lastAttackTime > comboResetTime && currentCombo > 0)
        {
            ResetCombo();
        }
    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(attackKey) && canAttack)
        {
            PerformComboAttack();
        }
    }

    private void PerformComboAttack()
    {
        Debug.Log($"Current Combo: {currentCombo}");
        foreach (string trigger in comboTriggers)
        {
            animator.ResetTrigger(trigger);
        }

        currentCombo++;

        if (currentCombo > comboTriggers.Count)
        {
            currentCombo = 1;
        }

        DealDamage();

        string triggerToActivate = comboTriggers[currentCombo - 1];
        animator.SetTrigger(triggerToActivate);

        lastAttackTime = Time.time;

        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    private void ResetCombo()
    {
        currentCombo = 0;
    }

    // private void DealDamage()
    // {

    //     Collider[] hitEnemies = Physics.OverlapSphere(sword.transform.position, attackRange, whatIsEnemy);

    //     //Debug.Log($"Enemy in range: {hitEnemies.Length}");
    //     HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();

    //     foreach (Collider target in hitEnemies)
    //     {
    //         IDamageable damageable = target.GetComponentInParent<IDamageable>();
    //         if (damageable != null && !damagedTargets.Contains(damageable))
    //         {
    //             damagedTargets.Add(damageable);

    //             damageable.NotifyDamageTaken(attackDamage);
    //             //Debug.Log($"Dealt {attackDamage} damge to {target.name}");
    //         }
    //         else
    //         {
    //             //Debug.Log($"No EnemyHealth script found on {target.name}");
    //         }
    //     }
    // }

    private void DealDamage()
{
    Ray ray = new Ray(sword.transform.position, transform.forward);
    RaycastHit[] hits = Physics.RaycastAll(ray, attackRange, whatIsEnemy);

    foreach (RaycastHit hit in hits)
    {
        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.NotifyDamageTaken(attackDamage);
            Debug.Log($"Dealt {attackDamage} damage to {hit.collider.name}");
        }
    }

    if (hits.Length == 0)
    {
        Debug.Log("No enemies hit.");
    }
}

    private void HandleDefense()
    {
        if (Input.GetMouseButtonDown(1)) // Right Mouse Button
        {
            StartDefense();
        }

        if (Input.GetMouseButton(1)) // Hold defense
        {
            if (isDefending && !isInParryWindow)
            {
                PlayDefenseLoop();
            }
        }

        if (Input.GetMouseButtonUp(1)) // Release defense
        {
            EndDefense();
        }
    }

    private void StartDefense()
    {
        if (isDefending) return;

        isDefending = true;
        isInParryWindow = true; // Enable parry window
        defenseStartTime = Time.time;

        animationManager.SetBool("IsDefending", true);
        animationManager.PlayTrigger("DefenseStart");

        // Start parry window timer
        StartCoroutine(HandleParryWindow());
    }

    private IEnumerator HandleParryWindow()
    {
        yield return new WaitForSeconds(parryWindowDuration);
        isInParryWindow = false;
    }

    private void PlayDefenseLoop()
    {
        animationManager.PlayTrigger("DefenseLoop");
    }

    private void EndDefense()
    {
        isDefending = false;
        animationManager.StopDefense(); // Stop all defense animations
    }

    public void TryParry(bool enemyAttackDetected)
    {
        if (!isDefending) return;

        if (isInParryWindow && enemyAttackDetected)
        {
            SuccessParry();
        }
        else if (enemyAttackDetected)
        {
            FailParry();
        }
    }



    private void SuccessParry()
    {
        int parryIndex = Random.Range(0, 3); // Choose one of three parry animations
        animationManager.SetParryIndex(parryIndex);
        animationManager.TriggerParryReaction();

        if (deflectEffect != null)
        {
            Instantiate(deflectEffect, transform.position, Quaternion.identity);
        }

        Debug.Log("Parry Successful!");
    }

    private void FailParry()
    {
        animationManager.PlayTrigger("DefenseHit");
        Debug.Log("Parry Failed! Playing Defend Hit Animation.");
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(sword.transform.position, attackRange);
        // Gizmos.color = Color.red;

    Gizmos.color = Color.red;

    // Gốc của Ray
    Vector3 rayOrigin = sword.transform.position;

    // Hướng chính diện
    Vector3 centralDirection = transform.forward;

    // Góc lệch (để tạo ray trái và phải)
    float sideAngle = 30f; // Góc lệch 30 độ

    // Tính toán hướng ray lệch
    Vector3 leftDirection = Quaternion.Euler(0, -sideAngle, 0) * transform.forward;
    Vector3 rightDirection = Quaternion.Euler(0, sideAngle, 0) * transform.forward;

    // Vẽ Ray chính diện
    Gizmos.DrawRay(rayOrigin, centralDirection * attackRange);

    // Vẽ Ray lệch trái
    Gizmos.DrawRay(rayOrigin, leftDirection * attackRange);

    // Vẽ Ray lệch phải
    Gizmos.DrawRay(rayOrigin, rightDirection * attackRange);

    // Debug: kiểm tra va chạm với kẻ địch trên từng ray
    RaycastHit hit;

    // Kiểm tra Ray chính diện
    if (Physics.Raycast(rayOrigin, centralDirection, out hit, attackRange, whatIsEnemy))
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(hit.point, 0.2f); // Đánh dấu vị trí va chạm
    }

    // Kiểm tra Ray lệch trái
    if (Physics.Raycast(rayOrigin, leftDirection, out hit, attackRange, whatIsEnemy))
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(hit.point, 0.2f); // Đánh dấu vị trí va chạm
    }

    // Kiểm tra Ray lệch phải
    if (Physics.Raycast(rayOrigin, rightDirection, out hit, attackRange, whatIsEnemy))
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(hit.point, 0.2f); // Đánh dấu vị trí va chạm
    }
    }
}
