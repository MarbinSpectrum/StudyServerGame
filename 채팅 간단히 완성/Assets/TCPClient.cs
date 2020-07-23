using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.Net.Sockets;

public class TCPClient : MonoBehaviour
{
    private Socket m_client;

    /// <summary> 서버로 부터 받은 고유아이디값 </summary>
    private int m_myID;

    /// <summary> 자신의 턴인지를 나타내는 플래그 </summary>
    private bool m_Turn = false;

    private string m_sendMsg = string.Empty;
    private string m_recvMsg = string.Empty;

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[Start]
    void Start()
    {

    }
    #endregion

    #region[Update]
    void Update()
    {
        Parser();
        GameInput();
    }
    #endregion

    #region[OnGUI]
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "Connect"))
            Connect("127.0.0.1", 80);
    }
    #endregion

    #region[OnApplicationQuit]
    private void OnApplicationQuit()
    {
        if (m_client != null)
        {
            m_client.Shutdown(SocketShutdown.Both);
            m_client.Close();
            m_client = null;
        }
    }
    #endregion

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    #region[Connect]
    private bool Connect(string ipAddress, int port )
    {
        Debug.Log("접속시도");

        try
        {
            m_client = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            // 서버에 접속 요청을 합니다.
            m_client.Connect(ipAddress, port);

            Debug.Log("접속성공");
            return true;

        }
        catch ( System.Exception exception )
        {
            m_client = null;
            Debug.Log(exception);
        }
        return false;
    }
    #endregion

    #region[SendMsg]
    public void SendMsg( string message )
    {
        //메세지가 존재할경우
        if (!message.Equals(string.Empty))
        {
            byte[] msg = System.Text.Encoding.UTF8.GetBytes(message);
            m_client.Send(msg);
        }
    }
    #endregion

    #region[GameInput]
    void GameInput()
    {
        if (!m_Turn)
            return;
    }
    #endregion

    #region[Parser]
    void Parser()
    {
        //클라이언트가 존재하면
        if (m_client != null && m_client.Poll(0, SelectMode.SelectRead))
        {
            Debug.Log("접속중");
            //서버로 부터 데이터를 받아서 문자열로 변경
            byte[] buffer = new byte[1024];
            int recvLen = m_client.Receive(buffer);
            string text = System.Text.Encoding.UTF8.GetString(buffer);
            //기본적인 패킷이 ',' 단위로 구별되어있어서 데이터를 분리한다.
            string[] arr = text.Split(',');

            Reversi.ProtocolType protocol;
            Enum.TryParse<Reversi.ProtocolType>(arr[0], out protocol);
            int id;
            switch (protocol)
            {
                case Reversi.ProtocolType.WAIT:
                    int.TryParse(arr[1], out m_myID);
                    break;
                case Reversi.ProtocolType.START:
                    int startId;
                    if (int.TryParse(arr[1], out startId))
                    {
                        //서버로받은 ID값이 자신의 ID값이면 먼저 게임을 플레이하도록 처리 
                        if (m_myID == startId)
                        {
                            m_Turn = true;
                            //게임을 시작하기 위한 준비단계를 거치도록함
                        }
                        else
                        {
                            m_Turn = false;
                        }
                    }
                    break;
                case Reversi.ProtocolType.SELPLACE:
                    if (int.TryParse(arr[1], out id))
                    {
                        //행과 열값을 얻어줌
                        int row, col;
                        int.TryParse(arr[2], out row);
                        int.TryParse(arr[3], out col);
                    }
                    break;
                case Reversi.ProtocolType.TURN:
                    break;
                case Reversi.ProtocolType.END:
                    if (int.TryParse(arr[1], out id))
                    {
                        if (id == m_myID)
                            Debug.Log("승리");
                    }
                    break;
            }

            if (recvLen > 0)
            {
                ChatUI chatUI = GameObject.FindObjectOfType<ChatUI>();
                chatUI.AddChat(text);
            }
        }
    }
    #endregion
}
