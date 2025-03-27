using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private NavMeshAgent agent;
    private float lastFindTime;
    private float findDelay = 0.1f;

    private float lastAttackTime;
    private float attackDelayTime = 0.2f;

    public LayerMask targetLayer;
    public float detectRadius = 3f;
    public float stoppingDistance = 1.5f;

    private Transform currentTarget;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
    }

    private void Update()
    {
        if (Time.time < lastFindTime + findDelay) return;
        lastFindTime = Time.time;

        FindTarget();

        if (currentTarget != null)
        {
            // 현재 타겟까지 도달했는지 확인
            float remaining = agent.remainingDistance;

            if (!agent.pathPending && remaining <= agent.stoppingDistance)
            {
                MoveStop();
            }
        }
    }

    private void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectRadius, targetLayer);

        if (colliders.Length == 0)
        {
            currentTarget = null;
            MoveStop();
            return;
        }

        // 가장 가까운 유효한 타겟을 탐색
        Transform nearestTarget = null;
        float minDistance = float.MaxValue;

        foreach (var col in colliders)
        {
            var woman = col.GetComponent<Woman>();
            if (woman != null)
            {
                float dist = Vector3.Distance(transform.position, woman.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestTarget = woman.transform;
                }
            }
        }

        if (nearestTarget != null)
        {
            currentTarget = nearestTarget;
            MoveTo(currentTarget.position);
        }
        else
        {
            currentTarget = null;
            MoveStop();
        }
    }

    private void MoveTo(Vector3 targetPos)
    {
        agent.isStopped = false;
        agent.SetDestination(targetPos);
        agent.angularSpeed = 120f; // 회전 속도 초기화(선택)
    }

    private void MoveStop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.ResetPath();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }


    private void OnTriggerStay(Collider other)
    {
        if (Time.time >= lastAttackTime + attackDelayTime)
        {
            var target = other.GetComponent<Woman>();

            if (target != null && !target.isDie) {
                lastAttackTime = Time.time;

                target.OnDamage(10);
            }
        }
    }
}
