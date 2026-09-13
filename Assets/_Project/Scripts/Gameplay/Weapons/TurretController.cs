using UnityEngine;
using Gameplay.Data;

namespace Gameplay.Weapons
{
    public class TurretController : MonoBehaviour
    {
        [field: SerializeField, Header("State (Read Only)")]
        public bool IsAimed { get; private set; }

        [field: SerializeField]
        public bool IsReadyToFire { get; private set; }

        private float _reloadTimer;
        private const float AIM_TOLERANCE_DEGREES = 1.5f; // Похибка в 1.5 градуса для дозволу на постріл


        [Header("Pivots")]
        [SerializeField] private Transform _azimuthPivot;   
        [SerializeField] private Transform _elevationPivot; 

        [Header("Limits")]
        [SerializeField] private float _minElevation = -5f;  
        [SerializeField] private float _maxElevation = 45f;  

        private ArtillerySettings _stats;
        private Vector3 _targetPoint;
        private bool _hasTarget = false;

        public void Fire()
        {
            if (!IsReadyToFire || !IsAimed) return;

            // Тут пізніше будемо спавнити снаряд або викликати події (VFX, звук)
            Debug.Log($"[{gameObject.name}] BOOM! Віддача, дим, вогонь!");

            // Скидаємо перезарядку
            _reloadTimer = _stats.ReloadTime;
            IsReadyToFire = false;
        }

        public void Initialize(ArtillerySettings stats)
        {
            _stats = stats;
        }

        // Fire Control System буде викликати цей метод кожного кадру
        public void SetAimPoint(Vector3 worldPoint)
        {
            _targetPoint = worldPoint;
            _hasTarget = true;
        }

        private void Update()
        {
            if (!_hasTarget || _azimuthPivot == null || _elevationPivot == null) return;

            AimTurret();

            if (_reloadTimer > 0f)
            {
                _reloadTimer -= Time.deltaTime;
            }
            IsReadyToFire = _reloadTimer <= 0f;
        }

        private void AimTurret()
        {
            Vector3 directionToTarget = _targetPoint - _azimuthPivot.position;
            directionToTarget.y = 0f;

            if (directionToTarget.sqrMagnitude > 0.01f)
            {
                Quaternion targetAzimuthRotation = Quaternion.LookRotation(directionToTarget);

                _azimuthPivot.rotation = Quaternion.RotateTowards(
                    _azimuthPivot.rotation,
                    targetAzimuthRotation,
                    _stats.AzimuthTurnRate * Time.deltaTime
                );
            }

            Vector3 localTargetPos = _azimuthPivot.InverseTransformPoint(_targetPoint);
            float distancePlane = new Vector2(localTargetPos.x, localTargetPos.z).magnitude;

            // Математичний кут підняття стволів (Балістику додамо пізніше, поки пряме наведення)
            float targetElevationAngle = Mathf.Atan2(localTargetPos.y, distancePlane) * Mathf.Rad2Deg;

            // Затискаємо кут у реалістичних межах конструкції башти
            targetElevationAngle = Mathf.Clamp(targetElevationAngle, _minElevation, _maxElevation);

            // Оскільки стволи дивляться вперед (по осі Z), підняття вгору — це від'ємний кут по осі X
            Quaternion targetElevationRotation = Quaternion.Euler(-targetElevationAngle, 0f, 0f);

            _elevationPivot.localRotation = Quaternion.RotateTowards(
                _elevationPivot.localRotation,
                targetElevationRotation,
                _stats.ElevationTurnRate * Time.deltaTime
            );

            // Перевіряємо, чи дивиться ствол на ціль (ігноруючи висоту для простоти бази)
            Vector3 currentForward = _azimuthPivot.forward;
            currentForward.y = 0f;
            directionToTarget.y = 0f;

            // Використовуємо швидку математику без алокацій
            float angleToTarget = Vector3.Angle(currentForward.normalized, directionToTarget.normalized);
            IsAimed = angleToTarget <= AIM_TOLERANCE_DEGREES;
        }
    }
}