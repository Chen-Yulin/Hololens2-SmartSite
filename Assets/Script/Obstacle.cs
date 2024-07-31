using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public RouteGenerator routeGenerator;
    public void hoverEnter(ManipulationEventData eventData)
    {
        //Debug.Log("Hover Enter");
        routeGenerator.ResetRouteValid();
        Destroy(GetComponent<Rigidbody>());
    }

    public void hoverExit(ManipulationEventData eventData)
    {
        //Debug.Log("Hover Exit");
        if (!GetComponent<Rigidbody>())
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
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
