using DG.Tweening;
using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class CharacterStack : MonoBehaviour
{
    [SerializeField] private Transform _stackTransform;
    [SerializeField] private int _capacity;
    [SerializeField] private AssetType _assetType;
    //[SerializeField] private AssetData _stackData;

    [HideInInspector] public List<AssetBase> Assets;

    public bool IsFull() => Assets.Count >= _capacity;
    public bool IsEmpty() => Assets.Count <= 0;

    public bool IsStackTypeEquals(AssetData assetData)
    {
        /*if (_stackData.Type.Equals(AssetType.DEFAULT) || _stackData.Type.Equals(assetData.Type))
        {
            _stackData = assetData;
            return true;
        }*/
        if (_assetType.Equals(AssetType.DEFAULT) || _assetType.Equals(assetData.Type))
        {
            _assetType = assetData.Type;
            return true;
        }
        return false;
    }

    public void TakeAsset(AssetBase asset)
    {
        var assetTransform = asset.transform;
        assetTransform.DOComplete();
        int killedTweenCount = assetTransform.DOComplete();
        //Debug.Log("[" + killedTweenCount + "] tweens completed" );
        assetTransform.SetParent(_stackTransform);
        assetTransform.DOLocalMove(Assets.Count * 0.25f * Vector3.up, 0.3f);
        assetTransform.DOLocalRotate(Vector3.zero, 0.3f);
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
