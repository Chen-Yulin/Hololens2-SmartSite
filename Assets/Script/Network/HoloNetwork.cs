using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;


public class UdpBroadcaster
{
    public void BroadcastHelloWorld(int port)
    {
        try
        {
            // 创建一个IP地址和端口
            IPAddress broadcastAddress = IPAddress.Parse("255.255.255.255");
            IPEndPoint broadcastEndPoint = new IPEndPoint(broadcastAddress, port);

            // 创建一个UDP客户端
            using (UdpClient udpClient = new UdpClient())
            {
                // 启用广播
                udpClient.EnableBroadcast = true;

                // 发送消息
                string message = "Hello, world!";
                byte[] data = Encoding.UTF8.GetBytes(message);
                udpClient.Send(data, data.Length, broadcastEndPoint);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.ToString());
        }
    }
}

public class HoloNetwork : MonoBehaviour
{
    UdpBroadcaster broadcaster;
    int port = 12345;
    // Start is called before the first frame update
    void Start()
    {
        broadcaster = new UdpBroadcaster();
    }

    // Update is called once per frame
    public void BroadCast()
    {
        Debug.Log("broadcast");
        broadcaster.BroadcastHelloWorld(port);
    }
}
