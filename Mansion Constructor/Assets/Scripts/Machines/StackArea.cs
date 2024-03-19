using Helpers;
using Items;
using System.Collections.Generic;
using UnityEngine;

public class StackArea : TriggerArea
{
    [SerializeField] private int _capacity;
    public Transform StackTransform;

    [Header("Stacking Formation")]
    [SerializeField] private int _row;
    [SerializeField] private float _rowSpace;
    [Space]
    [SerializeField] private int _column;
    [SerializeField] private float _columnSpace;
    [Space]
    [SerializeField] private float _heightGap;

    [HideInInspector] public List<SpawnedAsset> Assets = new List<SpawnedAsset>();

    public bool IsFull() => Assets.Count >= _capacity;

    public Vector3 GetAssetPosition()
    {
        int assetCount = Assets.Count;

        int rowIndex = assetCount % _row;
        int columnIndex = (assetCount / _row) % _column;

        float offsetX = (columnIndex - (_column - 1) / 2f) * _columnSpace;
        float offsetY = assetCount / (_column * _row) * _heightGap;
        float offsetZ = (rowIndex - (_row - 1) / 2f) * _rowSpace;

        return new Vector3(offsetX, offsetY, offsetZ);
    }

    public void TakeAssets(Player player)
    {

    }

    public void GiveAssets(Player player)
    {
        SpawnedAsset spawnedAsset = Assets[^1];
        Assets.Remove(spawnedAsset);
        player.TakeSpawnedAsset(spawnedAsset);
    }

    public void ProduceAssets()
    {

    }
}
