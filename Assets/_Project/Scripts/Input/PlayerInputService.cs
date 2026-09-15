using System;
using UnityEngine;
using UnityEngine.InputSystem;  

public class PlayerInputService : IShipInputService, IFleetInputService, IWeaponsInputService
{
    public event Action<int> OnThrottleStateChanged;

    public event Action OnNextUnit;
    public event Action OnPreviousUnit;

    public event Action OnFireRequested;

    private PlayerInputActions _inputActions;
    private float _steeringValue;

    private bool _isFreeLookActive;
    public bool IsFreeLookActive => _isFreeLookActive;

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

        _inputActions.ShipControl.FreeLook.performed += ctx => _isFreeLookActive = !_isFreeLookActive;
    }

    public float GetSteering() => _steeringValue;

    public Vector2 GetPointerPosition()
    {
        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }

        return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    }

    public void Disable()
    {
        _inputActions.ShipControl.Disable();
    }
}