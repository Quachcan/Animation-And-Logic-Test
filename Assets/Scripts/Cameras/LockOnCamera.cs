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

    private void Update()
    {
        HandleLockOnInput();

        if (isLockedOn && target != null)
        {
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
    Vector3 direction = (target.position - player.position).normalized;

    direction.y = 0;

    float angle = Vector3.Angle(player.forward, direction);
    if (angle < 1f) return;

    Quaternion targetRotation = Quaternion.LookRotation(direction);

    player.rotation = Quaternion.Slerp(player.rotation, targetRotation, Time.deltaTime * 10f);
}
}
