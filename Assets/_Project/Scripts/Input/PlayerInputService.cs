using System;
using UnityEngine;

public class PlayerInputService : IShipInputService
{
    public event Action<int> OnThrottleStateChanged; 

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
    }

    public float GetSteering() => _steeringValue;

    public void Disable()
    {
        _inputActions.ShipControl.Disable();
    }
}