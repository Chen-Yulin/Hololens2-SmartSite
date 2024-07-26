using Microsoft.MixedReality.GraphicsTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCollisionDetection : MonoBehaviour
{
    public Material AlertMat;
    public Material NormalMat;

    public TaskManager TM;

    public int[] colliderCnt = new int[10];

    public bool _valid = true;
    public bool Valid
    {
        get { return _valid; }
        set
        {
            if (_valid != value)
            {
                _valid = value;
                SetMat(transform, _valid ? NormalMat : AlertMat);
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
    public void GetCollision(int index)
    {
        colliderCnt[index]++;
        Valid = !JudgeCollision();
    }

    public void CollisionLeave(int index)
    {
        colliderCnt[index]--;
        Valid = !JudgeCollision();
    }
    public bool JudgeCollision()
    {
        foreach(var cnt in colliderCnt)
        {
            if (cnt != 0)
            {
                return true;
            }
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
