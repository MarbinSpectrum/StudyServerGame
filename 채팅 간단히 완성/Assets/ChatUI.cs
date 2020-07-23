using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    public InputField m_inputField;
    private Text m_textPrefab;
    public Scrollbar m_scrollbar;
    public ScrollRect m_scrollRect;

    // Start is called before the first frame update
    void Start()
    {
        m_textPrefab = Resources.Load<Text>("Text");
    }

    // 서버로부터 데이터를 받을 때 호출될 함수
    public void AddChat( string chat )
    {
        Text addText = Instantiate(m_textPrefab);
        addText.transform.SetParent(m_scrollRect.content);
        addText.text = chat;
    }

    // Update is called once per frame
    void Update()
    {
        // 사용자로부터 입력받은 메시지가 존재하고,
        if( ! string.IsNullOrEmpty( m_inputField.text ) )
        {
            // 리턴키를 입력받았다면,
            if( Input.GetKeyDown( KeyCode.Return ) )
            {
                // 클라이언트 소켓이 서버로 데이터를 보내도록 처리합니다.
                TCPClient client = GameObject.FindObjectOfType<TCPClient>();
                client.SendMsg(m_inputField.text);
                
                m_inputField.text = string.Empty;
            }
            else
            {
                // 입력 필드가 활성화 되어 있지 않고, 리턴키가 입력되었다면
                // 입력필드를 활성화 시킵니다.
                if( ! m_inputField.isFocused && Input.GetKeyDown( KeyCode.Return) )
                {
                    m_inputField.ActivateInputField();
                }
            }

        }
        
    }
}
