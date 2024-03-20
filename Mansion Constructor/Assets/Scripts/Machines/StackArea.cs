using Helpers;
using Items;
using System.Collections.Generic;
using UnityEngine;

public class StackArea : TriggerArea
{
    [SerializeField] private int _capacity;
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

    public void TakeAssets(Player player)
    {

    }

    public void GiveAssets(Player player)
    {
        AssetBase spawnedAsset = Assets[^1];
        Assets.Remove(spawnedAsset);
        player.TakeAsset(spawnedAsset);
    }

    public void ProduceAssets()
    {

    }
}
