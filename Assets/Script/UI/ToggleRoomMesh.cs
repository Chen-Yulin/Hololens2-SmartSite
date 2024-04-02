using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleRoomMesh : MonoBehaviour
{
    public IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem;
    public bool spatialMeshEnabled = false;

    void Start()
    {
        // 获取Spatial Awareness系统
        spatialAwarenessSystem = CoreServices.SpatialAwarenessSystem;

        if (spatialAwarenessSystem == null)
        {
            Debug.LogError("Spatial Awareness System is not available.");
        }
        else
        {
            spatialAwarenessSystem.SuspendObservers();
            spatialMeshEnabled = false;
        }
    }

    // 当按钮被按下时调用
    public void OnButtonPress()
    {
        if (spatialAwarenessSystem != null)
        {
            if (spatialMeshEnabled)
            {
                // 关闭环境网格的显示
                spatialAwarenessSystem.ClearObservations();
                spatialAwarenessSystem.SuspendObservers();
                spatialMeshEnabled = false;
            }
            else
            {
                // 启用环境网格的显示
                spatialAwarenessSystem.ResumeObservers();
                spatialMeshEnabled = true;
            }
        }
    }
}
