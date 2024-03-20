using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _characterAnimator;
    public CharacterStack Stack;
    [SerializeField] private float _speed;
    
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
        _characterAnimator.SetFloat("Walking", motionVector.magnitude);
        float runSpeed = inputDirection.magnitude * 0.5f + 0.5f;
        _characterAnimator.SetFloat("Speed", runSpeed);
        transform.LookAt(transform.position + worldDirection);

        _characterController.Move(motionVector);
    }

    private void Gravity()
    {
        if (_characterController.isGrounded) return;

        _characterController.Move(_gravity * Time.deltaTime);
    }
}
