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

        // ����һ����ת��ʹ�����峯������ͷ�ķ���ͬʱ����Y������
        Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera, Vector3.up);

        // Ӧ����ת������
        transform.rotation = rotationToCamera;
    }
}
