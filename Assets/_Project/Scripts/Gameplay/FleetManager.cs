using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

[DefaultExecutionOrder(-50)]
public class FleetManager : MonoBehaviour, IPlayerFleetState
{
    public event Action<ShipController> OnActiveShipChanged;
    public ShipController ActiveShip { get; private set; }

    [Header("Camera Setup")]
    [SerializeField] private CinemachineCamera _playerCamera;

    [Header("Fleet Roster")]
    [SerializeField] private List<UnitBrain> _fleet;

    private IFleetInputService _fleetInput;
    private IShipInputService _playerInput;
    private int _currentIndex = 0;

    private void Awake()
    {
        ServiceLocator.Register<IPlayerFleetState>(this);
    }

    private void Start()
    {
        _fleetInput = ServiceLocator.Get<IFleetInputService>();
        _playerInput = ServiceLocator.Get<IShipInputService>();

        _fleetInput.OnNextUnit += SwitchToNextUnit;
        _fleetInput.OnPreviousUnit += SwitchToPreviousUnit;

        if (_fleet.Count > 0)
        {
            PossessUnit(_currentIndex);
        }
    }

    private void SwitchToNextUnit()
    {
        if (_fleet.Count <= 1) return;
        int newIndex = (_currentIndex + 1) % _fleet.Count;
        SwitchUnit(newIndex);
    }

    private void SwitchToPreviousUnit()
    {
        if (_fleet.Count <= 1) return;
        int newIndex = _currentIndex - 1;
        if (newIndex < 0) newIndex = _fleet.Count - 1;
        SwitchUnit(newIndex);
    }

    private void SwitchUnit(int newIndex)
    {
        _fleet[_currentIndex].Possess(null);
        _currentIndex = newIndex;
        PossessUnit(_currentIndex);
    }

    private void PossessUnit(int index)
    {
        UnitBrain targetShip = _fleet[index];
        targetShip.Possess(_playerInput);

        if (_playerCamera != null && targetShip.CameraPivot != null)
        {
            _playerCamera.Follow = targetShip.CameraPivot;
            _playerCamera.LookAt = targetShip.CameraPivot;
        }

        ActiveShip = targetShip.GetComponent<ShipController>();
        OnActiveShipChanged?.Invoke(ActiveShip);
    }

    private void OnDestroy()
    {
        if (_fleetInput != null)
        {
            _fleetInput.OnNextUnit -= SwitchToNextUnit;
            _fleetInput.OnPreviousUnit -= SwitchToPreviousUnit;
        }

        ServiceLocator.Unregister<IPlayerFleetState>();
    }
}