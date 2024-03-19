using DG.Tweening;
using Helpers;
using Items;
using PoolSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Machines
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private TriggerArea _outputArea;
        [SerializeField] private Transform _stackTransform;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private int _capacity;
        [SerializeField] private GameObject _maxText;

        [Header("Stacking Formation")]
        [SerializeField] private int _row;
        [SerializeField] private float _rowSpace;
        [Space]
        [SerializeField] private int _column;
        [SerializeField] private float _columnSpace;
        [Space]
        [SerializeField] private float _heightGap;

        private List<SpawnedAsset> _spawnedAssets = new List<SpawnedAsset>();

        private readonly float _produceDelayTime = 0.2f;
        private readonly float _takeDelayTime = 0.05f;

        private bool _productionStarted = false;

        private Coroutine _assetGiving;
        private Coroutine _production;

        private bool IsFull() => _spawnedAssets.Count >= _capacity;

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
                StopCoroutine(_assetGiving);
            }
        }

        private IEnumerator GiveAssets(Player player)
        {
            while (!player.IsStackFull())
            {
                SpawnedAsset spawnedAsset = _spawnedAssets[^1];
                _spawnedAssets.Remove(spawnedAsset);
                player.TakeSpawnedAsset(spawnedAsset);
                yield return new WaitForSeconds(_takeDelayTime);
            }

            yield return null;

            if (!_productionStarted)
            {
                _production = StartCoroutine(ProduceAssets());
            }
        }

        private IEnumerator ProduceAssets()
        {
            _productionStarted = true;
            while (true)
            {
                if (IsFull())
                {
                    yield return null;
                    continue;
                }

                var spawnedAsset = PoolManager.Instance.GetPooledObject<SpawnedAsset>("SpawnedAsset");
                var spawnedAssetTransform = spawnedAsset.transform;
                spawnedAssetTransform.position = _spawnPoint.position;
                spawnedAssetTransform.SetParent(_stackTransform);
                float randomJumpPower = Random.Range(1.2f, 1.3f);
                spawnedAssetTransform.DOLocalJump(GetSpawnedAssetPosition(), randomJumpPower, 1, 0.4f);
                spawnedAssetTransform.DOScale(spawnedAssetTransform.localScale/*Vector3.one*/, 0.3f).From(Vector3.zero);
                spawnedAssetTransform.localRotation = Quaternion.identity;
                _spawnedAssets.Add(spawnedAsset);
                _maxText.SetActive(IsFull());
                spawnedAsset.gameObject.SetActive(true);
                yield return new WaitForSeconds(_produceDelayTime);
            }
        }

        private Vector3 GetSpawnedAssetPosition()
        {
            int assetCount = _spawnedAssets.Count;

            int rowIndex = assetCount % _row;
            int columnIndex = (assetCount / _row) % _column;

            float offsetX = columnIndex * _columnSpace;
            float offsetY = (assetCount / (_column * _row)) * _heightGap;
            float offsetZ = rowIndex * _rowSpace;

            return new Vector3(offsetX, offsetY, offsetZ);
        }
    }
}