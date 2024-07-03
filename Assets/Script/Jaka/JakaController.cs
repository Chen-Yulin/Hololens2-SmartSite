using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using jakaApi;
using jkType;
using UnityEngine.Events;
using System;
using UnityEngine.Serialization;

public class JakaController : MonoBehaviour
{
    int handle = 0;
    public delegate void TriggerCallback();

    [Serializable]
    public class JakaCallbackEvent : UnityEvent { }
    [FormerlySerializedAs("Callback")]
    [SerializeField]
    JakaCallbackEvent callback_event = new JakaCallbackEvent();

    public void SetJointRot(float[] rot)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        jakaAPI.create_handler("192.168.2.160",ref handle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
