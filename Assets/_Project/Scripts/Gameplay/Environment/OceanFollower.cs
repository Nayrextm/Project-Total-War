using UnityEngine;

public class OceanFollower : MonoBehaviour
{
    [SerializeField] private Transform _targetCamera;

    [SerializeField] private float _seaLevel = 0f;

    private void FixedUpdate() 
    {
        if (_targetCamera == null) return; 

        Vector3 newPosition = new Vector3(_targetCamera.position.x, _seaLevel, _targetCamera.position.z);
        transform.position = newPosition;
    }
}