using UnityEngine;

public class VisualBuoyancy : MonoBehaviour
{
    [Header("Ship Dimensions")]
    [Tooltip("Відстань від центру до носа/корми для розрахунку кута нахилу")]
    [SerializeField] private float _shipLength = 10f;

    [Header("Wave Mathematics")]
    [SerializeField] private float _waveSpeed = 1f;
    [SerializeField] private float _waveHeight = 0.5f;
    [SerializeField] private float _waveFrequency = 0.2f;

    [Header("Smoothing")]
    [SerializeField] private float _smoothness = 5f;

    private Transform _transform;
    private Vector3 _initialLocalPosition;
    private float _halfLength;
    private float _rollWidth;
    private float _rollDivisor;

    private float _targetY;
    private float _targetPitch;
    private float _targetRoll;

    private void Awake()
    {
        _transform = transform;

        _halfLength = _shipLength * 0.5f;
        _rollWidth = _shipLength * 0.2f;
        _rollDivisor = _shipLength * 0.4f;
    }

    private void Start()
    {
        _initialLocalPosition = _transform.localPosition;
    }

    private void Update()
    {
        Vector3 worldPos = _transform.position;
        float time = Time.time * _waveSpeed;

        float waveY = GetWaveHeight(worldPos.x, worldPos.z, time);

        Vector3 forwardOffset = _transform.forward * _halfLength;
        float bowHeight = GetWaveHeight(worldPos.x + forwardOffset.x, worldPos.z + forwardOffset.z, time);
        float sternHeight = GetWaveHeight(worldPos.x - forwardOffset.x, worldPos.z - forwardOffset.z, time);

        Vector3 rightOffset = _transform.right * _rollWidth;
        float portHeight = GetWaveHeight(worldPos.x - rightOffset.x, worldPos.z - rightOffset.z, time);
        float starboardHeight = GetWaveHeight(worldPos.x + rightOffset.x, worldPos.z + rightOffset.z, time);

        _targetPitch = -(Mathf.Atan2(bowHeight - sternHeight, _shipLength) * Mathf.Rad2Deg);
        _targetRoll = (Mathf.Atan2(portHeight - starboardHeight, _rollDivisor) * Mathf.Rad2Deg) * 0.5f;
        _targetY = waveY;

        ApplyTransform();
    }

    private float GetWaveHeight(float x, float z, float time)
    {
        float wave1 = Mathf.Sin((x * 0.7f + z * 0.3f) * _waveFrequency + time);
        float wave2 = Mathf.Cos((x * -0.2f + z * 0.8f) * _waveFrequency * 1.5f + time * 1.2f);
        return (wave1 + wave2) * 0.5f * _waveHeight;
    }

    private void ApplyTransform()
    {
        float dt = Time.deltaTime * _smoothness;

        Vector3 newPos = _initialLocalPosition;
        newPos.y = Mathf.Lerp(_transform.localPosition.y, _initialLocalPosition.y + _targetY, dt);
        _transform.localPosition = newPos;

        Quaternion targetRotation = Quaternion.Euler(_targetPitch, 0f, _targetRoll);
        _transform.localRotation = Quaternion.Lerp(_transform.localRotation, targetRotation, dt);
    }
}