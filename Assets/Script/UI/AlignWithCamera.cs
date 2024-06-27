using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithCamera : MonoBehaviour
{
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 directionToCamera = (-cam.transform.position + transform.position).normalized;

        // 创建一个旋转，使得物体朝向摄像头的方向，同时保持Y轴向上
        Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera, Vector3.up);

        // 应用旋转到物体
        transform.rotation = rotationToCamera;
    }
}
