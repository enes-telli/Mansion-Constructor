using DG.Tweening;
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
        [SerializeField] private GameObject _maxText;

        private readonly float _produceDelayTime = 0.2f;
        private readonly float _takeDelayTime = 0.05f;

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
            if (other.TryGetComponent(out CharacterStack characterStack))
            {
                _assetTaking = StartCoroutine(TakeAssets(characterStack));
            }
        }

        private void InputTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out CharacterStack _))
            {
                if (_assetTaking != null)
                    StopCoroutine(_assetTaking);
            }
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

        public IEnumerator TakeAssets(CharacterStack characterStack)
        {
            if (characterStack.IsEmpty()) yield break;
            if (!characterStack.IsStackTypeEquals(InputArea.AssetData)) yield break;
            
            var playerStackedAssets = characterStack.Assets;

            while (true)
            {
                yield return new WaitUntil(() => !characterStack.IsEmpty() && !InputArea.IsFull());

                var asset = playerStackedAssets[^1];
                var assetTransform = asset.transform;
                assetTransform.SetParent(InputArea.StackTransform);
                assetTransform.DOLocalMove(InputArea.GetFormatedAssetPosition(), 0.3f);
                assetTransform.localRotation = Quaternion.identity;
                characterStack.GiveAsset(asset);
                InputArea.Assets.Add(asset);

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

                _outputArea.GiveAssets(characterStack);

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

                var spawnedAsset = InputArea.Assets[^1];
                var spawnedAssetTransform = spawnedAsset.transform;
                float randomJumpPower = Random.Range(1.2f, 1.3f);
                spawnedAssetTransform.DOJump(_spawnPoint.position, randomJumpPower, 1, 0.4f)
                    .OnComplete(() => PoolManager.Instance.ReturnPooledObject(spawnedAsset, spawnedAsset.Data.name));
                InputArea.Assets.Remove(spawnedAsset);
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