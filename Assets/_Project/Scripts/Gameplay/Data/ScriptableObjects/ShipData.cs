using UnityEngine;

[CreateAssetMenu(fileName = "NewShipData", menuName = "Naval/Ship Data")]
public class ShipData : ScriptableObject
{
    [Header("General Information")]
    [Tooltip("The display name of the ship (e.g., Hiryu).")]
    [SerializeField] private string _shipName;

    [Header("Movement Settings")]
    [Tooltip("Maximum forward speed of the ship.")]
    [SerializeField] private float _maxSpeed;

    [Tooltip("How fast the ship accelerates to reach the target speed. Lower values simulate heavier ships.")]
    [SerializeField] private float _acceleration;

    [Header("Steering Settings")]
    [Tooltip("Maximum turning speed of the ship hull (degrees per second).")]
    [SerializeField] private float _turnRate;

    [Tooltip("How quickly the rudder reaches its maximum angle. Lower values mean higher turning inertia.")]
    [SerializeField] private float _rudderSpeed;

    [Header("Visual Physics (Heel)")]
    [Tooltip("Maximum roll angle in degrees during a hard turn. Use 2-4 for Carriers, 10-15 for PT boats.")]
    [SerializeField] private float _maxHeelAngle;

    [Tooltip("How fast the ship leans into the turn. Heavy ships should have lower values.")]
    [SerializeField] private float _heelSpeed;

    [Header("Aviation Settings")]
    [Tooltip("Maximum number of aircraft this carrier can hold and deploy.")]
    [SerializeField] private int _maxAircraftCapacity;

    public string ShipName => _shipName;
    public float MaxSpeed => _maxSpeed;
    public float Acceleration => _acceleration;
    public float TurnRate => _turnRate;
    public float RudderSpeed => _rudderSpeed;
    public float MaxHeelAngle => _maxHeelAngle;
    public float HeelSpeed => _heelSpeed;
    public int MaxAircraftCapacity => _maxAircraftCapacity;
}