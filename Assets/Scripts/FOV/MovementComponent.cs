using UnityEngine;

namespace FOV
{
    public class MovementComponent : MonoBehaviour
    {
        [SerializeField] private float _speed = 5.0f;
        
        private Rigidbody _rigidbody;
        private Camera _camera;
        private Vector3 _velocity;
        
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _camera =  Camera.main;
        }
        
        void Update()
        {
            Vector3 mousePos = _camera.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.transform.position.y)
                );
            
            transform.LookAt(mousePos + Vector3.up * transform.position.y);
            _velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * _speed;
        }

        private void FixedUpdate()
        {
            _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
        }
    }
}
