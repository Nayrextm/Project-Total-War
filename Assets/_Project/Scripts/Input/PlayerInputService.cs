using UnityEngine;

public class PlayerInputService : IShipInputService
{
    private PlayerInputActions _inputActions;
    private float _throttleValue;
    private float _steeringValue;

    public PlayerInputService()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.ShipControl.Enable();

        _inputActions.ShipControl.Throttle.performed += ctx => _throttleValue = ctx.ReadValue<float>();
        _inputActions.ShipControl.Throttle.canceled += ctx => _throttleValue = 0f;

        _inputActions.ShipControl.Steer.performed += ctx => _steeringValue = ctx.ReadValue<float>();
        _inputActions.ShipControl.Steer.canceled += ctx => _steeringValue = 0f;
    }

    public float GetThrottle() => _throttleValue;

    public float GetSteering() => _steeringValue;

    public void Disable()
    {
        _inputActions.ShipControl.Disable();
    }
}