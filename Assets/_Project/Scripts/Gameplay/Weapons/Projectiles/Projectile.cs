using Gameplay.Data;
using UnityEngine;

namespace Gameplay.Weapons.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        private ProjectileData _data;
        private Vector3 _velocity;
        private float _gravity;
        private bool _isActive;

        public bool IsActive => _isActive;

        public void Spawn(ProjectileData data, Vector3 startPosition, Vector3 direction, float speed)
        {
            _data = data;
            transform.position = startPosition;
            transform.forward = direction;

            _velocity = direction * speed;
            _gravity = Mathf.Abs(Physics.gravity.y);

            _isActive = true;
            gameObject.SetActive(true);
        }

       
        public void Tick(float deltaTime)
        {
            if (!_isActive) return;

            _velocity.y -= _gravity * deltaTime;
            Vector3 nextPosition = transform.position + _velocity * deltaTime;

            Vector3 frameDirection = nextPosition - transform.position;
            float frameDistance = frameDirection.magnitude;

            if (Physics.Raycast(transform.position, frameDirection.normalized, out RaycastHit hit, frameDistance))
            {
                HandleCollision(hit);
                return; 
            }

            transform.position = nextPosition;

            if (frameDirection.sqrMagnitude > 0.001f)
            {
                transform.forward = frameDirection.normalized;
            }

            if (transform.position.y < -1f)
            {
                HandleSplash();
            }
        }

        private void HandleCollision(RaycastHit hit)
        {
            Debug.Log($"[{_data.Name}] Влучання у {hit.collider.gameObject.name}!");
            Deactivate();
        }

        private void HandleSplash()
        {
            Deactivate();
        }

        private void Deactivate()
        {
            _isActive = false;
            gameObject.SetActive(false);
        }
    }
} 