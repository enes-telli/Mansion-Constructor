using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace AISystem
{
    public class Sheep : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private float _range;

        private void Update()
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (CanGetRandomPoint(out Vector3 targetPoint))
                {
                    _agent.SetDestination(targetPoint);
                }
            }
        }
        
        private bool CanGetRandomPoint(out Vector3 targetPoint)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * _range;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                targetPoint = hit.position;
                return true;
            }

            targetPoint = Vector3.zero;
            return false;
        }
    }
}