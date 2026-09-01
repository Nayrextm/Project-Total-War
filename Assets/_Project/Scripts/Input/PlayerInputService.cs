using System;
using UnityEngine;

public class PlayerInputService : IShipInputService
{
    public event Action<int> OnGearShifted;

    private PlayerInputActions _inputActions;
    private float _steeringValue;

    public PlayerInputService()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.ShipControl.Enable();

        // Підписуємося на натискання кнопок
        _inputActions.ShipControl.SpeedUp.performed += ctx => OnGearShifted?.Invoke(1);
        _inputActions.ShipControl.SpeedDown.performed += ctx => OnGearShifted?.Invoke(-1);

        _inputActions.ShipControl.Steer.performed += ctx => _steeringValue = ctx.ReadValue<float>();
        _inputActions.ShipControl.Steer.canceled += ctx => _steeringValue = 0f;
    }

    public float GetSteering() => _steeringValue;

    public void Disable()
    {
        _inputActions.ShipControl.Disable();
    }
}