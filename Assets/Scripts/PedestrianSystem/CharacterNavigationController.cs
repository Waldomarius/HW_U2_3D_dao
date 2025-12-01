using UnityEngine;

namespace PedestrianSystem
{
    public class CharacterNavigationController : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _stopDistance;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private Vector3 _destination;
        [SerializeField] private string _waypointName;
        
        public bool _isReachedDestination;
        private CharacterController _characterController;
       
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }
        
        private void Update()
        {
            if (transform.position != _destination)
            {
                Vector3 destinationDirection = _destination - transform.position;
                destinationDirection.y = 0;
                
                float destinationDistance = destinationDirection.magnitude;
                _isReachedDestination = true;

                if (destinationDistance < _stopDistance)
                {
                    return;
                }

                _isReachedDestination = false;
                Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
                Vector3 movement = destinationDirection.normalized * (_speed * Time.deltaTime);
                movement.y = 0;
                _characterController.Move(movement);
            }
        }


        public void SetDestination(Vector3 pointPosition, string waypointName)
        {
            _destination = new Vector3(pointPosition.x, transform.position.y, pointPosition.z);
            _waypointName = waypointName;
            _isReachedDestination = false;
        }
    }
}