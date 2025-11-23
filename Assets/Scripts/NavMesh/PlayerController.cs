using UnityEngine;

namespace NavMesh
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5.0f;
        [SerializeField] private float _stoppingDistance = 0.5f;
        [SerializeField] private float _rotationSpeed = 5.0f;
        [SerializeField] private LayerMask _groundLayer;

        private Camera _camera;
        private CharacterController _characterController;
        private Vector3 _targetPosition;
        private bool _isMoving;
        private Vector3 _velosity;
        private readonly float _gravity = -9.81f;


        private void Start()
        {
            _camera = Camera.main;
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            SetTargetPosition();
            MoveToTarget();
            HandleGravity();
        }
    
        private void SetTargetPosition()
        {
            if (Input.GetMouseButton(0) && _camera != null)
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundLayer))
                {
                    _targetPosition = hit.point;
                    _isMoving = true;
                }
            }
        }

        private void MoveToTarget()
        {
            if (!_isMoving)
            {
                Vector3 startMove = new Vector3(0, _velosity.y * Time.deltaTime, 0);
                _characterController.Move(startMove);
                return;
            }

            Vector3 direction = _targetPosition - transform.position;
            direction.y = 0;

            if (direction.magnitude <= _stoppingDistance)
            {
                _isMoving = false;
                return;
            }

            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
            }

            Vector3 moveDirection = transform.forward;
            Vector3 movement = moveDirection * (_moveSpeed * Time.deltaTime);


            Vector3 combineMove = new Vector3(
                movement.x,
                _velosity.y * Time.deltaTime,
                movement.z
            );
        
            _characterController.Move(combineMove);
        }
    
        private void HandleGravity()
        {
            if (_characterController.isGrounded && _velosity.y < 0)
            {
                _velosity.y = -2f;
            }
            else
            {
                _velosity.y += _gravity * Time.deltaTime;
            }
        }
    }
}
