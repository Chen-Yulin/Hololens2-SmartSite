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

    public GameObject UI;
    public GameObject Frame;

    // for aim type
    ObjectFrame source;
    [SerializeField] Material aimMat;
    [SerializeField] LineRenderer Line;

    // for detect type;
    ObjectFrame dist;
    [SerializeField] Material objectMat;

    public void InitFrame(Type t, Vector3 pos, Vector3 rot, Vector3 scale)
    {
        type = t;
        Frame.transform.position = pos;
        Frame.transform.eulerAngles = rot;
        Frame.transform.localScale = scale;
    }


    public void UpdateFrame(string cat, Vector3 pos, Vector3 rot, Vector3 scale)
    {
        Category = cat;
        Frame.transform.position = pos;
        Frame.transform.eulerAngles = rot;
        Frame.transform.localScale = scale;
    }

    public void SwitchToAim()
    {
        if (type == Type.Aim)
        {
            return;
        }
        GameObject clone = Instantiate(gameObject);
        clone.transform.parent = transform.parent;
        clone.name = name;
        clone.transform.Find("Frame").gameObject.GetComponent<ObjectManipulator>().ManipulationType = 0x0;
        if (manager.objects.ContainsKey(clone.name))
        {
            manager.objects[clone.name] = clone.GetComponent<ObjectFrame>();
        }
        manager.objects[clone.name].dist = this;
        type = Type.Aim;
        source = manager.objects[name];
        transform.Find("Frame").gameObject.GetComponent<MeshRenderer>().material = aimMat;
    }

    void Start()
    {
        Frame = transform.Find("Frame").gameObject;
        arrows = Frame.transform.Find("Arrow");
        UI = transform.Find("UI").gameObject;
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
            UI.SetActive(true);
        }
        else
        {
            UI.SetActive(false);
            Line.enabled = false;
        }

    }
}
