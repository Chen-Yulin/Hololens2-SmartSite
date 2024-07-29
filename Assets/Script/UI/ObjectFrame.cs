using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFrame : MonoBehaviour
{

    public enum Type
    {
        Detect,
        Aim
    }
    
    Transform arrows;

    public Type type = Type.Detect;

    public int life = 50;
    public string Category;

    public ObjectSpaceManager manager;

    public GameObject UI_Aim;
    public GameObject UI_Detect;
    public GameObject Frame;

    // for aim type
    ObjectFrame source;
    [SerializeField] Material aimMat;
    [SerializeField] LineRenderer Line;
    [SerializeField] TaskManager taskManager;

    // for detect type;
    ObjectFrame dist;
    public bool InTask = false;

    public Transform RightHand;
    public Transform LeftHand;

    public void SwitchTaskMode(bool intask)
    {
        if (type == Type.Aim)
        {
            InTask = intask;
            try
            {
                source.InTask = intask;
            }
            catch { }
        }
        else
        {
            InTask = intask;
            try
            {
                dist.InTask = intask;
            }
            catch { }
        }
    }

    public void SendTask(int task_type) // type 0 for move, 1 for left grab, 2 for right grab
    {
        if (taskManager)
        {
            ArmTask t = new ArmTask();
            if (task_type == 0)
            {
                t.InitAsMoveObject(source.Frame.transform.position, Frame.transform.position, source.Frame.transform.right, Frame.transform.right);
            }else if ((task_type == 1 || task_type == 2) && type == Type.Detect)
            {
                t.InitAsGrabObject(
                    Frame.transform.position, 
                    Frame.transform.right, 
                    task_type == 2 ? RightHand : LeftHand
                    );
            }

            taskManager.GetTask(t, gameObject);
        }
        else
        {
            Debug.Log("TaskManager not found.");
        }
    }

    public void InitFrame(Type t, Vector3 pos, Vector3 rot, Vector3 scale)
    {
        type = t;
        Frame.transform.position = pos;
        Frame.transform.eulerAngles = rot;
        Frame.transform.localScale = scale;
    }


    public void UpdateFrame(string cat, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        if (InTask)
        {
            return;
        }
        Category = cat;
        Frame.transform.localPosition = Vector3.Lerp(Frame.transform.localPosition, pos, 0.3f);
        Frame.transform.localRotation = Quaternion.Lerp(Frame.transform.localRotation, rot, 0.15f);
        Frame.transform.localScale = scale;
    }

    public void CancelAim()
    {
        if (InTask)
        {
            return;
        }
        Debug.Log("Cancel Aim");

        if (type == Type.Aim)
        {
            source.transform.Find("Frame").gameObject.GetComponent<ObjectManipulator>().ManipulationType = Microsoft.MixedReality.Toolkit.Utilities.ManipulationHandFlags.OneHanded;
            source.dist = null;
            Destroy(gameObject);
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

    public void SwitchToAim()
    {
        if (InTask)
        {
            return;
        }
        if (type == Type.Aim)
        {
            return;
        }
        GameObject clone = Instantiate(gameObject);
        clone.transform.parent = transform.parent;
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localRotation = Quaternion.identity;
        clone.transform.localScale = Vector3.one;
        clone.name = name;
        clone.transform.Find("Frame").gameObject.GetComponent<ObjectManipulator>().ManipulationType = 0x0;
        if (manager.objects.ContainsKey(clone.name))
        {
            manager.objects[clone.name] = clone.GetComponent<ObjectFrame>();
        }
        manager.objects[clone.name].dist = this;
        type = Type.Aim;
        source = manager.objects[name];
        SetMat(transform.Find("Frame").Find("box"),aimMat);
    }

    void Start()
    {
        Frame = transform.Find("Frame").gameObject;
        arrows = Frame.transform.Find("Arrow");
        UI_Aim = transform.Find("UI_Aim").gameObject;
        UI_Detect = transform.Find("UI_Detect").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        arrows.localScale = new Vector3(1f/Frame.transform.lossyScale.x, 1f/Frame.transform.localScale.y, 1f/Frame.transform.lossyScale.z) * 0.5f * Frame.transform.localScale.magnitude;


        if (type == Type.Aim)
        {
            Line.enabled = true;
            Line.positionCount = 2;
            Line.SetPosition(1, Frame.transform.position);
            Line.SetPosition(0, source.Frame.transform.position);
            UI_Aim.SetActive(true);
            UI_Detect.SetActive(false);
        }

        else if(type == Type.Detect && !dist)
        {
            UI_Aim.SetActive(false);
            UI_Detect.SetActive(true);
            Line.enabled = false;
        }

    }
}
