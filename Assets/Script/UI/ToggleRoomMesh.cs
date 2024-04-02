using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleRoomMesh : MonoBehaviour
{
    public IMixedRealitySpatialAwarenessMeshObserver observer;
    public bool spatialMeshEnabled = false;

    void Start()
    {
        // ��ȡSpatial Awarenessϵͳ
        observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();

        if (observer == null)
        {
            Debug.LogError("Spatial Awareness System is not available.");
        }
        else
        {
            observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.Occlusion;
            spatialMeshEnabled = false;
        }
    }

    // ����ť������ʱ����
    public void OnButtonPress()
    {
        if (observer != null)
        {
            if (spatialMeshEnabled)
            {
                observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.Occlusion;
                spatialMeshEnabled = false;
            }
            else
            {
                observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.Visible;
                spatialMeshEnabled = true;
            }
        }
    }
}
