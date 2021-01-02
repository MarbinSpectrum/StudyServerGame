using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;


public class ClientMoveObject : MonoBehaviour
{
    public static ClientMoveObject instance;

    public InputField iPInput;
    public InputField portInput;
    public InputField nickNameInput;
    public string clientName;
    public int clientid;

    bool socketReady;
    TcpClient socket;
    NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;

    private void Awake() => instance = this;

    private void Update()
    {
        //소켓이 준비됫고 데이터를 읽을수잇다면
        if (socketReady && stream.DataAvailable)
        {
            string data = reader.ReadLine();
            Debug.Log(data);
            if (data != null)
                OnIncomingData(data);
        }
    }


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
            Chat.instance.ShowMessage($"소켓에러 : {e.Message}");
        }
    }

    void OnIncomingData(string data)
    {
        if (data.Contains("Init"))
        {
            clientid = int.Parse(data.Split('|')[1]);
            nickNameInput.text = nickNameInput.text.Trim();
            clientName = nickNameInput.text == "" ? "Guset" + UnityEngine.Random.Range(1000, 10000) : nickNameInput.text;
            CreateCharacter(clientid, clientName, 0, 0);
            Invoke("SetCharacter", 1);
            return;
        }
        else if (data.Contains("CreateCharacter"))
        {
            int tempID = int.Parse(data.Split('|')[2]);
            if (!MoveObjectList.instance.CheckPlayer(tempID))
            {
                float x = float.Parse(data.Split('|')[3]);
                float y = float.Parse(data.Split('|')[4]);
                CreateCharacter(tempID, data.Split('|')[1], x, y);
            }
            return;
        }
        else if (data.Contains("SetCharacter"))
        {
            MoveObject temp = MoveObjectList.instance.GetObject(clientid);
            if(temp != null)
                Send($"CreateCharacter|{temp.nickName}|{temp.id}|{temp.transform.position.x}| {temp.transform.position.y}");
            return;
        }
        else if (data.Contains("Delete"))
        {
            int deleteid = int.Parse(data.Split('|')[1]);
            MoveObjectList.instance.DeleteOject(deleteid);
            return;
        }
        else if (data.Contains("PlayerPos"))
        {
            int playerId = int.Parse(data.Split('|')[1]);
            float x = float.Parse(data.Split('|')[2]);
            float y = float.Parse(data.Split('|')[3]);
            if(playerId != clientid)
                MoveObjectList.instance.SetOjectPos(playerId, x, y);
            return;
        }
    }

    public void CreateCharacter(int Cid, string Cname, float ax, float ay)
    {
        MoveObject temp = Instantiate(MoveObjectList.instance.playerObject, new Vector3(0, 0, 0), Quaternion.identity);
        temp.id = Cid;
        temp.nickName = Cname;
        temp.transform.position = new Vector3(ax, ay, temp.transform.position.z);
    }

    public void SetCharacter()
    {
        Send(clientid + "SetCharacter");
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
        Send($"Delete|{clientid}");
        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }
}
