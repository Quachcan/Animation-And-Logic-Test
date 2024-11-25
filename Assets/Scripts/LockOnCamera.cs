using UnityEngine;
using Cinemachine;

public class LockOnCamera : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineFreeLook freeLookCamera; // Default FreeLook camera
    public CinemachineVirtualCamera lockOnCamera; // Dedicated lock-on camera

    [Header("Lock-On Settings")]
    public Transform player; // Player transform
    public Transform target; // Current lock-on target
    public float lockOnDistance = 15f; // Maximum distance to lock on
    public KeyCode lockOnKey = KeyCode.Q; // Key to toggle lock-on mode

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
                // Turn off lock-on mode
                DisableLockOn();
            }
            else
            {
                // Find and lock onto the nearest target
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
            float distance = Vector3.Distance(player.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestTarget = collider.transform;
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

            target = null; // Clear the lock-on target
        }
    }

    private void RotatePlayerTowardTarget()
{
    // Calculate direction to the target
    Vector3 direction = (target.position - player.position).normalized;

    // Ignore Y-axis for a flat rotation
    direction.y = 0;

    // Calculate the target rotation
    Quaternion targetRotation = Quaternion.LookRotation(direction);

    // Smoothly rotate the player towards the target
    player.rotation = Quaternion.Slerp(player.rotation, targetRotation, Time.deltaTime * 10f);
}
}
