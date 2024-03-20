using DG.Tweening;
using Items;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _characterAnimator;
    [SerializeField] private float _speed;
    [SerializeField] private int _stackCapacity;
    //[SerializeField] private AssetData _stackData;
    [SerializeField] private AssetType _assetType;

    //[Header("STACK PARAMETERS")]
    [SerializeField] private Transform stackTransform;
    [HideInInspector] public List<AssetBase> StackedAssets;

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

    public bool IsStackFull() => StackedAssets.Count >= _stackCapacity;
    public bool IsStackEmpty() => StackedAssets.Count <= 0;

    private Vector3 _gravity;

    // void Initialize()
    void Start()
    {
        _gravity = Physics.gravity;

        InputManager.Instance.OnInput += Move;
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnInput -= Move;
    }

    void Update()
    {
        Gravity();
    }

    private void Move(Vector2 inputDirection)
    {
        var worldDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
        var motionVector = _speed * Time.deltaTime * worldDirection;
        _characterAnimator.SetFloat("Speed", motionVector.magnitude);
        transform.LookAt(transform.position + worldDirection);

        _characterController.Move(motionVector);
    }

    private void Gravity()
    {
        if (_characterController.isGrounded) return;

        _characterController.Move(_gravity * Time.deltaTime);
    }

    public void TakeAsset(AssetBase asset)
    {
        var spawnedAssetTransform = asset.transform;
        spawnedAssetTransform.DOComplete();
        int killedTweenCount = spawnedAssetTransform.DOComplete();
        //Debug.Log("[" + killedTweenCount + "] tweens completed" );
        spawnedAssetTransform.SetParent(stackTransform);
        spawnedAssetTransform.DOLocalMove(StackedAssets.Count * 0.25f * Vector3.up, 0.3f);
        spawnedAssetTransform.DOLocalRotate(Vector3.zero, 0.3f);
        StackedAssets.Add(asset);
    }

    public void GiveAsset(AssetBase asset)
    {
        StackedAssets.Remove(asset);

        if (StackedAssets.Count.Equals(0))
        {
            _assetType = AssetType.DEFAULT;
        }
    }
}
