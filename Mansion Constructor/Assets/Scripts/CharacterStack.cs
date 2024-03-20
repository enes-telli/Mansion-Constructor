using Items;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStack : MonoBehaviour
{
    public Transform _stackTransform;
    [SerializeField] private int _capacity;
    [SerializeField] private AssetType _assetType;
    [SerializeField] private float _stackGap = 0.3f;

    [HideInInspector] public List<AssetBase> Assets;

    public bool IsFull() => Assets.Count >= _capacity;
    public bool IsEmpty() => Assets.Count <= 0;

    public bool IsStackTypeEquals(AssetData assetData)
    {
        if (_assetType.Equals(AssetType.DEFAULT) || _assetType.Equals(assetData.Type))
        {
            _assetType = assetData.Type;
            return true;
        }
        return false;
    }

    public Vector3 GetFormatedAssetPosition()
    {
        return Assets.Count * _stackGap * Vector3.up;
    }

    public void TakeAsset(AssetBase asset)
    {
        Assets.Add(asset);
    }

    public void GiveAsset(AssetBase asset)
    {
        Assets.Remove(asset);

        if (Assets.Count.Equals(0))
        {
            _assetType = AssetType.DEFAULT;
        }
    }
}
