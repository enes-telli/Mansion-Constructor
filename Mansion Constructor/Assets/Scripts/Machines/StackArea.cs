using DG.Tweening;
using Helpers;
using Items;
using PoolSystem;
using System.Collections.Generic;
using UnityEngine;

public class StackArea : TriggerArea
{
    [SerializeField] private int _capacity;
    [SerializeField] private GameObject _maxText;
    public AssetData AssetData;
    public Transform StackTransform;

    [Header("Stacking Formation")]
    [SerializeField] private int _row;
    [SerializeField] private float _rowSpacing;
    [Space]
    [SerializeField] private int _column;
    [SerializeField] private float _columnSpacing;
    [Space]
    [SerializeField] private float _heightGap;

    [Header("Jump Tween")]
    [SerializeField] private float _jumpPower;
    [SerializeField] private int _numJumps;
    [SerializeField] private float _duration;

    [HideInInspector] public List<AssetBase> Assets = new List<AssetBase>();

    public bool IsFull() => Assets.Count >= _capacity;
    public bool IsEmpty() => Assets.Count <= 0;

    public Vector3 GetFormatedAssetPosition()
    {
        int assetCount = Assets.Count;

        int rowIndex = assetCount % _row;
        int columnIndex = (assetCount / _row) % _column;

        float offsetX = (columnIndex - (_column - 1) / 2f) * _columnSpacing;
        float offsetY = assetCount / (_column * _row) * _heightGap;
        float offsetZ = (rowIndex - (_row - 1) / 2f) * _rowSpacing;

        return new Vector3(offsetX, offsetY, offsetZ);
    }

    public void TakeAsset(AssetBase asset, Vector3 fromPosition)
    {
        var assetTransform = asset.transform;
        assetTransform.DOKill();
        assetTransform.position = fromPosition;
        assetTransform.SetParent(StackTransform);
        assetTransform.DOLocalJump(GetFormatedAssetPosition(), _jumpPower, _numJumps, _duration);
        assetTransform.DOLocalRotate(asset.Data.Rotation, _duration);
        Assets.Add(asset);
        _maxText.SetActive(IsFull());
        asset.gameObject.SetActive(true);
    }

    public AssetBase GiveAsset(Transform toTransform, Vector3 toPosition, bool returnToPool = false)
    {
        var asset = Assets[^1];
        var assetTransform = asset.transform;
        assetTransform.DOKill();
        assetTransform.SetParent(toTransform);
        assetTransform.DOLocalJump(toPosition, _jumpPower, _numJumps, _duration).OnComplete(() =>
        {
            if (returnToPool)
                PoolManager.Instance.ReturnPooledObject(asset, asset.Data.name);
        });
        assetTransform.DOLocalRotate(asset.Data.Rotation, _duration);
        Assets.Remove(asset);
        _maxText.SetActive(IsFull());

        return asset;
    }
}
