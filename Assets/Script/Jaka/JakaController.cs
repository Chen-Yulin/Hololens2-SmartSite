using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using jakaApi;
using jkType;
using UnityEngine.Events;
using System;
using UnityEngine.Serialization;
using WW2NavalAssembly;

public class JakaController : MonoBehaviour
{
    int handle = 0;
    public delegate void TriggerCallback();

    [Serializable]
    public class JakaCallbackEvent : UnityEvent { }
    [FormerlySerializedAs("Callback")]
    [SerializeField]
    JakaCallbackEvent callback_event = new JakaCallbackEvent();
    public UdpSocket network;

    public void SetJointRot(float[] rot)
    {
        if (rot.Length != 6)
        {
            throw new ArgumentException("数组长度不等于6");
        }
        string info = "{Target Joint}\n";

        float[] real_rot = MathTool.DT_to_Real_angle(rot);
        foreach (var r in real_rot)
        {
            info += $"{Math.Round(r, 2)}\n";
        }
        network.SendData(info);
    }

    public void SetGripper(bool open)
    {
        string info = "{Gripper}\n";
        info += open ? "0\n" : "1\n";
        //Debug.Log(info);
        network.SendData(info);
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
