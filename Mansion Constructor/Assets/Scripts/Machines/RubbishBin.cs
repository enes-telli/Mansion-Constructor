using DG.Tweening;
using PoolSystem;
using System.Collections;
using UnityEngine;

namespace Machines
{
    public class RubbishBin : MonoBehaviour
    {
        [SerializeField] private StackArea _inputArea;
        //[SerializeField] private Transform _spawnPoint;

        private readonly float _takeDelayTime = 0.02f;

        private Coroutine _assetTaking;

        private void OnEnable()
        {
            _inputArea.onTriggerEnter += InputTriggerEnter;
            _inputArea.onTriggerExit += InputTriggerExit;
        }

        private void OnDisable()
        {
            _inputArea.onTriggerEnter -= InputTriggerEnter;
            _inputArea.onTriggerExit -= InputTriggerExit;
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

        private IEnumerator TakeAssets(Player player)
        {
            var playerStackedAssets = player.StackedAssets;

            while (true)
            {
                yield return new WaitUntil(() => !player.IsStackEmpty()/* && !_inputArea.IsFull()*/);

                var asset = playerStackedAssets[^1];
                var assetTransform = asset.transform;
                assetTransform.SetParent(_inputArea.StackTransform);
                float randomJumpPower = Random.Range(1.2f, 1.3f);
                assetTransform.DOLocalJump(_inputArea.GetFormatedAssetPosition(), randomJumpPower, 1, 0.4f)
                    .OnComplete(() => PoolManager.Instance.ReturnPooledObject(asset, asset.Data.name));
                player.GiveAsset(asset);
                yield return new WaitForSeconds(_takeDelayTime);
            }
        }
    }
}