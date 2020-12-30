using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class Client : MonoBehaviour
{
    public InputField iPInput;
    public InputField portInput;
    public InputField nickNameInput;
    string clientName;

    bool socketReady;
    TcpClient socket;
    NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;

    public void ConnectedToServer()
    {
        //이미연결되었다면 무시
        if (socketReady)
            return;

        string ip = iPInput.text.Equals(string.Empty) ? "127.0.0.1" : iPInput.text;
        int port = portInput.text.Equals(string.Empty) ? 7777 : int.Parse(portInput.text);

        try
        {
            socket = new TcpClient(ip, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Chat.instance.ShowMessage($"소켓에러 : {e.Message}");
        }
    }

    private void Update()
    {
        //소켓이 준비됫고 데이터를 읽을수잇다면
        if (socketReady && stream.DataAvailable)
        {
            string data = reader.ReadLine();
            if (data != null)
                OnIncomingData(data);
        }
    }

    void OnIncomingData(string data)
    {
        if (data == "%NAME")
        {
            clientName = nickNameInput.Equals(string.Empty) ? "Guset" + UnityEngine.Random.Range(1000, 10000) : nickNameInput.text;
            Send($"NAME|{clientName}");
            return;
        }

        Chat.instance.ShowMessage(data);
    }

    void Send(string data)
    {
        if (!socketReady)
            return;

        writer.WriteLine(data);
        writer.Flush();
    }

    public void OnSendButton(InputField sendInput)
    {
#if(UNITY_EDITOR || UNITY_STANDALONE)
        if (!Input.GetButtonDown("Submit"))
            return;

        //UI 포커스를 sendInput로 조정
        sendInput.ActivateInputField();
#endif
        //쓴게없으면 보내지않음
        if (sendInput.text.Trim().Equals(string.Empty))
            return;

        string message = sendInput.text;
        sendInput.text = "";
        Send(message);
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void CloseSocket()
    {
        if (!socketReady)
            return;
        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }
}
