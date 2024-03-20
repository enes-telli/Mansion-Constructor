using Items;
using PoolSystem;
using System.Collections;
using UnityEngine;

namespace Machines
{
    public class AssetTransformer : MonoBehaviour
    {
        public StackArea InputArea;
        [SerializeField] private StackArea _outputArea;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private float _produceDelayTime = 0.2f;
        [SerializeField] private float _takeDelayTime = 0.05f;

        private bool _productionStarted = false;

        private Coroutine _assetTaking;
        private Coroutine _assetGiving;
        private Coroutine _production;

        private void OnEnable()
        {
            InputArea.onTriggerEnter += InputTriggerEnter;
            InputArea.onTriggerExit += InputTriggerExit;
            _outputArea.onTriggerEnter += OutputTriggerEnter;
            _outputArea.onTriggerExit += OutputTriggerExit;
        }

        private void OnDisable()
        {
            InputArea.onTriggerEnter -= InputTriggerEnter;
            InputArea.onTriggerExit -= InputTriggerExit;
            _outputArea.onTriggerEnter -= OutputTriggerEnter;
            _outputArea.onTriggerExit -= OutputTriggerExit;
        }

        private void InputTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                _assetTaking = StartCoroutine(TakeAssets(player.Stack));
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
                _assetGiving = StartCoroutine(GiveAssets(player.Stack));
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

        public IEnumerator TakeAssets(CharacterStack characterStack)
        {
            if (characterStack.IsEmpty()) yield break;
            if (!characterStack.IsStackTypeEquals(InputArea.AssetData)) yield break;
            
            var playerStackedAssets = characterStack.Assets;

            while (true)
            {
                yield return new WaitUntil(() => !characterStack.IsEmpty() && !InputArea.IsFull());

                var asset = playerStackedAssets[^1];
                characterStack.GiveAsset(asset);
                InputArea.TakeAsset(asset, asset.transform.position);

                yield return new WaitForSeconds(_takeDelayTime);

                if (!_productionStarted)
                {
                    _production = StartCoroutine(ProduceAssets());
                }
            }
        }

        private IEnumerator GiveAssets(CharacterStack characterStack)
        {
            while (_outputArea.IsEmpty())
            {
                yield return null;
            }

            if (!characterStack.IsStackTypeEquals(_outputArea.AssetData))
            {
                yield break;
            }

            while (true)
            {
                yield return new WaitWhile(() => characterStack.IsFull() || _outputArea.IsEmpty());

                var asset = _outputArea.GiveAsset(characterStack._stackTransform, characterStack.GetFormatedAssetPosition());
                characterStack.TakeAsset(asset);

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

                yield return new WaitWhile(() => InputArea.IsEmpty() || _outputArea.IsFull());

                var spawnedAsset = InputArea.GiveAsset(_spawnPoint, Vector3.zero, true);

                yield return new WaitForSeconds(0.5f);

                var asset = PoolManager.Instance.GetPooledObject<AssetBase>(_outputArea.AssetData.name);
                _outputArea.TakeAsset(asset, _spawnPoint.position);

                yield return new WaitForSeconds(_produceDelayTime);
            }
        }
    }
}