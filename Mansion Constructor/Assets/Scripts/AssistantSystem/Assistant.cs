using Machines;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace AssistantSystem
{
    public class Assistant : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        [SerializeField] private CharacterStack _stack;

        [Header("Targets")]
        [SerializeField] private Spawner _spawner;
        [SerializeField] private AssetTransformer _assetTransformer;

        private Coroutine _assetGiving;
        private Coroutine _assetTaking;

        private void Start()
        {
            StartCoroutine(Work());
        }

        private IEnumerator Work()
        {
            while (true)
            {
                SetDestination(_spawner.OutputArea.transform);
                _animator.SetFloat("Speed", 1);
                yield return null;
                yield return new WaitUntil(() => _agent.remainingDistance < 0.1f);
                _animator.SetFloat("Speed", 0);
                yield return new WaitForSeconds(0.5f);

                _assetTaking = StartCoroutine(_spawner.GiveAssets(_stack));
                yield return new WaitUntil(() => _stack.IsFull());
                StopCoroutine(_assetTaking);
                _assetTaking = null;
                yield return new WaitForSeconds(0.5f);

                SetDestination(_assetTransformer.InputArea.transform);
                _animator.SetFloat("Speed", 1);
                yield return null;
                yield return new WaitUntil(() => _agent.remainingDistance < 0.1f);
                _animator.SetFloat("Speed", 0);
                yield return new WaitForSeconds(0.5f);

                _assetGiving = StartCoroutine(_assetTransformer.TakeAssets(_stack));
                yield return new WaitUntil(() => _stack.IsEmpty());
                StopCoroutine(_assetGiving);
                _assetGiving = null;
                yield return new WaitForSeconds(0.5f);
            }
        }

        private void SetDestination(Transform target)
        {
            Vector3 direction = (transform.position - target.position).normalized * 2f;
            _agent.SetDestination(target.position + direction);
        }
    }
}