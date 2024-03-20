using DG.Tweening;
using Items;
using PoolSystem;
using System.Collections;
using UnityEngine;

namespace Machines
{
    public class AssetTransformer : MonoBehaviour
    {
        [SerializeField] private StackArea _inputArea;
        [SerializeField] private StackArea _outputArea;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GameObject _maxText;

        private readonly float _produceDelayTime = 0.2f;
        private readonly float _takeDelayTime = 0.05f;

        private bool _productionStarted = false;

        private Coroutine _assetTaking;
        private Coroutine _assetGiving;
        private Coroutine _production;

        private void OnEnable()
        {
            _inputArea.onTriggerEnter += InputTriggerEnter;
            _inputArea.onTriggerExit += InputTriggerExit;
            _outputArea.onTriggerEnter += OutputTriggerEnter;
            _outputArea.onTriggerExit += OutputTriggerExit;
        }

        private void OnDisable()
        {
            _inputArea.onTriggerEnter -= InputTriggerEnter;
            _inputArea.onTriggerExit -= InputTriggerExit;
            _outputArea.onTriggerEnter -= OutputTriggerEnter;
            _outputArea.onTriggerExit -= OutputTriggerExit;
        }

        private void InputTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                _assetTaking = StartCoroutine(TakeAssets(player));
            }
        }

        private void InputTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player _))
            {
                if (_assetTaking != null)
                    StopCoroutine(_assetTaking);
            }
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
            if (other.TryGetComponent(out Player _))
            {
                if (_assetGiving != null)
                    StopCoroutine(_assetGiving);
            }
        }

        private IEnumerator TakeAssets(Player player)
        {
            if (player.IsStackEmpty()) yield break;
            if (!player.IsStackTypeEquals(_inputArea.AssetData)) yield break;
            
            var playerStackedAssets = player.StackedAssets;

            while (true)
            {
                yield return new WaitUntil(() => !player.IsStackEmpty() && !_inputArea.IsFull());

                var asset = playerStackedAssets[^1];
                var assetTransform = asset.transform;
                assetTransform.SetParent(_inputArea.StackTransform);
                assetTransform.DOLocalMove(_inputArea.GetFormatedAssetPosition(), 0.3f);
                assetTransform.localRotation = Quaternion.identity;
                player.GiveAsset(asset);
                _inputArea.Assets.Add(asset);

                yield return new WaitForSeconds(_takeDelayTime);

                if (!_productionStarted)
                {
                    _production = StartCoroutine(ProduceAssets());
                }
            }
        }

        private IEnumerator GiveAssets(Player player)
        {
            while (_outputArea.IsEmpty())
            {
                yield return null;
            }

            if (!player.IsStackTypeEquals(_outputArea.AssetData))
            {
                yield break;
            }

            while (true)
            {
                yield return new WaitWhile(() => player.IsStackFull() || _outputArea.IsEmpty());

                _outputArea.GiveAssets(player);

                yield return new WaitForSeconds(_takeDelayTime);
            }
        }

        private IEnumerator ProduceAssets()
        {
            _productionStarted = true;
            while (true)
            {
                if (_outputArea.IsFull())
                {
                    yield return null;
                    continue;
                }

                yield return new WaitWhile(() => _inputArea.IsEmpty() || _outputArea.IsFull());

                var spawnedAsset = _inputArea.Assets[^1];
                var spawnedAssetTransform = spawnedAsset.transform;
                float randomJumpPower = Random.Range(1.2f, 1.3f);
                spawnedAssetTransform.DOJump(_spawnPoint.position, randomJumpPower, 1, 0.4f)
                    .OnComplete(() => PoolManager.Instance.ReturnPooledObject(spawnedAsset, spawnedAsset.Data.name));
                _inputArea.Assets.Remove(spawnedAsset);
                //_maxText.SetActive(_outputArea.IsFull());

                yield return new WaitForSeconds(0.5f);

                var transformedAsset = PoolManager.Instance.GetPooledObject<AssetBase>(_outputArea.AssetData.name);
                var transformedAssetTransform = transformedAsset.transform;
                transformedAssetTransform.position = _spawnPoint.position;
                transformedAssetTransform.SetParent(_outputArea.StackTransform);
                transformedAssetTransform.DOLocalJump(_outputArea.GetFormatedAssetPosition(), randomJumpPower, 1, 0.4f);
                transformedAssetTransform.DOScale(transformedAssetTransform.localScale, 0.3f).From(Vector3.zero);
                transformedAssetTransform.eulerAngles = new Vector3(-90f, 0f, 0f);
                _outputArea.Assets.Add(transformedAsset);
                _maxText.SetActive(_outputArea.IsFull());
                transformedAsset.gameObject.SetActive(true);

                yield return new WaitForSeconds(_produceDelayTime);
            }
        }
    }
}