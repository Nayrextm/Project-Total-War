using System;

public interface IFleetInputService
{
    event Action OnNextUnit;
    event Action OnPreviousUnit;
}