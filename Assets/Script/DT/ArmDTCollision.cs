using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ArmDTCollision : MonoBehaviour
{

    public int index = 0;
    public GhostCollisionDetection GCD;
    // Start is called before the first frame update
    void Start()
    {
        GCD = gameObject.GetComponentInParent<GhostCollisionDetection>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        GCD.GetCollision(index);
    }

    public void OnTriggerExit(Collider other)
    {
        GCD.CollisionLeave(index);
    }

    public void OnCollisionEnter(Collision collision)
    {
        
    }
    public void OnCollisionExit(Collision collision)
    {
        
    }
}
