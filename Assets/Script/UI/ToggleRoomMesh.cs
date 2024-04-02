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
        // ��ȡSpatial Awarenessϵͳ
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

    // ����ť������ʱ����
    public void OnButtonPress()
    {
        if (spatialAwarenessSystem != null)
        {
            if (spatialMeshEnabled)
            {
                // �رջ����������ʾ
                spatialAwarenessSystem.ClearObservations();
                spatialAwarenessSystem.SuspendObservers();
                spatialMeshEnabled = false;
            }
            else
            {
                // ���û����������ʾ
                spatialAwarenessSystem.ResumeObservers();
                spatialMeshEnabled = true;
            }
        }
    }
}
