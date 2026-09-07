using System;

public interface IPlayerFleetState
{
    event Action<ShipController> OnActiveShipChanged;
    ShipController ActiveShip { get; }
}