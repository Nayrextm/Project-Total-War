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
        _currentInput = newInputProvider;
    }

    private void Update()
    {
        if (_currentInput == null) return;

        _shipController.SetThrottle(_currentInput.GetThrottle());
        _shipController.Steer(_currentInput.GetSteering());
    }
}