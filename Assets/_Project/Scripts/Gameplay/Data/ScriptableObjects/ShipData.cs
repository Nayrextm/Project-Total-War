using UnityEngine;

[CreateAssetMenu(fileName = "NewShipData", menuName = "Naval/Ship Data")]
public class ShipData : ScriptableObject
{
    [SerializeField] private string _shipName;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _turnRate;
    [SerializeField] private float _acceleration;
    [SerializeField] private int _maxAircraftCapacity;

    public string ShipName => _shipName;
    public float MaxSpeed => _maxSpeed;
    public float TurnRate => _turnRate;
    public float Acceleration => _acceleration;
    public int MaxAircraftCapacity => _maxAircraftCapacity;
}