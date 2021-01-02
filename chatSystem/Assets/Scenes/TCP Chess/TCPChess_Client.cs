using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class TCPChess_Client : MonoBehaviour
{
    public static TCPChess_Client instance;
    public void Awake() => instance = this;

    public Team team;

    public InputField iPInput;
    public InputField portInput;

    public GameObject serverUI;
    public GameObject gameUI;

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

        iPInput.text = iPInput.text.Trim();
        portInput.text = portInput.text.Trim();
        string ip = iPInput.text == "" ? "127.0.0.1" : iPInput.text;
        int port = portInput.text == "" ? 7777 : int.Parse(portInput.text);

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
            Debug.Log($"소켓에러 : {e.Message}");
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
        if (data.Contains("GameStart"))
        {
            StartGame();
            team = Team.Black;
        }
        else if (data.Contains("Set"))
        {
            int ar = int.Parse(data.Split('|')[1]);
            int ac = int.Parse(data.Split('|')[2]);
            int br = int.Parse(data.Split('|')[3]);
            int bc = int.Parse(data.Split('|')[4]);
            TCPChess_PieceControl.instance.NipPice(ar, ac, br, bc);
        }
    }

    public void StartGame()
    {
        gameUI.SetActive(true);
        serverUI.SetActive(false);
    }

    public void Send(string data)
    {
        if (!socketReady)
            return;
        writer.WriteLine(data);
        writer.Flush();
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
