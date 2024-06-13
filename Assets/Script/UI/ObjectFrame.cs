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

    Type type;

    public int life = 50;
    public string Category;

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
