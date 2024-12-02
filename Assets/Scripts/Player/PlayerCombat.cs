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
        HandleDeflect();

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

    private void DealDamage()
    {

        Collider[] hitEnemies = Physics.OverlapSphere(sword.transform.position, attackRange, whatIsEnemy);

        //Debug.Log($"Enemy in range: {hitEnemies.Length}");
        HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();

        foreach (Collider target in hitEnemies)
        {
            IDamageable damageable = target.GetComponentInParent<IDamageable>();
            if (damageable != null && !damagedTargets.Contains(damageable))
            {
                damagedTargets.Add(damageable);

                damageable.NotifyDamageTaken(attackDamage);
                //Debug.Log($"Dealt {attackDamage} damge to {target.name}");
            }
            else
            {
                //Debug.Log($"No EnemyHealth script found on {target.name}");
            }
        }
    }

    private void HandleDeflect()
    {
        if(Input.GetKeyDown(deflectKey) && canDeflect)
        {
            StartCoroutine(PerformDeflect());
        }
    }

    private IEnumerator PerformDeflect()
    {
        canDeflect = false;

        animator.SetBool("Deflect", true);

        float elpapsedTime = 0f;
        bool deflectSuccess = false;

        while (elpapsedTime < deflectWindown)
        {
            if (CheckForEnemyAttack())
            {
                deflectSuccess = true;
                break;
            }
            elpapsedTime += Time.deltaTime;
            yield return null;
        }

        if (deflectSuccess)
        {
            SuccessDeflect();
        }
        else
        {
            FailedDeflect();
        }

        yield return new WaitForSeconds(deflectCoolDown);
        canDeflect = true;
    }

    private bool CheckForEnemyAttack()
    {
        //Check for enemy atack states.    
        return Random.Range(0, 100) < 10;
    }

    private void SuccessDeflect()
    {
        //Todo: Deal posture damage for enemy
        int parryIndex = Random.Range(0, 3);
        animationManager.SetParryIndex(parryIndex);  

        if (deflectEffect != null)
        {
            //Effect for deflect
        }
        // audio for deflect 
    }

    private void FailedDeflect()
    {
        Debug.Log("Parry Failed");
        animationManager.PlayDefenseHit();
        // TODO:  Player take damage
    }

    private void HandleDefense()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            StartDefense();
        }

        if (Input.GetMouseButton(1)) 
        {
            if (isInParryWindow)
            {
                animationManager.SetActionState(1); 
            }
        }

        if (Input.GetMouseButtonUp(1)) 
        {
            EndDefense();
        }

        if (Time.time - defenseStartTime < parryWindowDuration && !isInParryWindow)
        {
            isInParryWindow = true;
            int parryIndex = Random.Range(0, 3); // Chọn ngẫu nhiên phản ứng parry
            animationManager.SetParryIndex(parryIndex);  // Cập nhật ParryIndex
            animationManager.TriggerParryReaction();
        }
    }

    private void StartDefense()
    {
        defenseStartTime = Time.time;
        isInParryWindow = false;
        animationManager.SetActionState(0);  // DefenseStart
        animationManager.TriggerDefenseStart();
    }

    private void EndDefense()
    {
        animationManager.SetActionState(2);  // Kết thúc DefenseLoop
        animationManager.TriggerDefenseLoop();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(sword.transform.position, attackRange);
    }
}
