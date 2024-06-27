using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http.Headers;
using UnityEngine;
using WW2NavalAssembly;

[System.Serializable]
public class KeyPoint
{
    public Vector3 pos = Vector3.zero;
    public bool grab = false;
    public bool wait = false;
    public KeyPoint(Vector3 pos, bool grab = false, bool wait = false)
    {
        this.pos = pos;
        this.grab = grab;
        this.wait = wait;
    }
}
[System.Serializable]
public class ArmAction
{
    public float[] angles = new float[6];
    public bool grab = false;
    public bool wait = false;
    public ArmAction(float[] a, bool grab = false, bool wait = false)
    {
        angles = a;
        this.grab = grab;
        this.wait = wait;
    }
}

[System.Serializable]
public class Route
{
    public List<KeyPoint> keypoints = new List<KeyPoint>();
    public Route(List<KeyPoint> turningPoints)
    {
        if (turningPoints.Count < 2)
        {
            return;
        }
        for (int i = 1; i < turningPoints.Count; i++)
        {
            
            KeyPoint start = turningPoints[i-1];
            KeyPoint end = turningPoints[i];
            Debug.Log(start.pos.ToString() + ' ' + end.pos.ToString());
            float dist = (start.pos - end.pos).magnitude;
            int sigment = (int)(dist * 20f);
            for (int j = 0; j <= sigment; j++)
            {
                keypoints.Add(new KeyPoint(start.pos + ((float)j) / sigment * (end.pos - start.pos), start.grab));
            }
        }
    }
}

public class RouteGenerator : MonoBehaviour
{

    public IKController IK;

    public Route route;
    public List<ArmAction> actionSequence = new List<ArmAction>();

    public GameObject GhostPrefab;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetRoute(List<KeyPoint> turningPoints)
    {
        route = new Route(turningPoints);
    }

    public bool CalculateSequence()
    {
        actionSequence.Clear();
        IK.ResetAngle();
        foreach (var kpt in route.keypoints)
        {
            Debug.Log("Calculate action for " + kpt.pos.ToString());
            bool success = IK.InverseKinematics(IK.GetPositionForJ4(kpt.pos));
            if (!success)
            {
                actionSequence.Clear();
                IK.ResetAngle();
                return false;
            }
            float[] res = new float[IK.Angles.Length];
            Array.Copy(IK.Angles, res, IK.Angles.Length);
            actionSequence.Add(new ArmAction(res, kpt.grab, kpt.wait));
        }
        return true;
    }

    public void GenerateGhost()
    {
        
        try
        {
            Destroy(transform.Find("GhostSpace").gameObject);
        }
        catch { }
        GameObject space = new GameObject("GhostSpace");
        space.transform.parent = transform;
        space.transform.localPosition = Vector3.zero;
        space.transform.localRotation = Quaternion.identity;
        space.transform.localScale = Vector3.one;
        foreach (var action in actionSequence)
        {
            GameObject ghost = Instantiate(GhostPrefab);
            ghost.transform.parent = space.transform;
            ghost.transform.localPosition = Vector3.zero;
            ghost.transform.localRotation = Quaternion.identity;
            ArmDTController adc = ghost.GetComponent<ArmDTController>();
            for (int i = 0; i < action.angles.Length; i++)
            {
                adc.Rotate[i] = MathTool.DT_to_IK_angle(action.angles[i], i);
            }
            ghost.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
