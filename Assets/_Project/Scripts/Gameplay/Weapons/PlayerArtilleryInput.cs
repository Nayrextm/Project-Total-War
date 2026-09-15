using UnityEngine;
using System;

namespace Gameplay.Weapons
{
    public class PlayerArtilleryInput : MonoBehaviour
    {
        [Header("Targeting")]
        [Tooltip("Система керування вогнем корабля, яким зараз керує гравець")]
        [SerializeField] private FireControlSystem _fireControlSystem;

        [Tooltip("Шари, в які можуть влучати наші промені прицілювання")]
        [SerializeField] private LayerMask _targetLayerMask = ~0;

        private Camera _mainCamera;
        private Plane _waterPlane;
        private IWeaponsInputService _weaponsInput;

        private IPlayerFleetState _fleetState;

        private void Start()
        {
            _mainCamera = Camera.main;
            _waterPlane = new Plane(Vector3.up, Vector3.zero);

            _weaponsInput = ServiceLocator.Get<IWeaponsInputService>();

            if (_weaponsInput != null)
            {
                _weaponsInput.OnFireRequested += HandleFireRequest;
            }

            _fleetState = ServiceLocator.Get<IPlayerFleetState>();
            if (_fleetState != null)
            {
                _fleetState.OnActiveShipChanged += HandleShipChanged;

                if (_fleetState.ActiveShip != null)
                {
                    HandleShipChanged(_fleetState.ActiveShip);
                }
            }
        }

        private void OnDestroy()
        {
            if (_weaponsInput != null)
            {
                _weaponsInput.OnFireRequested -= HandleFireRequest;
            }

            if (_fleetState != null)
            {
                _fleetState.OnActiveShipChanged -= HandleShipChanged;
            }
        }

        private void HandleShipChanged(ShipController activeShip)
        {
            if (activeShip != null && activeShip.TryGetComponent<FireControlSystem>(out var newFCS))
            {
                PossessShip(newFCS);
            }
            else
            {
                PossessShip(null); // Відключаємо стрільбу, якщо на юніті немає зброї
            }
        }

        private void HandleFireRequest()
        {
            if (_fireControlSystem != null)
            {
                _fireControlSystem.FireSalvo();
            }
        }

        private void Update()
        {
            if (_fireControlSystem == null || _mainCamera == null || _weaponsInput == null) return;

            if (!_weaponsInput.IsFreeLookActive)
            {
                AimAtCrosshair();
            }
        }

        private void AimAtCrosshair()
        {
            if (_weaponsInput == null) return;

            Vector2 screenAimPoint = _weaponsInput.GetPointerPosition();

            Ray ray = _mainCamera.ScreenPointToRay(screenAimPoint);

            Vector3 targetPoint = Vector3.zero;
            bool targetFound = false;

            if (Physics.Raycast(ray, out RaycastHit hit, 20000f, _targetLayerMask))
            {
                targetPoint = hit.point;
                targetFound = true;
            }

            else if (_waterPlane.Raycast(ray, out float distanceToWater))
            {
                targetPoint = ray.GetPoint(distanceToWater);
                targetFound = true;
            }

            if (targetFound)
            {
                _fireControlSystem.SetTargetPoint(targetPoint);
            }
        }

        public void PossessShip(FireControlSystem newShipFCS)
        {
            _fireControlSystem = newShipFCS;
        }
    }
}