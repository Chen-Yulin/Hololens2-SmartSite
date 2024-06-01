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
            // ����һ��IP��ַ�Ͷ˿�
            IPAddress broadcastAddress = IPAddress.Parse("255.255.255.255");
            IPEndPoint broadcastEndPoint = new IPEndPoint(broadcastAddress, port);

            // ����һ��UDP�ͻ���
            using (UdpClient udpClient = new UdpClient())
            {
                // ���ù㲥
                udpClient.EnableBroadcast = true;

                // ������Ϣ
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
