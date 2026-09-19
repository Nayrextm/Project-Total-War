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
            // TODO: пізніше можна зробити локальний множник гравітації для "Золотої середини"

            _isActive = true;
            gameObject.SetActive(true);
        }

       
        // ProjectilePool - (Update Manager).
        public void Tick(float deltaTime)
        {
            if (!_isActive) return;

            // 1. Розрахунок балістичної параболи
            _velocity.y -= _gravity * deltaTime;
            Vector3 nextPosition = transform.position + _velocity * deltaTime;

            // 2. Перевірка зіткнення через Raycast (запобігає пролітанню крізь стіни)
            Vector3 frameDirection = nextPosition - transform.position;
            float frameDistance = frameDirection.magnitude;

            // Raycast б'є тільки на ту відстань, яку снаряд пролетить за цей один кадр
            if (Physics.Raycast(transform.position, frameDirection.normalized, out RaycastHit hit, frameDistance))
            {
                HandleCollision(hit);
                return; 
            }

            // 3. Якщо не влучили — рухаємо снаряд і повертаємо його "носом" по вектору польоту
            transform.position = nextPosition;

            if (frameDirection.sqrMagnitude > 0.001f)
            {
                transform.forward = frameDirection.normalized;
            }

            // 4. Утилізація при падінні у воду (якщо Y впав нижче нуля)
            if (transform.position.y < -1f)
            {
                HandleSplash();
            }
        }

        private void HandleCollision(RaycastHit hit)
        {
            // TODO: Спавн VFX вибуху на броні, передача інтерфейсу IDamageable для розрахунку шкоди
            Debug.Log($"[{_data.Name}] Влучання у {hit.collider.gameObject.name}!");
            Deactivate();
        }

        private void HandleSplash()
        {
            // TODO: Спавн VFX сплеску води
            Deactivate();
        }

        private void Deactivate()
        {
            _isActive = false;
            gameObject.SetActive(false);
            // Снаряд засинає і готовий до повторного використання пулом
        }
    }
} 