using System;

public interface IShipInputService
{
    event Action<int> OnThrottleStateChanged;
    float GetSteering();
}