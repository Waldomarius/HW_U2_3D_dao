using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public class MoveAgent : MonoBehaviour
{
    [Header("Components")]
    private CharacterController _characterController;
    private Camera _camera;
    private NavMeshAgent _navMeshAgent;
    
    [Header("Movement")]
    [SerializeField] private float _rotationSpeed = 5.0f;
    
    [Header("GroundCheck")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckDistance;
    private readonly float _gravity = -9.81f;
    
    [Header("Debug")]
    [SerializeField] private bool _isDebugShow = true;
    private Vector3 _targetPosition;
    
    private Vector3 _velocity;
    private bool _isGrounded;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
        
        SetUpNavMeshAgent();
    }

    private void SetUpNavMeshAgent()
    {
        _navMeshAgent.updatePosition = true;
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = true;

        _navMeshAgent.angularSpeed = _rotationSpeed * 60f;
        _navMeshAgent.radius = _characterController.radius;
        _navMeshAgent.height = _characterController.height;
    }

    private void Update()
    {
        CheckGrounded();
        ApplyGravity();
        SetTargetPosition();
        RotateTowardsMovement();
    }

    private void RotateTowardsMovement()
    {
        if (_navMeshAgent.velocity.magnitude > 0.1f)
        {
            Vector3 direction = _navMeshAgent.velocity.normalized;
            direction.y = 0;
            
            if (direction.magnitude > 0.01f)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
            }
        }
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
                _navMeshAgent.isStopped = false;
                _navMeshAgent.SetDestination(_targetPosition);
                if (_isDebugShow)
                {
                    Debug.Log("Target set to: " + _targetPosition);
                }
            }
        }
    }

    private void ApplyGravity()
    {
        if (!_isGrounded)
        {
            if (_velocity.y < 0)
            {
                _velocity.y = -2f;
            }
            else
            {
                _velocity.y += _gravity * Time.deltaTime;
            }
            
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }

    private void CheckGrounded()
    {
        _isGrounded = _characterController.isGrounded;
        
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        bool isRayCastGrounded = Physics.Raycast(rayOrigin, Vector3.down, 2f, _groundLayer);
        
        _isGrounded = _isGrounded || isRayCastGrounded;
    }
}