using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class ServerClient
{
    public TcpClient tcp;
    public string clientName;
    public int id;

    public ServerClient(TcpClient clintSocket)
    {
        clientName = "Guest";
        tcp = clintSocket;
        id = 0;
    }
}
