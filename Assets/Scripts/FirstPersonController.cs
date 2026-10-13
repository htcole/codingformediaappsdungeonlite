using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [Header("Settings")]
    public float currentMoveSpeed;
    public float walkSpeed = 5f;
    public float sprintSpeed = 7.5f;
    public float mouseSensitivity = 2f;

    private bool sprinting;


    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private Camera _mainCamera;
    private float _pitch;

    void Awake()
    {
        _mainCamera = Camera.main;
        _playerInput = GetComponent<PlayerInput>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _moveAction = _playerInput.actions["Move"];
        _lookAction = _playerInput.actions["Look"];
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
    }

    void HandleMovement()
    {
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        transform.Translate(moveDirection *currentMoveSpeed * Time.deltaTime, Space.Self);

        if (Keyboard.current.leftShiftKey.isPressed)
        {
            sprinting = true;
        }
        else
        {
            sprinting = false;
        }

            currentMoveSpeed = (sprinting) ? sprintSpeed : walkSpeed;
    }

    void HandleLook()
    {
        Vector2 lookInput = _lookAction.ReadValue<Vector2>();
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

        _pitch -= lookInput.y * mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, -85f, 85f); //how far the player can look up or down
        _mainCamera.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }
}