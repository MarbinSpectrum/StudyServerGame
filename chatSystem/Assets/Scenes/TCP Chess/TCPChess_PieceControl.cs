using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCPChess_PieceControl : MonoBehaviour
{
    public static TCPChess_PieceControl instance;

    public Team controlPlayer = Team.White;
    public ChessPiece controlPiece = ChessPiece.Empty;
    public int r, c;
    private int frontR, frontC;
    private int[,] Dic = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };
    private int[,] Dic8 = { { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 } };
    private int[,] KnightDic = { { 2, 1 }, { 1, 2 }, { -2, 1 }, { -1, 2 }, { 2, -1 }, { 1, -2 }, { -2, -1 }, { -1, -2 } };

    public static int GameEnd = 0;

    private void Awake() => instance = this;

    public void NipPice(int ar,int ac,int r,int c)
    {
        frontR = ar;
        frontC = ac;
        TCPChesss_Board.instance.chessMap[r, c] = TCPChesss_Board.instance.chessMap[frontR, frontC];
        TCPChesss_Board.instance.chessMap[r, c].r = r;
        TCPChesss_Board.instance.chessMap[r, c].c = c;
        TCPChesss_Board.instance.chessMap[r, c].transform.position = TCPChesss_Board.instance.pos[r, c];
    }

    public void NipPiece(int ar, int ac)
    {
        if (GameEnd != 0)
        {
            Debug.Log("체크메이크 상태");
            return;
        }

        if(controlPlayer != TCPChess_Client.instance.team)
            return;

        r = ar;
        c = ac;
        TCPChess_Piece temp = TCPChesss_Board.instance.chessMap[r, c];
        GameObject moveCheckObject = TCPChesss_Board.instance.canMovePoint[r, c].gameObject;

        if (controlPiece == ChessPiece.Empty)
        {
            if (temp != null && temp.team == controlPlayer)
            {
                temp.Select(true);
                SelectPiece(r,c, temp);
            }
        }
        else
        {
            if (temp != null)
            {
                if(temp.team == controlPlayer)
                {
                    DisableSelect();
                    temp.Select(true);
                    SelectPiece(r, c, temp);
                }
                else if(!moveCheckObject.activeSelf)
                    DisableSelect();
                else if(MovingIsCheck(frontR, frontC,r,c, controlPlayer))
                    DisableSelect();
                else 
                {
                    Destroy(TCPChesss_Board.instance.chessMap[r, c].gameObject);
                    TCPChesss_Board.instance.chessMap[r, c] = TCPChesss_Board.instance.chessMap[frontR, frontC];
                    TCPChesss_Board.instance.chessMap[r, c].r = r;
                    TCPChesss_Board.instance.chessMap[r, c].c = c;
                    TCPChesss_Board.instance.chessMap[r, c].transform.position = TCPChesss_Board.instance.pos[r, c];
                    TCPChesss_Board.instance.chessMap[frontR, frontC] = null;
                    TCPChess_Client.instance.Send($"Set|{frontR}|{frontC}|{r}|{c}");
                    if (TCPChesss_Board.instance.chessMap[r, c].chessPiece == ChessPiece.Pawn)
                    {
                        TCPChesss_Board.instance.chessMap[r, c].firstMove = false;
                        if (r == 0 || r == 7)
                        {
                            TCPChesss_Board.instance.chessMap[r, c].chessPiece = ChessPiece.Queen;
                            TCPChesss_Board.instance.chessMap[r, c].Init();
                        }
                    }
                    if (controlPlayer == Team.White)
                        controlPlayer = Team.Black;
                    else
                        controlPlayer = Team.White;
                    if (CheckMate(controlPlayer))
                        GameEnd = (int)controlPlayer;
                    DefenceSuccess();
                    DisableSelect();
                    if (IsCheck(TCPChesss_Board.instance.chessMap, controlPlayer))
                        CheckDefence(controlPlayer);
                }
            }
            else
            {
                if (!MovingIsCheck(frontR, frontC, r, c, controlPlayer) && moveCheckObject.activeSelf)
                {
                    TCPChesss_Board.instance.chessMap[r, c] = TCPChesss_Board.instance.chessMap[frontR, frontC];
                    TCPChesss_Board.instance.chessMap[r, c].r = r;
                    TCPChesss_Board.instance.chessMap[r, c].c = c;
                    TCPChesss_Board.instance.chessMap[r, c].transform.position = TCPChesss_Board.instance.pos[r, c];
                    TCPChesss_Board.instance.chessMap[frontR, frontC] = null;
                    TCPChess_Client.instance.Send($"Set|{frontR}|{frontC}|{r}|{c}");
                    if (TCPChesss_Board.instance.chessMap[r, c].chessPiece == ChessPiece.Pawn)
                    {
                        TCPChesss_Board.instance.chessMap[r, c].firstMove = false;
                        if (r == 0 || r == 7)
                        {
                            TCPChesss_Board.instance.chessMap[r, c].chessPiece = ChessPiece.Queen;
                            TCPChesss_Board.instance.chessMap[r, c].Init();
                        }
                    }
                    if (controlPlayer == Team.White)
                        controlPlayer = Team.Black;
                    else
                        controlPlayer = Team.White;
                    if (CheckMate(controlPlayer))
                        GameEnd = (int)controlPlayer;
                    DefenceSuccess();
                    DisableSelect();
                    if (IsCheck(TCPChesss_Board.instance.chessMap, controlPlayer))
                        CheckDefence(controlPlayer);
                }
                else
                    DisableSelect();
            }


        }
    }

    public void SelectPiece(int ar,int ac, TCPChess_Piece chessPiece)
    {
        TCPChess_Piece temp = chessPiece;
        controlPiece = temp.chessPiece;
        if (controlPiece == ChessPiece.Bishop)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    int dr = Dic[i, 1] + Dic[j, 1];
                    int dc = Dic[i, 0] + Dic[j, 0];
                    if (dc == 0 || dr == 0)
                        continue;
                    int aar = ar;
                    int aac = ac;
                    while (0 <= aar && aar < 8 && 0 <= aac && aac < 8)
                    {
                        TCPChess_Piece nextPos = TCPChesss_Board.instance.chessMap[aar, aac];
                        GameObject movePoint = TCPChesss_Board.instance.canMovePoint[aar, aac].gameObject;
                        if (aar != r || aac != c)
                        {
                            if (nextPos != null)
                            {
                                if (nextPos.team != controlPlayer && nextPos.chessPiece != ChessPiece.King)
                                    movePoint.SetActive(true);
                                break;
                            }
                            else
                                movePoint.SetActive(true);
                        }

                        aar += dr;
                        aac += dc;
                    }
                }
        }
        else if (controlPiece == ChessPiece.King)
        {
            for (int i = 0; i < 8; i++)
            {
                int dr = Dic8[i, 1];
                int dc = Dic8[i, 0];
    
                int aar = ar + dr;
                int aac = ac + dc;
                if (0 <= aar && aar < 8 && 0 <= aac && aac < 8)
                {
                    TCPChess_Piece nextPos = TCPChesss_Board.instance.chessMap[aar, aac];
                    GameObject movePoint = TCPChesss_Board.instance.canMovePoint[aar, aac].gameObject;
                    if (aar != r || aac != c)
                    {
                        if (nextPos != null)
                        {
                            if (nextPos.team != controlPlayer && nextPos.chessPiece != ChessPiece.King)
                                movePoint.SetActive(true);
                        }
                        else
                            movePoint.SetActive(true);
                    }

                }
            }
        }
        else if (controlPiece == ChessPiece.Queen)
        {
            for (int i = 0; i < 8; i++)
            {
                int dr = Dic8[i, 1];
                int dc = Dic8[i, 0];

                int aar = ar;
                int aac = ac;
                while (0 <= aar && aar < 8 && 0 <= aac && aac < 8)
                {
                    TCPChess_Piece nextPos = TCPChesss_Board.instance.chessMap[aar, aac];
                    GameObject movePoint = TCPChesss_Board.instance.canMovePoint[aar, aac].gameObject;
                    if (aar != r || aac != c)
                    {
                        if (nextPos != null)
                        {
                            if (nextPos.team != controlPlayer && nextPos.chessPiece != ChessPiece.King)
                                movePoint.SetActive(true);
                            break;
                        }
                        else
                            movePoint.SetActive(true);
                    }

                    aar += dr;
                    aac += dc;
                }
            }
        }
        else if (controlPiece == ChessPiece.Rook)
        {
            for (int i = 0; i < 4; i++)
            {
                int dr = Dic[i, 1];
                int dc = Dic[i, 0];

                int aar = ar;
                int aac = ac;
                while (0 <= aar && aar < 8 && 0 <= aac && aac < 8)
                {
                    TCPChess_Piece nextPos = TCPChesss_Board.instance.chessMap[aar, aac];
                    GameObject movePoint = TCPChesss_Board.instance.canMovePoint[aar, aac].gameObject;
                    if (aar != r || aac != c)
                    {
                        if (nextPos != null)
                        {
                            if (nextPos.team != controlPlayer && nextPos.chessPiece != ChessPiece.King)
                                movePoint.SetActive(true);
                            break;
                        }
                        else
                            movePoint.SetActive(true);
                    }

                    aar += dr;
                    aac += dc;
                }
            }
        }
        else if (controlPiece == ChessPiece.Knight)
        {
            for (int i = 0; i < 8; i++)
            {
                int dr = KnightDic[i, 1];
                int dc = KnightDic[i, 0];

                int aar = ar + dr;
                int aac = ac + dc;
                if (0 <= aar && aar < 8 && 0 <= aac && aac < 8)
                {
                    TCPChess_Piece nextPos = TCPChesss_Board.instance.chessMap[aar, aac];
                    GameObject movePoint = TCPChesss_Board.instance.canMovePoint[aar, aac].gameObject;
                    if (aar != r || aac != c)
                    {
                        if (nextPos != null)
                        {
                            if (nextPos.team != controlPlayer && nextPos.chessPiece != ChessPiece.King)
                                movePoint.SetActive(true);
                        }
                        else
                            movePoint.SetActive(true);
                    }
                }
            }
        }
        else if (controlPiece == ChessPiece.Pawn)
        {
            int dr = temp.team == Team.White ? 1 : -1;
            int dc = 0;

            int aar = ar + dr;
            int aac = ac + dc;
            if (0 <= aar && aar < 8 && 0 <= aac && aac < 8)
            {
                TCPChess_Piece nextPos = TCPChesss_Board.instance.chessMap[aar, aac];
                GameObject movePoint = TCPChesss_Board.instance.canMovePoint[aar, aac].gameObject;
                if (aar != r || aac != c)
                    if (nextPos == null)
                        movePoint.SetActive(true);

            }

           
            {
                int aaar = ar + dr;
                int aaac = ac + 1;

                if (0 <= aaar && aaar < 8 && 0 <= aaac && aaac < 8)
                {
                    TCPChess_Piece nextPos = TCPChesss_Board.instance.chessMap[aaar, aaac];
                    GameObject movePoint = TCPChesss_Board.instance.canMovePoint[aaar, aaac].gameObject;
                    if (aaar != r || aaac != c)
                    {
                        if (nextPos != null)
                        {
                            if (nextPos.team != controlPlayer && nextPos.chessPiece != ChessPiece.King)
                                movePoint.SetActive(true);
                        }
                    }
                }
                aaac -= 2;
                if (0 <= aaar && aaar < 8 && 0 <= aaac && aaac < 8)
                {
                    TCPChess_Piece nextPos = TCPChesss_Board.instance.chessMap[aaar, aaac];
                    GameObject movePoint = TCPChesss_Board.instance.canMovePoint[aaar, aaac].gameObject;
                    if (aaar != r || aaac != c)
                    {
                        if (nextPos != null)
                        {
                            if (nextPos.team != controlPlayer && nextPos.chessPiece != ChessPiece.King)
                                movePoint.SetActive(true);
                        }
                    }
                }

            }

            aar += dr;
            aac += dc;
            if (0 <= aar && aar < 8 && 0 <= aac && aac < 8 && temp.firstMove)
            {
                TCPChess_Piece nextPos = TCPChesss_Board.instance.chessMap[aar, aac];
                GameObject movePoint = TCPChesss_Board.instance.canMovePoint[aar, aac].gameObject;
                if (aar != r || aac != c)
                    if (nextPos == null)
                        movePoint.SetActive(true);

            }
        }
        frontR = ar;
        frontC = ac;
    }

    public bool MovingIsCheck(int ar,int ac,int br,int bc, Team team)
    {
        TCPChess_Piece[,] chessMap = new TCPChess_Piece[8, 8];
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                chessMap[r,c] = TCPChesss_Board.instance.chessMap[r, c];
        chessMap[br, bc] = chessMap[ar, ac];
        //chessMap[br, bc].c = chessMap[ar, ac].c;
        //chessMap[br, bc].r = chessMap[ar, ac].r;
        //chessMap[br, bc].chessPiece = chessMap[ar, ac].chessPiece;
        //chessMap[br, bc].firstMove = chessMap[ar, ac].firstMove;
        chessMap[ar, ac] = null;
        return IsCheck(chessMap, team);
    }
    public bool IsCheck(TCPChess_Piece[,] chessMap,Team team)
    {
        int kingR = 0;
        int kingC = 0;
        for(int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                if (chessMap[r, c] != null && chessMap[r, c].team == team && chessMap[r, c].chessPiece == ChessPiece.King)
                {
                    kingR = r;
                    kingC = c;
                    break;
                }

        for(int i = 0; i < 8; i++)
        {
            int ar = kingR + KnightDic[i, 0];
            int ac = kingC + KnightDic[i, 1];
            if (0 <= ar && ar < 8 && 0 <= ac && ac < 8)
                if (chessMap[ar, ac] != null && chessMap[ar, ac].team != team && chessMap[ar, ac].chessPiece == ChessPiece.Knight)
                {
                    Debug.Log("나이트에 의해 체크");
                    return true;
                }
        }

        if(team == Team.Black)
        {
            int ar = kingR - 1;
            int ac = kingC + 1;
            if (0 <= ar && ar < 8 && 0 <= ac && ac < 8)
                if (chessMap[ar, ac] != null && chessMap[ar, ac].team != team && chessMap[ar, ac].chessPiece == ChessPiece.Pawn)
                {
                    Debug.Log("폰에 의해 체크");
                    return true;
                }
            ac -= 2;
            if (0 <= ar && ar < 8 && 0 <= ac && ac < 8)
                if (chessMap[ar, ac] != null && chessMap[ar, ac].team != team && chessMap[ar, ac].chessPiece == ChessPiece.Pawn)
                {
                    Debug.Log("폰에 의해 체크");
                    return true;
                }
        }
        else if (team == Team.White)
        {
            int ar = kingR + 1;
            int ac = kingC + 1;
            if (0 <= ar && ar < 8 && 0 <= ac && ac < 8)
                if (chessMap[ar, ac] != null && chessMap[ar, ac].team != team && chessMap[ar, ac].chessPiece == ChessPiece.Pawn)
                {
                    Debug.Log("폰에 의해 체크");
                    return true;
                }
            ac -= 2;
            if (0 <= ar && ar < 8 && 0 <= ac && ac < 8)
                if (chessMap[ar, ac] != null && chessMap[ar, ac].team != team && chessMap[ar, ac].chessPiece == ChessPiece.Pawn)
                {
                    Debug.Log("폰에 의해 체크");
                    return true;
                }
        }

        for (int i = 0; i < 8; i++)
        {
            int ar = kingR + Dic8[i, 0];
            int ac = kingC + Dic8[i, 1];
            while(0 <= ar && ar < 8 && 0 <= ac && ac < 8)
            {
                if (chessMap[ar, ac] != null && chessMap[ar, ac].team == team)
                    break;

                if (Mathf.Abs(ar - kingR) <= 1 && Mathf.Abs(ac - kingC) < 1)
                    if (chessMap[ar, ac] != null && chessMap[ar, ac].team != team && chessMap[ar, ac].chessPiece == ChessPiece.King)
                    {
                        Debug.Log("킹에 의해 체크");
                        return true;
                    }

                if (i == 0 || i == 2 || i == 4 || i == 6)
                {
                    if (chessMap[ar, ac] != null && chessMap[ar, ac].team != team && chessMap[ar, ac].chessPiece == ChessPiece.Rook)
                    {
                        Debug.Log("룩에 의해 체크");
                        return true;
                    }
                }
                else
                {
                    if (chessMap[ar, ac] != null && chessMap[ar, ac].team != team && chessMap[ar, ac].chessPiece == ChessPiece.Bishop)
                    {
                        Debug.Log("비숍에 의해 체크");
                        return true;
                    }
                }
                if (chessMap[ar, ac] != null && chessMap[ar, ac].team != team && chessMap[ar, ac].chessPiece == ChessPiece.Queen)
                {
                    Debug.Log("퀸에 의해 체크");
                    return true;
                }

                ar += Dic8[i, 0];
                ac += Dic8[i, 1];
            }
        }
        return false;
    }

    public void CheckDefence(Team team)
    {
        int kingR = 0;
        int kingC = 0;
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
            {
                TCPChess_Piece temp = TCPChesss_Board.instance.chessMap[r, c];
                if (temp != null && temp.team == team && temp.chessPiece == ChessPiece.King)
                {
                    kingR = r;
                    kingC = c;
                    break;
                }
            }

        TCPChesss_Board.instance.warningPoint[kingR, kingC].gameObject.SetActive(true);
    }

    public void DisableSelect()
    {
        controlPiece = ChessPiece.Empty;
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                if (TCPChesss_Board.instance.chessMap[r, c] != null)
                    TCPChesss_Board.instance.chessMap[r, c].Select(false);
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                TCPChesss_Board.instance.canMovePoint[r, c].gameObject.SetActive(false);
    }

    public void DefenceSuccess()
    {
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                TCPChesss_Board.instance.warningPoint[r, c].gameObject.SetActive(false);
    }

    public bool CheckMate(Team team)
    {
        TCPChess_Piece[,] chessMap = new TCPChess_Piece[8, 8];
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                chessMap[r, c] = TCPChesss_Board.instance.chessMap[r, c];

        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
            {
                if (chessMap[r, c] != null && chessMap[r, c].team == team)
                {
                    if(chessMap[r, c].chessPiece == ChessPiece.Bishop)
                    {
                        int[] order = { 1, 3, 5, 7 };
                        for(int i = 0; i < 4; i++)
                        {
                            int dr = Dic8[order[i], 0];
                            int dc = Dic8[order[i], 1];
                            int ar = r + dr;
                            int ac = c + dc;
                            while(0 <= ar && ar < 8 && 0 <= ac && ac < 8)
                            {
                                if (chessMap[ar, ac] == null)
                                {
                                    if (!MovingIsCheck(r, c, ar, ac, team))
                                        return false;
                                }
                                else
                                {
                                    if (chessMap[ar, ac].team != team)
                                        if (!MovingIsCheck(r, c, ar, ac, team))
                                            return false;
                                    break;
                                }
                                ar += dr;
                                ac += dc;
                            }

                        }
                    }
                    else if (chessMap[r, c].chessPiece == ChessPiece.Queen)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            int dr = Dic8[i, 0];
                            int dc = Dic8[i, 1];
                            int ar = r + dr;
                            int ac = c + dc;
                            while (0 <= ar && ar < 8 && 0 <= ac && ac < 8)
                            {
                                if (chessMap[ar, ac] == null)
                                {
                                    if (!MovingIsCheck(r, c, ar, ac, team))
                                        return false;
                                }
                                else
                                {
                                    if (chessMap[ar, ac].team != team)
                                        if (!MovingIsCheck(r, c, ar, ac, team))
                                            return false;
                                    break;
                                }
                                ar += dr;
                                ac += dc;
                            }

                        }
                    }
                    else if (chessMap[r, c].chessPiece == ChessPiece.Rook)
                    {
                        int[] order = { 0, 2, 4, 6 };
                        for (int i = 0; i < 4; i++)
                        {
                            int dr = Dic8[order[i], 0];
                            int dc = Dic8[order[i], 1];
                            int ar = r + dr;
                            int ac = c + dc;
                            while (0 <= ar && ar < 8 && 0 <= ac && ac < 8)
                            {
                                if (chessMap[ar, ac] == null)
                                {
                                    if (!MovingIsCheck(r, c, ar, ac, team))
                                        return false;
                                }
                                else
                                {
                                    if (chessMap[ar, ac].team != team)
                                        if (!MovingIsCheck(r, c, ar, ac, team))
                                            return false;
                                    break;
                                }
                                ar += dr;
                                ac += dc;
                            }

                        }
                    }
                    else if (chessMap[r, c].chessPiece == ChessPiece.Knight)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            int ar = r + KnightDic[i, 0];
                            int ac = c + KnightDic[i, 1];
                            if(0 <= ar && ar < 8 && 0 <= ac && ac < 8)
                            {
                                if (chessMap[ar, ac] == null)
                                {
                                    if (!MovingIsCheck(r, c, ar, ac, team))
                                        return false;
                                }
                                else
                                {
                                    if (chessMap[ar, ac].team != team)
                                        if (!MovingIsCheck(r, c, ar, ac, team))
                                            return false;
                                    break;
                                }
                            }
                        }
                    }
                    else if (chessMap[r, c].chessPiece == ChessPiece.King)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            int ar = r + Dic8[i, 0];
                            int ac = c + Dic8[i, 1];
                            if(0 <= ar && ar < 8 && 0 <= ac && ac < 8)
                            {
                                if (chessMap[ar, ac] == null)
                                {
                                    if (!MovingIsCheck(r, c, ar, ac, team))
                                        return false;
                                }
                                else
                                {
                                    if (chessMap[ar, ac].team != team)
                                        if (!MovingIsCheck(r, c, ar, ac, team))
                                            return false;
                                    break;
                                }
                            }

                        }
                    }
                    else if (chessMap[r, c].chessPiece == ChessPiece.Pawn)
                    {
                        int ar = r + team == Team.White ? 1 : -1;
                        int ac = c;

                        if (0 <= ar && ar < 8 && 0 <= ac && ac < 8)
                        {
                            if (chessMap[ar, ac] == null)
                            {
                                if (!MovingIsCheck(r, c, ar, ac, team))
                                    return false;
                            }
                        }

                        ac += 1;

                        if (0 <= ar && ar < 8 && 0 <= ac && ac < 8)
                        {
                            if (chessMap[ar, ac] != null && chessMap[ar, ac].team != team)
                            {
                                if (!MovingIsCheck(r, c, ar, ac, team))
                                    return false;
                            }
                        }

                        ac -= 2;

                        if (0 <= ar && ar < 8 && 0 <= ac && ac < 8)
                        {
                            if (chessMap[ar, ac] != null && chessMap[ar, ac].team != team)
                            {
                                if (!MovingIsCheck(r, c, ar, ac, team))
                                    return false;
                            }
                        }

                        ac += 1;
                        ar += team == Team.White ? 1 : -1;

                        if (0 <= ar && ar < 8 && 0 <= ac && ac < 8)
                        {
                            if (chessMap[ar, ac] == null && chessMap[r, c].firstMove)
                            {
                                if (!MovingIsCheck(r, c, ar, ac, team))
                                    return false;
                            }
                        }
                    }
                }
            }

        return true;
    }
}
