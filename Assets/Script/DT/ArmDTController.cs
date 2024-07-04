using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmDTController : MonoBehaviour
{
    Quaternion[] offset = new Quaternion[6];
    public Vector3[] RotateDir = new Vector3[6];
    public float[] Rotate = new float[6];


    public Transform[] joint = new Transform[6];

    // Start is called before the first frame update

    public void UpdateRealAngle(float[] angles)
    {
        Rotate = angles;
    }

    Transform RecursiveFindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            else
            {
                Transform found = RecursiveFindChild(child, childName);
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }

    void Start()
    {
        for (int i = 0; i < joint.Length; i++)
        {
            joint[i] = RecursiveFindChild(transform, "j"+(i+1).ToString());
            offset[i] = joint[i].localRotation;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0;i < joint.Length;i++)
        {
            joint[i].localRotation = offset[i] * Quaternion.AngleAxis(Rotate[i], RotateDir[i]);
        }
        
    }
}
