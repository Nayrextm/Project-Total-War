using UnityEngine;

[RequireComponent(typeof(ShipController))]
public class UnitBrain : MonoBehaviour
{
    private ShipController _shipController;
    private IShipInputService _currentInput;

    private void Awake()
    {
        _shipController = GetComponent<ShipController>();
    }

    private void Start()
    {
        Possess(ServiceLocator.Get<IShipInputService>());
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