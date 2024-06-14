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

    // for aim type
    ObjectFrame source;
    [SerializeField] Material aimMat;

    // for detect type;
    ObjectFrame dist;
    [SerializeField] Material objectMat;

    public void InitFrame(Type t, Vector3 pos, Vector3 rot, Vector3 scale)
    {
        type = t;
        transform.position = pos;
        transform.eulerAngles = rot;
        transform.localScale = scale;
    }


    public void UpdateFrame(string cat, Vector3 pos, Vector3 rot, Vector3 scale)
    {
        Category = cat;
        transform.position = pos;
        transform.eulerAngles = rot;
        transform.localScale = scale;
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
        clone.GetComponent<ObjectManipulator>().ManipulationType = 0x0;
        if (manager.objects.ContainsKey(clone.name))
        {
            manager.objects[clone.name] = clone.GetComponent<ObjectFrame>();
        }
        manager.objects[clone.name].dist = this;
        type = Type.Aim;
        source = manager.objects[name];
        GetComponent<MeshRenderer>().material = aimMat;
    }

    void Start()
    {
        arrows = transform.Find("Arrow");
    }

    // Update is called once per frame
    void Update()
    {
        arrows.localScale = new Vector3(1f/transform.lossyScale.x, 1f/transform.localScale.y, 1f/transform.lossyScale.z) * 0.5f * transform.localScale.magnitude;
    }
}
