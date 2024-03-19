using DG.Tweening;
using Helpers;
using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Machines
{
    public class AssetTransformer : MonoBehaviour
    {
        [SerializeField] private TriggerArea _inputArea;
        [SerializeField] private TriggerArea _outputArea;

        [SerializeField] private Transform _spawnedAssetStackTransform;
        [SerializeField] private Transform _transformedAssetStackTransform;

        private Stack<SpawnedAsset> _spawnedAssets = new Stack<SpawnedAsset>();
        private Stack<TransformedAsset> _transformedAssets = new Stack<TransformedAsset>();

        private float _takeDelayTime = 0.05f;

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
                StopCoroutine(_assetGiving);
            }
        }

        private IEnumerator TakeAssets(Player player)
        {
            var playerStackedAssets = player.stackedAssets;

            while (playerStackedAssets.Count > 0)
            {
                var asset = playerStackedAssets[^1];
                var assetTransform = asset.transform;
                assetTransform.SetParent(_spawnedAssetStackTransform);
                assetTransform.localPosition = _spawnedAssets.Count * 0.135f * Vector3.up;
                assetTransform.DOScale(assetTransform.localScale/*Vector3.one*/, 0.3f).From(Vector3.zero);
                assetTransform.localRotation = Quaternion.identity;
                playerStackedAssets.Remove(asset);
                _spawnedAssets.Push(asset);

                yield return new WaitForSeconds(_takeDelayTime);
            }

            yield return null;

            if (!_productionStarted)
            {
                _production = StartCoroutine(ProduceAssets());
            }
        }

        private IEnumerator GiveAssets(Player player)
        {
            yield return null;

            if (!_productionStarted)
            {
                _production = StartCoroutine(ProduceAssets());
            }
        }

        private IEnumerator ProduceAssets()
        {
            _productionStarted = true;
            yield return null;
        }
    }
}