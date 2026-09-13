using System;
using UnityEngine;
using Gameplay.Data;

public enum EngineGear
{
    FullAstern = -3,
    HalfAstern = -2,
    SlowAstern = -1,
    Stop = 0,
    SlowAhead = 1,
    HalfAhead = 2,
    FullAhead = 3
}

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    public event Action<EngineGear> OnGearChanged;
    public float CurrentSpeed => _currentSpeed;
    public ShipData Data => _shipData;

    [SerializeField] private ShipData _shipData;
    [SerializeField] private Transform _visualModel;

    private Rigidbody _rigidbody;

    [Header("Gear Settings")]
    [SerializeField] private float _gearShiftDelay = 0.3f;
    private float _lastShiftTime;
    private int _currentThrottleInput = 0; 
    public EngineGear CurrentGear { get; private set; } = EngineGear.Stop;

    private float _currentSpeed;
    private float _targetSpeed;
    private float _currentTurnRate;
    private float _targetTurnRate;
    private float _currentHeelAngle;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Initialize(ShipData data)
    {
        _shipData = data;
    }

    public void SetThrottleInput(int direction)
    {
        _currentThrottleInput = direction;

        if (_currentThrottleInput == 0)
        {
            _lastShiftTime = 0f;
        }
    }
    private void Update()
    {
        ProcessContinuousGearShifting();
    }

    private void ProcessContinuousGearShifting()
    {
        if (_currentThrottleInput != 0)
        {
            if (Time.time >= _lastShiftTime + _gearShiftDelay)
            {
                ShiftGear(_currentThrottleInput);
                _lastShiftTime = Time.time;
            }
        }
    }
    public void ShiftGear(int step)
    {
        int newGearValue = (int)CurrentGear + step;
        newGearValue = Mathf.Clamp(newGearValue, (int)EngineGear.FullAstern, (int)EngineGear.FullAhead);

        if (CurrentGear != (EngineGear)newGearValue)
        {
            CurrentGear = (EngineGear)newGearValue;
            UpdateTargetSpeed();
            OnGearChanged?.Invoke(CurrentGear);
        }
    }

    private void UpdateTargetSpeed()
    {
        float maxReverseSpeed = _shipData.MaxSpeed * 0.2f;

        switch (CurrentGear)
        {
            case EngineGear.FullAstern: _targetSpeed = -maxReverseSpeed; break;
            case EngineGear.HalfAstern: _targetSpeed = -(maxReverseSpeed * 0.66f); break;
            case EngineGear.SlowAstern: _targetSpeed = -(maxReverseSpeed * 0.33f); break;
            case EngineGear.Stop: _targetSpeed = 0f; break;
            case EngineGear.SlowAhead: _targetSpeed = _shipData.MaxSpeed * 0.33f; break;
            case EngineGear.HalfAhead: _targetSpeed = _shipData.MaxSpeed * 0.66f; break;
            case EngineGear.FullAhead: _targetSpeed = _shipData.MaxSpeed; break;
        }
    }

    public void Steer(float direction)
    {
        _targetTurnRate = direction * _shipData.TurnRate;
    }

    private void FixedUpdate()
    {
        MoveShip();
        ApplySteering();
        ApplyHeel();
    }

    private void MoveShip()
    {
        float activeAcceleration = _shipData.Acceleration;

        if (_targetSpeed < _currentSpeed || (_targetSpeed > 0 && _currentSpeed < 0))
        {
            activeAcceleration *= 0.33f;
        }

        _currentSpeed = Mathf.MoveTowards(_currentSpeed, _targetSpeed, activeAcceleration * Time.fixedDeltaTime);
        Vector3 movement = transform.forward * (_currentSpeed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(_rigidbody.position + movement);
    }

    private void ApplySteering()
    {
        _currentTurnRate = Mathf.MoveTowards(_currentTurnRate, _targetTurnRate, _shipData.RudderSpeed * Time.fixedDeltaTime);

        float speedFactor = _shipData.MaxSpeed > 0f ? Mathf.Abs(_currentSpeed / _shipData.MaxSpeed) : 0f;

        float turnAmount = _currentTurnRate * speedFactor * Time.fixedDeltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
        _rigidbody.MoveRotation(_rigidbody.rotation * turnRotation);
    }

    private void ApplyHeel()
    {
        if (_visualModel == null) return;

        float turnRatio = _shipData.TurnRate > 0f ? _currentTurnRate / _shipData.TurnRate : 0f;
        float speedRatio = _shipData.MaxSpeed > 0f ? _currentSpeed / _shipData.MaxSpeed : 0f;

        float targetHeel = -turnRatio * speedRatio * _shipData.MaxHeelAngle;

        _currentHeelAngle = Mathf.MoveTowardsAngle(_currentHeelAngle, targetHeel, _shipData.HeelSpeed * Time.fixedDeltaTime);

        _visualModel.localRotation = Quaternion.Euler(0f, 0f, _currentHeelAngle);
    }
}