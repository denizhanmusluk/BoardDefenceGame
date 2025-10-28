using System;
using UnityEngine;

public class AIMoving : MonoBehaviour
{
    public event Action<Vector3> followingPos;
    public event Action<Transform> followingTr;
    public event Action targetArrivedBehaviour;

    [Header("Movement Settings")]
    public Animator animator;
    public Enemy enemy;
    public float walkSpeed = 1f;
    public float runSpeed = 2f;
    public float arrivedDistance_Offset = 1f;

    private void OnEnable() => SetWalkState();

    public void SetWalkState()
    {
        if (animator != null)
            animator.SetFloat("walkspeed", walkSpeed * 1.5f);
    }

    public void BehaviourInit(Action fnct)
    {
        targetArrivedBehaviour = null;
        targetArrivedBehaviour += fnct;
    }

    public void GoTargetPos(Vector3 targetPos)
    {
        followingPos = (pos) => MoveTowards(targetPos);
    }

    private void MoveTowards(Vector3 targetPos)
    {
        if (Vector3.Distance(transform.position, targetPos) > arrivedDistance_Offset)
        {
            animator?.SetBool("walk", true);
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                Time.deltaTime * walkSpeed * 0.1f
            );
        }
        else
        {
            animator?.SetBool("walk", false);
            followingPos = null;
            targetArrivedBehaviour?.Invoke();
        }
    }

    public void GoTargetTransform(Transform targetPosTR)
    {
        followingTr = (posTR) => MoveTowardsTransform(targetPosTR);
    }

    private void MoveTowardsTransform(Transform targetPosTR)
    {
        if (targetPosTR == null) return;

        if (Vector3.Distance(transform.position, targetPosTR.position) > arrivedDistance_Offset)
        {
            animator?.SetBool("walk", true);
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosTR.position,
                Time.deltaTime * walkSpeed * 0.1f
            );
        }
        else
        {
            animator?.SetBool("walk", false);
            followingTr = null;
            targetArrivedBehaviour?.Invoke();
        }
    }

    private void Update()
    {
        followingTr?.Invoke(transform);
        followingPos?.Invoke(Vector3.one);
    }

    public void StopAllActions()
    {
        followingTr = null;
        followingPos = null;
        targetArrivedBehaviour = null;
        animator?.SetBool("walk", false);
    }
}
