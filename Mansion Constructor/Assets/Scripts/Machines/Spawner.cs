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
        [SerializeField] private float _produceDelayTime = 0.2f;
        [SerializeField] private float _takeDelayTime = 0.05f;

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
                {
                    StopCoroutine(_assetGiving);
                    _assetGiving = null;
                }
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

                var asset = OutputArea.GiveAsset(characterStack._stackTransform, characterStack.GetFormatedAssetPosition());
                characterStack.TakeAsset(asset);

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

                var asset = PoolManager.Instance.GetPooledObject<AssetBase>(OutputArea.AssetData.name);
                OutputArea.TakeAsset(asset, _spawnPoint.position);

                yield return new WaitForSeconds(_produceDelayTime);
            }
        }
    }
}