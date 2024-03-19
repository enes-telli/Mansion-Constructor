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

    [Header("STACK PARAMETERS")]
    [SerializeField] private Transform stackTransform;
    public List<SpawnedAsset> stackedAssets;

    public bool IsStackFull() => stackedAssets.Count >= _stackCapacity;

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

    public void TakeSpawnedAsset(SpawnedAsset spawnedAsset)
    {
        var spawnedAssetTransform = spawnedAsset.transform;
        spawnedAssetTransform.DOComplete();
        int killedTweenCount = spawnedAssetTransform.DOComplete();
        //Debug.Log("[" + killedTweenCount + "] tweens completed" );
        spawnedAssetTransform.SetParent(stackTransform);
        spawnedAssetTransform.DOLocalMove(stackedAssets.Count * 0.25f * Vector3.up, 0.3f);
        spawnedAssetTransform.DOLocalRotate(Vector3.zero, 0.3f);
        stackedAssets.Add(spawnedAsset);
    }
}
