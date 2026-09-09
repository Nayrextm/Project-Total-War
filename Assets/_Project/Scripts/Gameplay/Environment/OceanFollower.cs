using UnityEngine;

public class OceanFollower : MonoBehaviour
{
    [SerializeField] private Transform _targetCamera;
    [SerializeField] private float _seaLevel = 0f;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void LateUpdate()
    {
        if (_targetCamera == null) return;

        Vector3 newPosition = _targetCamera.position;
        newPosition.y = _seaLevel; 

        _transform.position = newPosition;
    }
}