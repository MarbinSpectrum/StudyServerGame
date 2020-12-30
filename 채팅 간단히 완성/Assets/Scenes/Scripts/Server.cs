using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class Server : MonoBehaviour
{
    public InputField portInput;

    List<ServerClient> clients;
    List<ServerClient> disConnectList;

    TcpListener server;
    bool serverStarted;

    public void ServerCreate()
    {
        clients = new List<ServerClient>();
        disConnectList = new List<ServerClient>();

        try
        {
            int port = portInput.text.Equals(string.Empty) ? 7777 : int.Parse(portInput.text);
            //아이피와 포트에 해당하는 TCP서버 생성
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
            serverStarted = true;
            Chat.instance.ShowMessage($"서버가 {port}에서 시작되었습니다.");
        }
        catch (Exception e)
        {
            Chat.instance.ShowMessage($"Socket Error: {e.Message}");
        }
    }

    //비동기적으로 사용자를 계속받음
    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        StartListening();

        BroadcastMessage("%NAME", new List<ServerClient>() { clients[clients.Count - 1] });
    }

    private void Update()
    {
        if (!serverStarted)
            return;

        foreach (ServerClient c in clients)
        {
            //클라이언트가 연결안되어있다.
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disConnectList.Add(c);
            }
            //클라이언트가 연결되어있다.
            else
            {
                //데이터흐름관리
                NetworkStream s = c.tcp.GetStream();
                //데이터가존재
                if(s.DataAvailable)
                {
                    //StreamReader통해서 데이터를 읽음
                    string data = new StreamReader(s,true).ReadLine();
                    //보낼데이터가 있으면
                    if (data != null)
                        OnIncomingData(c, data);

                }
            }
        }
    }

    void OnIncomingData(ServerClient c,string data)
    {
        if(data.Contains("&Name"))
        {
            c.clientName = data.Split('|')[1];
            Broadcast($"{c.clientName}이 연결되어있습니다", clients);
            return;
        }

        //모든정보를 모든 클라이언트에게 보냄
        Broadcast($"{c.clientName} : {data}", clients);
    }

    void Broadcast(string data,List<ServerClient> cl)
    {
        foreach(var c in cl)
        {
            try
            {
                //StreamWriter통해서 데이터를 씀
                StreamWriter wrtier = new StreamWriter(c.tcp.GetStream());
                //데이터를 작성후
                wrtier.WriteLine(data);
                //내보냄
                wrtier.Flush();
            }
            catch(Exception e)
            {
                Chat.instance.ShowMessage($"쓰기 에러 : {e.Message}를 클라이언트에게 {c.clientName}");
            }
        }
    }

    bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                    //1바이트짜리 신호를 보내서 제대로 연결되었으면 true 반환하는 로직
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }

    }
}


public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clintSocket)
    {
        clientName = "Guest";
        tcp = clintSocket;
    }
}
