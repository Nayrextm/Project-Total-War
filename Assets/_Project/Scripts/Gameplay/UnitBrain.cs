using UnityEngine;

[RequireComponent(typeof(ShipController))]
public class UnitBrain : MonoBehaviour
{
    [SerializeField] private Transform _cameraPivot;
    public Transform CameraPivot => _cameraPivot;

    private ShipController _shipController;
    private IShipInputService _currentInput;

    private void Awake()
    {
        _shipController = GetComponent<ShipController>();

        if (TryGetComponent<Gameplay.Weapons.FireControlSystem>(out var fcs))
        {
            fcs.Initialize(_shipController.Data);
        }
    }

    public void Possess(IShipInputService newInputProvider)
    {
        if (_currentInput != null)
        {
            _currentInput.OnThrottleStateChanged -= HandleGearShift;
        }

        _currentInput = newInputProvider;

        if (_currentInput != null)
        {
            _currentInput.OnThrottleStateChanged += HandleGearShift;
        }
        else
        {
            _shipController.SetThrottleInput(0);
        }
    }

    private void HandleGearShift(int step)
    {
        _shipController.SetThrottleInput(step);
    }

    private void Update()
    {
        if (_currentInput == null) return;
        _shipController.Steer(_currentInput.GetSteering());
    }

    private void OnDestroy()
    {
        if (_currentInput != null)
        {
            _currentInput.OnThrottleStateChanged -= HandleGearShift;
        }
    }
}