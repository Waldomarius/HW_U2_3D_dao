using UnityEngine;
using UnityEngine.AI;

namespace NavMesh
{
    public class PlayerNavMeshController : MonoBehaviour
    {
        [Header("MovementSettings")] [SerializeField]
        private float _moveSpeed = 5.0f;

        [SerializeField] private float _stoppingDistance = 0.5f;
        [SerializeField] private float _rotationSpeed = 5.0f;
        private bool _isMoving;
        private Vector3 _velosity;
        [SerializeField] private LayerMask _groundLayer;
        private readonly float _gravity = -9.81f;

        [Header("Components")] 
        private CharacterController _characterController;
        private Camera _camera;
        private NavMeshAgent _navMeshAgent;

        [Header("Debug")]
        [SerializeField] private bool _isDebugShow = true;
        private Vector3 _targetPosition;
        private bool _hasTarget = false;


        private void Start()
        {
            _camera = Camera.main;
            _characterController = GetComponent<CharacterController>();
            _navMeshAgent = GetComponent<NavMeshAgent>();

            _navMeshAgent.updatePosition = false;
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _navMeshAgent.speed = _moveSpeed;
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
                    _hasTarget = true;

                    _navMeshAgent.SetDestination(_targetPosition);
                    if (_isDebugShow)
                    {
                        Debug.Log("Target set to: " + _targetPosition);
                    }
                }
            }
        }

        private void MoveToTarget()
        {
            if (!_isMoving)
            {
                return;
            }

            Vector3 movementDirection = _navMeshAgent.desiredVelocity.normalized;
            Vector3 movement = movementDirection * (_moveSpeed * Time.deltaTime);

            _characterController.Move(movement);

            if (movementDirection != Vector3.zero)
            {
                RotateTodwardsMovement(movementDirection);
            }

            CheckTargetReached();
        }

        private void CheckTargetReached()
        {
            float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);

            if (distanceToTarget <= _stoppingDistance)
            {
                _isMoving = false;
                _hasTarget = false;
                _navMeshAgent.ResetPath();

                if (_isDebugShow)
                {
                    Debug.Log("Target reached!");
                }
            }
        }

        private void RotateTodwardsMovement(Vector3 direction)
        {
            direction.y = 0;

            if (direction.magnitude > 0.01f)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
            }
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

        public void StopMoving()
        {
            _isMoving = false;
            _hasTarget = false;
            _navMeshAgent.ResetPath();
        }

        public void SetNewTarget(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
            _hasTarget = true;
            _navMeshAgent.SetDestination(_targetPosition);
        }

        public void OnDrawGizmos()
        {
            if (_isDebugShow && _hasTarget)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _targetPosition);
            
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_targetPosition, 0.3f);
            }
        }
    }
}