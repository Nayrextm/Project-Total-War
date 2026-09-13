using System;
using UnityEngine;

public class PlayerInputService : IShipInputService, IFleetInputService, IWeaponsInputService
{
    public event Action<int> OnThrottleStateChanged;

    public event Action OnNextUnit;
    public event Action OnPreviousUnit;

    public event Action OnFireRequested;

    private PlayerInputActions _inputActions;
    private float _steeringValue;

    public PlayerInputService()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.ShipControl.Enable();

        _inputActions.ShipControl.Steer.performed += ctx => _steeringValue = ctx.ReadValue<float>();
        _inputActions.ShipControl.Steer.canceled += ctx => _steeringValue = 0f;

        _inputActions.ShipControl.Throttle.performed += ctx => OnThrottleStateChanged?.Invoke(Mathf.RoundToInt(ctx.ReadValue<float>()));
        _inputActions.ShipControl.Throttle.canceled += ctx => OnThrottleStateChanged?.Invoke(0);

        _inputActions.ShipControl.NextUnit.performed += ctx => OnNextUnit?.Invoke();
        _inputActions.ShipControl.PreviousUnit.performed += ctx => OnPreviousUnit?.Invoke();

        _inputActions.ShipControl.Fire.performed += ctx => OnFireRequested?.Invoke();
    }

    public float GetSteering() => _steeringValue;

    public void Disable()
    {
        _inputActions.ShipControl.Disable();
    }
}