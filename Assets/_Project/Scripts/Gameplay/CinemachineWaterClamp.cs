using UnityEngine;
using Unity.Cinemachine;


[ExecuteAlways]
[AddComponentMenu("Cinemachine/Water Clamp")]
public class CinemachineWaterClamp : CinemachineExtension
{
    [Tooltip("Мінімальна висота камери над рівнем води")]
    [SerializeField] private float _minHeightAboveWater = 1.5f;
    
    [Tooltip("Глобальний рівень моря (зазвичай 0)")]
    [SerializeField] private float _seaLevel = 0f;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        
        if (stage == CinemachineCore.Stage.Finalize)
        {
            Vector3 pos = state.RawPosition;

            
            float limit = _seaLevel + _minHeightAboveWater;
            if (pos.y < limit)
            {
                pos.y = limit;
                state.RawPosition = pos;
            }
        }
    }
}