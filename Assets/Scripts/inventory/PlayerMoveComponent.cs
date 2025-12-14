using UnityEngine;

namespace inventory
{
    public class PlayerMoveComponent : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            Vector3 movement = new Vector3(horizontal, 0f, vertical) *  _moveSpeed;
            _rb.velocity = new Vector3(movement.x, _rb.velocity.y, movement.z);
        }
    }
}
