using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reversi
{
    public enum ProtocolType
    {
        /// <summary> (대기) 부여될 아이디값 </summary>
        WAIT = 100,
        /// <summary> (시작) 시작하는 플레이어의 아이디값 </summary>
        START,
        /// <summary> (장소선택) 클라이언트 아이디,행,열 </summary>
        SELPLACE,
        TURN,    
        /// <summary> (턴종료) 다음에 게임을 진행할 클라이언트 아이디 </summary>
        END      
    }

    public class ReversiHelper
    {

    }
}

