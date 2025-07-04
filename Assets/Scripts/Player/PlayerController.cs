using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float runSpeed;
    public float jumpPower;
    private Vector2 curMovementInput;
    public LayerMask groundLayerMask;
    private bool isRunning=false;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;
    public bool isFPS = true;

    

    public Action inventory;
    private Rigidbody _rigidbody;

    public Transform mainCameraPosition;
    public Camera equipCameraToggle;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        //equipCameraToggle = GetComponentInChildren<Camera>();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    void FixedUpdate()
    {
        Move();
        if (isRunning)
        {
            CharacterManager.Instance.Player.condition.uiCondition.stamina.Substract(Time.fixedDeltaTime*10f);
        }
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= isRunning? runSpeed : moveSpeed;
        dir.y = _rigidbody.velocity.y; // 수평이동 중 중력의 영향을 받거나, 점프를 할 때 자연스러운 포물선 움직임을 유지할 수 있게 됨.
        _rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isRunning = true;
            
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isRunning = false;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward*0.2f) + (transform.up*0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward*0.2f) + (transform.up*0.01f), Vector3.down),
            new Ray(transform.position + (transform.right*0.2f) + (transform.up*0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right*0.2f) + (transform.up*0.01f), Vector3.down),
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
    public void OnSwitchView(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            isFPS = !isFPS;
            if (isFPS)
            {
                mainCameraPosition.localPosition = Vector3.zero;
                equipCameraToggle.enabled = true;
                //equipCameraToggle.SetActive(true);
            }
            else
            {
                mainCameraPosition.localPosition = new Vector3(0, 0.3f, -2.0f);
                equipCameraToggle.enabled = false;
                //equipCameraToggle.SetActive(false);
            }
        }
    }
}
