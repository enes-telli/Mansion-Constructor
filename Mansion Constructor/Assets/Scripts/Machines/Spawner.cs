using DG.Tweening;
using Items;
using PoolSystem;
using System.Collections;
using UnityEngine;

namespace Machines
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private StackArea _outputArea;
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
            _outputArea.onTriggerEnter += OutputTriggerEnter;
            _outputArea.onTriggerExit += OutputTriggerExit;
        }

        private void OnDisable()
        {
            _outputArea.onTriggerEnter -= OutputTriggerEnter;
            _outputArea.onTriggerExit -= OutputTriggerExit;
        }

        private void OutputTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                _assetGiving = StartCoroutine(GiveAssets(player));
            }
        }

        private void OutputTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                if (_assetGiving != null)
                    StopCoroutine(_assetGiving);
            }
        }

        private IEnumerator GiveAssets(Player player)
        {
            if (!player.IsStackTypeEquals(_outputArea.AssetData))
            {
                yield break;
            }

            while (true)
            {
                yield return new WaitUntil(() => !player.IsStackFull() && !_outputArea.IsEmpty());

                _outputArea.GiveAssets(player);

                yield return new WaitForSeconds(_takeDelayTime);
            }
        }

        private IEnumerator ProduceAssets()
        {
            while (true)
            {
                if (_outputArea.IsFull())
                {
                    yield return null;
                    continue;
                }

                var spawnedAsset = PoolManager.Instance.GetPooledObject<AssetBase>(_outputArea.AssetData.name);
                var spawnedAssetTransform = spawnedAsset.transform;
                spawnedAssetTransform.position = _spawnPoint.position;
                spawnedAssetTransform.SetParent(_outputArea.StackTransform);
                float randomJumpPower = Random.Range(1.2f, 1.3f);
                spawnedAssetTransform.DOLocalJump(_outputArea.GetFormatedAssetPosition(), randomJumpPower, 1, 0.4f);
                spawnedAssetTransform.DOScale(spawnedAssetTransform.localScale, 0.3f).From(Vector3.zero);
                spawnedAssetTransform.localRotation = Quaternion.identity;
                _outputArea.Assets.Add(spawnedAsset);
                _maxText.SetActive(_outputArea.IsFull());
                spawnedAsset.gameObject.SetActive(true);
                yield return new WaitForSeconds(_produceDelayTime);
            }
        }
    }
}