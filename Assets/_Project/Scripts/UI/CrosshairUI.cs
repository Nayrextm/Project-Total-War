using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CrosshairUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _crosshairRect;

        [Tooltip("Aiming reticle(crosshair) fade-in/fade-out speed")]
        [SerializeField] private float _fadeSpeed = 10f;

        private CanvasGroup _canvasGroup;
        private IWeaponsInputService _weaponsInput;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _weaponsInput = ServiceLocator.Get<IWeaponsInputService>();
        }

        private void Update()
        {
            if (_weaponsInput == null || _crosshairRect == null) return;

            _crosshairRect.position = _weaponsInput.GetPointerPosition();

            float targetAlpha = _weaponsInput.IsFreeLookActive ? 0f : 1f;

            if (!Mathf.Approximately(_canvasGroup.alpha, targetAlpha))
            {
                _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, targetAlpha, Time.deltaTime * _fadeSpeed);
            }
        }
    }
}