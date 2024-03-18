using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _characterAnimator;
    [SerializeField] private float _speed;

    // void Initialize()
    void Start()
    {

    }

    private void OnEnable()
    {
        InputManager.Instance.OnInput += Move;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnInput -= Move;
    }

    void Update()
    {
        
    }

    private void Move(Vector2 inputDirection)
    {
        var worldDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
        var motionVector = _speed * Time.deltaTime * worldDirection;
        _characterAnimator.SetFloat("Speed", motionVector.magnitude);
        transform.LookAt(transform.position + worldDirection);

        _characterController.Move(motionVector);
    }
}
