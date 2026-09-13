using UnityEngine;
using Gameplay.Data;

namespace Gameplay.Weapons
{
    public class FireControlSystem : MonoBehaviour
    {
        [Header("Main Battery Turrets")]
        [Tooltip("Перетягни сюди башти головного калібру з ієрархії")]
        [SerializeField] private TurretController[] _turrets;

        private Vector3 _currentTargetPoint;

        public void Initialize(ShipData shipData)
        {
            if (_turrets == null || _turrets.Length == 0)
            {
                Debug.LogWarning($"[{gameObject.name}] FireControlSystem: Не призначено жодної башти!");
                return;
            }

            for (int i = 0; i < _turrets.Length; i++)
            {
                _turrets[i].Initialize(shipData.MainBattery);
            }
        }

        public void SetTargetPoint(Vector3 worldPoint)
        {
            _currentTargetPoint = worldPoint;

            for (int i = 0; i < _turrets.Length; i++)
            {
                _turrets[i].SetAimPoint(_currentTargetPoint);
            }
        }

        public void FireSalvo()
        {
            if (_turrets == null || _turrets.Length == 0) return;

            bool firedAtLeastOne = false;

            for (int i = 0; i < _turrets.Length; i++)
            {
                TurretController turret = _turrets[i];

                if (turret.IsReadyToFire && turret.IsAimed)
                {
                    turret.Fire();
                    firedAtLeastOne = true;
                }
            }

            if (firedAtLeastOne)
            {
                // OnSalvoFired?.Invoke();
            }
        }
    }
}