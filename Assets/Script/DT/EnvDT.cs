using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvDT : MonoBehaviour
{
    bool Initialized = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Initialized)
        {
            try
            {
                Transform dt = GameObject.Find("MixedRealityPlayspace").transform.Find("Spatial Awareness System");
                dt.SetParent(transform);
                Initialized = true;
            }
            catch { }
            
        }
    }
}
