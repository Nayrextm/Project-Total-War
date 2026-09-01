using System;

public interface IShipInputService
{
    event Action<int> OnGearShifted; 
    float GetSteering();
}