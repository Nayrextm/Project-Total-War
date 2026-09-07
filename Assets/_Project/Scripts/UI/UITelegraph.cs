using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UITelegraph : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform _telegraphHandle;
    [SerializeField] private TextMeshProUGUI _speedText;

    [Header("Animation Settings")]
    [SerializeField] private float _rotationDuration = 0.4f;

    [Header("Telegraph Angles (Z Rotation)")]
    [Tooltip("Position ~11:00")]
    [SerializeField] private float _angleFullAhead = 30f;
    [SerializeField] private float _angleHalfAhead = 50f;
    [SerializeField] private float _angleSlowAhead = 70f;
    [Tooltip("Position ~9:00 (Center)")]
    [SerializeField] private float _angleStop = 90f;
    [SerializeField] private float _angleSlowAstern = 110f;
    [SerializeField] private float _angleHalfAstern = 130f;
    [Tooltip("Position ~7:00")]
    [SerializeField] private float _angleFullAstern = 150f;

    [Header("UI Update Settings")]
    [SerializeField] private float _uiUpdateInterval = 0.1f;

    private ShipController _activeShip;
    private IPlayerFleetState _fleetState;

    private float _updateTimer = 0f;
    private float _lastDisplayedKnots = -1f;

    private void Start()
    {
        _fleetState = ServiceLocator.Get<IPlayerFleetState>();

        _fleetState.OnActiveShipChanged += HandleActiveShipChanged;

        if (_fleetState.ActiveShip != null)
        {
            HandleActiveShipChanged(_fleetState.ActiveShip);
        }
    }

    private void HandleActiveShipChanged(ShipController newShip)
    {
        if (_activeShip != null)
        {
            _activeShip.OnGearChanged -= UpdateTelegraphHandle;
        }

        _activeShip = newShip;

        if (_activeShip != null)
        {
            _activeShip.OnGearChanged += UpdateTelegraphHandle;
            UpdateTelegraphHandle(_activeShip.CurrentGear);
            _lastDisplayedKnots = -1f;
        }
    }

    private void Update()
    {
        if (_activeShip == null) return;

        _updateTimer += Time.deltaTime;
        if (_updateTimer < _uiUpdateInterval) return;
        _updateTimer = 0f;

        float knots = Mathf.Abs(_activeShip.CurrentSpeed * 1.94384f);
        float roundedKnots = Mathf.Round(knots * 10f) / 10f;

        if (Mathf.Abs(_lastDisplayedKnots - roundedKnots) > 0.01f)
        {
            _lastDisplayedKnots = roundedKnots;
            _speedText.SetText("{0:F1} KTS", roundedKnots);
        }
    }

    private void OnDestroy()
    {
        if (_fleetState != null)
        {
            _fleetState.OnActiveShipChanged -= HandleActiveShipChanged;
        }
        if (_activeShip != null)
        {
            _activeShip.OnGearChanged -= UpdateTelegraphHandle;
        }
    }

    private void UpdateTelegraphHandle(EngineGear gear)
    {
        float targetAngle = GetAngleForGear(gear);
        _telegraphHandle.DORotate(new Vector3(0, 0, targetAngle), _rotationDuration).SetEase(Ease.OutBack);
    }

    private float GetAngleForGear(EngineGear gear)
    {
        return gear switch
        {
            EngineGear.FullAhead => _angleFullAhead,
            EngineGear.HalfAhead => _angleHalfAhead,
            EngineGear.SlowAhead => _angleSlowAhead,
            EngineGear.Stop => _angleStop,
            EngineGear.SlowAstern => _angleSlowAstern,
            EngineGear.HalfAstern => _angleHalfAstern,
            EngineGear.FullAstern => _angleFullAstern,
            _ => 90f
        };
    }
}