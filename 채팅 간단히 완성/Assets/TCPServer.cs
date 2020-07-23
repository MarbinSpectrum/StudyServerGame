using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class TCPServer : MonoBehaviour
{
    // 서버 소켓입니다.
    private Socket m_server;

    // 클라이언트 리스트
    private List<Socket> m_clients = new List<Socket>();

    void CreateServer()
    {
        try
        {
            // 네자리 주소체계를 사용, 연결되어 있는 형태 , 프로토콜 타입은 Tcp
            m_server = new Socket(AddressFamily.InterNetwork,
                                    SocketType.Stream,
                                    ProtocolType.Tcp);

            // IPAddress.Any 속성 : pc에 연결된 주소중에 하나의 주소값을 사용하는 옵션입니다.
            // Bind함수는 소켓에 아이피 주소와 포트 번호를 연결하는 함수입니다.
            m_server.Bind(new IPEndPoint(IPAddress.Any, 80));

            // 한번에 접속할 수 있는 클라이언트의 수는 한명
            m_server.Listen(1);
        }
        catch(Exception exception )
        {
            Debug.Log(exception.Message);
            m_server = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateServer();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_server == null) return;

        // Poll 함수는 메시지가 있는지 체크하는 함수입니다.
        // 첫번째 매개변수는 대기시간,
        // 두번째 매개변수는 처리할 모드
        // 아래의 코드는 서버 소켓에 읽어들일 값이 있는지 체크하는 코드입니다.
        // 클라이언트 요청이 있다면 true값을 리턴하는 함수입니다.
        if( m_server.Poll(0, SelectMode.SelectRead ))
        {
            // 접속을 요청한 클라이언트 소켓의 정보를 갖고 있는 복사본 소켓을 생성합니다.
            Socket client = m_server.Accept();
            m_clients.Add(client);
        }

        for( int i = 0; i< m_clients.Count; ++ i )
        {
            if( m_clients[i].Poll(0, SelectMode.SelectRead ) )
            {
                byte[] buffer = new byte[1024];

                try
                {
                    // 클라이언트가 보낸 데이터를 받습니다.
                    int recvLength = m_clients[i].Receive(buffer);
                    // 데이터의 길이가 0이라면 클라이언트가 종료되었다는 의미를 갖습니다.
                    if (recvLength == 0)
                        continue;
                    else
                    {
                        // 서버에 접속되어 있는 모든 클라이언트에게 메시지를 전달합니다.
                        for( int j = 0; j< m_clients.Count; ++ j )
                        {
                            m_clients[j].Send(buffer);
                        }
                    }
                }
                catch( System.Exception exception )
                {
                    Debug.Log(exception);
                }
            }
        }
        
    }

    // 프로그램이 종료될 때 한 번 호출되는 함수입니다.
    private void OnApplicationQuit()
    {
        for( int i = 0; i< m_clients.Count; ++ i )
        {
            if( m_clients[i] != null )
            {
                // 클라이언트 소켓의 읽고, 쓰기기능을 모두 종료합니다.
                m_clients[i].Shutdown(SocketShutdown.Both);
                // 소켓 자원을 해제합니다.
                m_clients[i].Close();
            }
        }
        m_clients.Clear();

        if( m_server != null )
        {
            m_server.Close();
            m_server = null;
        }
        
    }
}
