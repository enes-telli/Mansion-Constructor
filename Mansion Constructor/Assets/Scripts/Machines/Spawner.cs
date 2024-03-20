using DG.Tweening;
using Items;
using PoolSystem;
using System.Collections;
using UnityEngine;

namespace Machines
{
    public class Spawner : MonoBehaviour
    {
        public StackArea OutputArea;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GameObject _maxText;

        private readonly float _produceDelayTime = 0.2f;
        private readonly float _takeDelayTime = 0.05f;

        private Coroutine _assetGiving;
        private Coroutine _production;

        private void Start()
        {
            _production = StartCoroutine(ProduceAssets());
        }

        private void OnEnable()
        {
            OutputArea.onTriggerEnter += OutputTriggerEnter;
            OutputArea.onTriggerExit += OutputTriggerExit;
        }

        private void OnDisable()
        {
            OutputArea.onTriggerEnter -= OutputTriggerEnter;
            OutputArea.onTriggerExit -= OutputTriggerExit;
        }

        private void OutputTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CharacterStack characterStack))
            {
                _assetGiving = StartCoroutine(GiveAssets(characterStack));
            }
        }

        private void OutputTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out CharacterStack _))
            {
                if (_assetGiving != null)
                    StopCoroutine(_assetGiving);
            }
        }

        public IEnumerator GiveAssets(CharacterStack characterStack)
        {
            if (!characterStack.IsStackTypeEquals(OutputArea.AssetData))
            {
                yield break;
            }

            while (true)
            {
                yield return new WaitUntil(() => !characterStack.IsFull() && !OutputArea.IsEmpty());

                OutputArea.GiveAssets(characterStack);

                yield return new WaitForSeconds(_takeDelayTime);
            }
        }

        private IEnumerator ProduceAssets()
        {
            while (true)
            {
                if (OutputArea.IsFull())
                {
                    yield return null;
                    continue;
                }

                var spawnedAsset = PoolManager.Instance.GetPooledObject<AssetBase>(OutputArea.AssetData.name);
                var spawnedAssetTransform = spawnedAsset.transform;
                spawnedAssetTransform.position = _spawnPoint.position;
                spawnedAssetTransform.SetParent(OutputArea.StackTransform);
                float randomJumpPower = Random.Range(1.2f, 1.3f);
                spawnedAssetTransform.DOLocalJump(OutputArea.GetFormatedAssetPosition(), randomJumpPower, 1, 0.4f);
                spawnedAssetTransform.DOScale(spawnedAssetTransform.localScale, 0.3f).From(Vector3.zero);
                spawnedAssetTransform.localRotation = Quaternion.identity;
                OutputArea.Assets.Add(spawnedAsset);
                _maxText.SetActive(OutputArea.IsFull());
                spawnedAsset.gameObject.SetActive(true);
                yield return new WaitForSeconds(_produceDelayTime);
            }
        }
    }
}