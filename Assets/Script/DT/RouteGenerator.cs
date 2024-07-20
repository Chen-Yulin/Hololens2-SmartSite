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
    public Vector3 right = default;
    public KeyPoint(Vector3 pos, Vector3 right = default, bool grab = false, bool wait = false)
    {
        this.pos = pos;
        this.right = right;
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
    public Vector3 position = Vector3.zero;
    public ArmAction(float[] a, bool grab = false, bool wait = false, Vector3 position = default)
    {
        angles = a;
        this.grab = grab;
        this.wait = wait;
        this.position = position;
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
                keypoints.Add(new KeyPoint(start.pos + ((float)j) / sigment * (end.pos - start.pos), start.right, start.grab));
            }
        }
    }
}

public class RouteGenerator : MonoBehaviour
{

    public IKController IK;

    public Route route;
    public List<ArmAction> actionSequence = new List<ArmAction>();

    public Transform GhostSpace;

    public GameObject GhostPrefab;

    public Material transparentMat;
    public Material hightlightMat;

    [SerializeField]
    uint currHighlight = 0;


    
    public uint CurrHighlight
    {
        get
        {
            return currHighlight;
        }
        set
        {
            if (currHighlight != value)
            {
                currHighlight = value;
                if (!GhostSpace)
                {
                    return;
                }
                if (GhostSpace.childCount > 0)
                {
                    if (currHighlight == 0)
                    {
                        SetMat(GhostSpace.GetChild(GhostSpace.childCount - 1), transparentMat);
                    }
                    else
                    {
                        SetMat(GhostSpace.GetChild((int)(currHighlight - 1)), transparentMat);
                    }
                    SetMat(GhostSpace.GetChild((int)currHighlight), hightlightMat);
                }
            }
        }
    }

    IEnumerator IncrementVis()
    {
        while (true)
        {
            //Debug.Log("Increment");
            if (!GhostSpace)
            {
                CurrHighlight = 0;
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                if (CurrHighlight < GhostSpace.childCount - 1)
                {
                    CurrHighlight++;
                }
                else
                {
                    CurrHighlight = 0;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public void SetMat(Transform target, Material mat)
    {
        foreach (Transform child in target)
        {
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                Material[] newMaterials = new Material[meshRenderer.materials.Length];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = mat;
                }
                meshRenderer.materials = newMaterials;
            }
            SetMat(child, mat);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IncrementVis());
    }

    private void OnDestroy()
    {
        StopCoroutine(IncrementVis());
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
            bool success = IK.InverseKinematics(IK.GetPositionForJ4(kpt.pos), kpt.right);
            if (!success)
            {
                actionSequence.Clear();
                IK.ResetAngle();
                return false;
            }
            float[] res = new float[IK.Angles.Length];
            Array.Copy(IK.Angles, res, IK.Angles.Length);
            actionSequence.Add(new ArmAction(res, kpt.grab, kpt.wait, kpt.pos));
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
        CurrHighlight = 0;
        GameObject space = new GameObject("GhostSpace");
        GhostSpace = space.transform;
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
