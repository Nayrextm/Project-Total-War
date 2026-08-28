using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    [SerializeField] private ShipData _shipData;

    private Rigidbody _rigidbody;
    private float _currentSpeed;
    private float _targetSpeed;
    private float _steerInput;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Initialize(ShipData data)
    {
        _shipData = data;
        _currentSpeed = 0f;
        _targetSpeed = 0f;
        _steerInput = 0f;
    }

    public void SetThrottle(float throttlePercentage)
    {
        float clampedThrottle = Mathf.Clamp(throttlePercentage, -0.2f, 1f);
        _targetSpeed = _shipData.MaxSpeed * clampedThrottle;
    }

    public void Steer(float direction)
    {
        _steerInput = direction;
    }

    private void FixedUpdate()
    {
        MoveShip();
        ApplySteering();
    }

    private void MoveShip()
    {
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, _targetSpeed, _shipData.Acceleration * Time.fixedDeltaTime);
        Vector3 movement = transform.forward * (_currentSpeed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(_rigidbody.position + movement);
    }

    private void ApplySteering()
    {
        float turnAmount = _steerInput * _shipData.TurnRate * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
        _rigidbody.MoveRotation(_rigidbody.rotation * turnRotation);
    }
}