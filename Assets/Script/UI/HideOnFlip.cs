using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnFlip : MonoBehaviour
{
    GameObject mainCamera;
    GameObject child;

    public float Coeff = -5;

    void Start()
    {
        mainCamera = Camera.main.gameObject;
        child = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float size = Vector3.Dot(mainCamera.transform.forward, transform.forward);
        size *= Coeff;
        size = Mathf.Clamp(size, 0.1f, 1);
        if (size == 0.1f)
        {
            child.SetActive(false);
        }
        else
        {
            child.SetActive(true);
        }
        transform.localScale = size * Vector3.one;
    }
}
