using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace nis
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private Rigidbody _rb;

        private InputAction _jumpAction;
        private InputAction _moveAction;

        [Header("Moving options")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpForce = 5f;

        private Vector2 _moveInput =  Vector2.zero;
        
        private void Awake()
        {

            _rb = GetComponent<Rigidbody>();
            _playerInput = GetComponent<PlayerInput>();

            // Debug.Log($"gggggggggggggggggggggggggggggg:  {_playerInput.actions["Moved"]}");
       
            _moveAction = _playerInput.actions["Moved"];
            // Debug.Log(_moveAction.bindings);
            
            //
            // _jumpAction = _playerInput.actions["Jump"];
            // Debug.Log(_jumpAction);


            // if (InputSystem.actions != null)
            // {
            //     _moveAction = InputSystem.actions.FindAction("Player/Move");
            //     _jumpAction = InputSystem.actions.FindAction("Player/Jump");
            // }
            //
            // if (_playerInput != null)
            // {
            //     _moveAction = _playerInput.FindAction("Player/Move");
            //     _jumpAction = _playerInput.FindAction("Player/Jump");
            // }
        }

        private void Update()
        {
            Vector2 move = _moveAction.ReadValue<Vector2>();
            Debug.Log(move);
            
            // bool jump = _jumpAction.ReadValue<bool>();
            // Debug.Log(jump);

        }

        private void OnEnable()
        {
            _moveAction.Enable();
            // _jumpAction?.Enable();
            
            _moveAction.performed += OnMovePerformed;
            // _moveAction.canceled += OnMoveCanceled;
            //
            // _jumpAction.performed += OnJumpPerfoprmed;
        }

        private void OnDisable()
        {
            _moveAction.Disable();
            // _jumpAction?.Disable();
            
            _moveAction.performed -= OnMovePerformed;
            // _moveAction.canceled -= OnMoveCanceled;
            //
            // _jumpAction.performed -= OnJumpPerfoprmed;
        }
        
        private void OnJumpPerfoprmed(InputAction.CallbackContext context)
        {
            // _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            Debug.Log("OnJumpPerfoprmed");
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
            Debug.Log($"ddddddddddddddddddddddd{_moveInput}");
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            // _moveInput = context.ReadValue<Vector2>();
            Debug.Log("-------------------------------------------   OnMovePerformed");
        }



        private void FixedUpdate()
        {
            // Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y) * (_moveSpeed *  Time.fixedDeltaTime);
            // _rb.MovePosition(transform.position + move);
            // _rb.velocity = new Vector3(_moveInput.x, 0, _moveInput.y) * (_moveSpeed * Time.fixedDeltaTime);
        }


        // private void OnEnable()
        // {
        //     _moveAction?.Enable();
        //     _jumpAction?.Enable();
        //
        //     if (_moveAction != null)
        //     {
        //         // _moveAction.performed += MoveAction;
        //     }
        // }
        // private void Awake()
        // {
        //     _rb = GetComponent<Rigidbody>();
        //     _controls =  new NISActions();
        // }
        //
        // private void OnEnable()
        // {
        //     _controls.Player.Enable();
        //
        //     _controls.Player.Moved.performed += OnMovePerformed;
        //     _controls.Player.Moved.canceled += OnMoveCanceled;
        //     _controls.Player.Jump.performed += OnJumpPerfprmed;
        //     
        //     // _controls.Player.Moved.performed += Explosion;
        //     // _controls.Player.Moved.canceled += Explosion;
        //     // _controls.Player.Jump.performed += Explosion;
        // }
        //
        // // private void Explosion(InputAction.CallbackContext context)
        // // {
        // //     Debug.Log("Explosion");
        // //     
        // //     context.ReadValue<Vector2>();
        // //     context.ReadValueAsButton();
        // //     
        // // }
        //
        // private void OnDisable()
        // {
        //     _controls.Player.Disable();
        //     
        //     _controls.Player.Moved.performed -= OnMovePerformed;
        //     _controls.Player.Moved.canceled -= OnMoveCanceled;
        //     _controls.Player.Jump.performed -= OnJumpPerfprmed;
        // }
        //
        // private void OnJumpPerfprmed(InputAction.CallbackContext context)
        // {
        //     _rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        //     Debug.Log("Jump performed");
        // }
        //
        // private void OnMoveCanceled(InputAction.CallbackContext context)
        // {
        //     _moveInput = Vector2.zero;
        // }
        //
        // private void OnMovePerformed(InputAction.CallbackContext context)
        // {
        //     _moveInput = context.ReadValue<Vector2>();
        // }
        //
        // private void FixedUpdate()
        // {
        //     Vector3 movement = new Vector3(_moveInput.x, 0f, _moveInput.y) * (10f * Time.deltaTime);
        //     _rb.MovePosition(transform.position + movement);
        // }
    }
}