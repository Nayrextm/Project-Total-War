using Gameplay.Data;
using Gameplay.Weapons.Projectiles;
using UnityEngine;

namespace Gameplay.Weapons
{
    public class TurretController : MonoBehaviour
    {
        [field: SerializeField, Header("State (Read Only)")]
        public bool IsAimed { get; private set; }

        [field: SerializeField]
        public bool IsReadyToFire { get; private set; }

        private float _reloadTimer;
        private const float AIM_TOLERANCE_DEGREES = 1.5f;

        public enum FiringMode
        {
            Salvo,      // Salvo: all barrels simultaneously
            Sequential, // One by one: one barrel per click (air defense)
            Burst       // Cascade: barrel -> pause -> barrel -> reload
        }

        [Header("Fire Points & Mode")]
        [SerializeField] private FiringMode _firingMode = FiringMode.Salvo;
        [SerializeField] private Transform[] _firePoints;
        private int _currentBarrelIndex = 0;

        private bool _isBursting = false;
        private int _burstShotsFired = 0;
        private float _burstTimer = 0f;

        [Header("Pivots")]
        [SerializeField] private Transform _azimuthPivot;
        [SerializeField] private Transform _elevationPivot;

        [Header("Limits")]
        [SerializeField] private float _minElevation = -5f;
        [SerializeField] private float _maxElevation = 45f;

        private ArtillerySettings _stats;
        private Vector3 _targetPoint;
        private bool _hasTarget = false;

        public void Initialize(ArtillerySettings stats)
        {
            _stats = stats;
            IsReadyToFire = true;
            _reloadTimer = 0f;
            _isBursting = false;
        }

        public void SetAimPoint(Vector3 worldPoint)
        {
            _targetPoint = worldPoint;
            _hasTarget = true;
        }

        public void Fire()
        {
            if (!IsReadyToFire || !IsAimed || _isBursting) return;

            var projectilePool = ServiceLocator.Get<IProjectilePoolService>();
            if (projectilePool == null || _stats.AmmoData == null || _firePoints.Length == 0) return;

            switch (_firingMode)
            {
                case FiringMode.Salvo:
                    for (int i = 0; i < _firePoints.Length; i++)
                    {
                        SpawnProjectileFromPoint(projectilePool, _firePoints[i]);
                    }
                    StartReload();
                    break;

                case FiringMode.Sequential:
                    SpawnProjectileFromPoint(projectilePool, _firePoints[_currentBarrelIndex]);
                    _currentBarrelIndex = (_currentBarrelIndex + 1) % _firePoints.Length;
                    StartReload();
                    break;

                case FiringMode.Burst:
                    _isBursting = true;
                    _burstShotsFired = 0;
                    _burstTimer = 0f;
                    break;
            }
        }

        private void Update()
        {
            if (!_hasTarget || _azimuthPivot == null || _elevationPivot == null) return;

            AimTurret();

            if (_isBursting)
            {
                ProcessBurst();
            }
            else
            {
                if (_reloadTimer > 0f)
                {
                    _reloadTimer -= Time.deltaTime;

                    if (_reloadTimer <= 0f)
                    {
                        IsReadyToFire = true;
                    }
                }
            }
        }

        private void ProcessBurst()
        {
            _burstTimer -= Time.deltaTime;

            if (_burstTimer <= 0f)
            {
                var pool = ServiceLocator.Get<IProjectilePoolService>();

                Transform firePoint = _firePoints[_burstShotsFired];
                SpawnProjectileFromPoint(pool, firePoint);

                _burstShotsFired++;

                if (_burstShotsFired >= _firePoints.Length)
                {
                    _isBursting = false;
                    StartReload();
                }
                else
                {
                    _burstTimer = _stats.BurstDelay;
                }
            }
        }

        private void StartReload()
        {
            if (_stats.ReloadTime <= 0f)
            {
                _reloadTimer = 0f;
                IsReadyToFire = true;
            }
            else
            {
                _reloadTimer = _stats.ReloadTime;
                IsReadyToFire = false;
            }
        }

        private void SpawnProjectileFromPoint(IProjectilePoolService pool, Transform firePoint)
        {
            Projectile proj = pool.GetProjectile(_stats.AmmoData);
            if (proj != null)
            {
                proj.Spawn(_stats.AmmoData, firePoint.position, _elevationPivot.forward, _stats.MuzzleVelocity);
            }
        }

        private float CalculateBallisticAngle(float distance, float heightDiff, float muzzleVelocity)
        {
            if (distance <= 0.1f) return 0f;

            float g = Mathf.Abs(Physics.gravity.y);
            float v = muzzleVelocity;
            float v2 = v * v;
            float v4 = v2 * v2;

            float root = v4 - g * (g * distance * distance + 2f * heightDiff * v2);

            if (root < 0f)
            {
                return 45f;
            }

            float angleRad = Mathf.Atan((v2 - Mathf.Sqrt(root)) / (g * distance));
            return angleRad * Mathf.Rad2Deg;
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
            float distanceToTarget = new Vector2(localTargetPos.x, localTargetPos.z).magnitude;
            float heightDifference = localTargetPos.y;

            float targetElevationAngle = CalculateBallisticAngle(distanceToTarget, heightDifference, _stats.MuzzleVelocity);
            targetElevationAngle = Mathf.Clamp(targetElevationAngle, _minElevation, _maxElevation);

            Quaternion targetElevationRotation = Quaternion.Euler(-targetElevationAngle, 0f, 0f);

            _elevationPivot.localRotation = Quaternion.RotateTowards(
                _elevationPivot.localRotation,
                targetElevationRotation,
                _stats.ElevationTurnRate * Time.deltaTime
            );

            Vector3 currentForward = _azimuthPivot.forward;
            currentForward.y = 0f;
            directionToTarget.y = 0f;

            float angleToTarget = Vector3.Angle(currentForward.normalized, directionToTarget.normalized);
            IsAimed = angleToTarget <= AIM_TOLERANCE_DEGREES;
        }
    }
}