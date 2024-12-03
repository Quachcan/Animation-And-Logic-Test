using UnityEngine;
using Cinemachine;

public class LockOnCamera : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineFreeLook freeLookCamera; 
    public CinemachineVirtualCamera lockOnCamera; 

    [Header("Lock-On Settings")]
    public Transform player; 
    public Transform target; 
    public float lockOnDistance = 15f; 
    public KeyCode lockOnKey = KeyCode.Q; 

    private bool isLockedOn = false;

    private EnemyHealth enemy;

    private void Update()
    {
        HandleLockOnInput();

        if (isLockedOn)
        {
            if (target == null)
            {
                DisableLockOn();
                return;
            }
            if (Vector3.Distance(player.position, target.position) > lockOnDistance)
            {
                DisableLockOn();
                return;
            }

            RotatePlayerTowardTarget();
        }

    }

    private void HandleLockOnInput()
    {
        if (Input.GetKeyDown(lockOnKey))
        {
            if (isLockedOn)
            {              
                DisableLockOn();
            }
            else
            {
                FindTarget();
            }
        }
    }

    private void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(player.position, lockOnDistance, LayerMask.GetMask("Enemy"));
        Transform nearestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            Enemy enemy = collider.gameObject.GetComponentInParent<Enemy>();
            if (enemy != null && enemy.lockOnTarget != null)
            {
                float distance = Vector3.Distance(player.position, enemy.lockOnTarget.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestTarget = enemy.lockOnTarget;
                }
            }
        }

        if (nearestTarget != null)
        {
            target = nearestTarget;
            EnableLockOn();
        }
    }

    private void EnableLockOn()
    {
        if (lockOnCamera != null && target != null)
        {
            isLockedOn = true;

            // Switch to the lock-on camera
            lockOnCamera.gameObject.SetActive(true);
            freeLookCamera.gameObject.SetActive(false);

            // Set the lock-on target for the lock-on camera
            lockOnCamera.LookAt = target;

        }
    }

        private void HandleEnemyDeath()
    {
        DisableLockOn();
    }

    private void DisableLockOn()
    {
        if (lockOnCamera != null)
        {
            isLockedOn = false;

            // Switch back to the free camera
            lockOnCamera.gameObject.SetActive(false);
            freeLookCamera.gameObject.SetActive(true);

            target = null; 

        }
    }

        private void RotatePlayerTowardTarget()
    {
        if (target == null) return;

        Vector3 direction = (target.position - player.position).normalized;
        direction.y = 0; // Keep the rotation on the horizontal plane

        if (direction.magnitude > 0.01f) // Check if there's significant direction to rotate towards
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            player.rotation = Quaternion.Lerp(player.rotation, targetRotation, Time.deltaTime * 5f); // Reduce the rotation speed for smoother transitions
        }
    }

}
