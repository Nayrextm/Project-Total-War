using System;
using UnityEngine;

public interface IWeaponsInputService
{
    event Action OnFireRequested;

    Vector2 GetPointerPosition();
    bool IsFreeLookActive { get; }
}