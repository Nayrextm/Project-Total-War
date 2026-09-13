using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class Bootstrapper : MonoBehaviour
{
    private PlayerInputService _playerInputService;

    private void Awake()
    {
        InitializeServices();
    }

    private void InitializeServices()
    {
        _playerInputService = new PlayerInputService();

        ServiceLocator.Register<IShipInputService>(_playerInputService);
        ServiceLocator.Register<IFleetInputService>(_playerInputService);
        ServiceLocator.Register<IWeaponsInputService>(_playerInputService);
    }

    private void OnDestroy()
    {
        _playerInputService?.Disable();

        ServiceLocator.Unregister<IShipInputService>();
        ServiceLocator.Unregister<IFleetInputService>();
        ServiceLocator.Unregister<IWeaponsInputService>();
    }
}