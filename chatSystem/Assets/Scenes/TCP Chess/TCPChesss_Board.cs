using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TCPChesss_Board : MonoBehaviour
{
    public static TCPChesss_Board instance;

    public Vector3[,] pos = new Vector3[8, 8];
    public Image[,] canMovePoint = new Image[8, 8];
    public Image[,] warningPoint = new Image[8, 8];
    public Image[,] selectCheck = new Image[8, 8];

    public TCPChess_Piece[,] chessMap = new TCPChess_Piece[8, 8];

    float dis = 1.3f;

    private void Awake() => instance = this;

    void Start()
    {
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
            {
                GameObject temp = Instantiate(TCPChess_Data.instance.chessBoardPiece, new Vector3(c - 4, r - 3.2f, 0) * dis, Quaternion.identity);
                temp.GetComponent<Image>().color = TCPChess_Data.instance.boardColor[(r + c) % 2];
                temp.transform.SetParent(transform);
                temp.transform.name = $"Board|{r}|{c}";
                pos[r, c] = temp.transform.position;
            }

        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
            {
                GameObject temp = Instantiate(TCPChess_Data.instance.chessBoardPiece, new Vector3(c - 4, r - 3.2f, 0) * dis, Quaternion.identity);
                canMovePoint[r, c] = temp.GetComponent<Image>();
                canMovePoint[r, c].color = TCPChess_Data.instance.canMoveColor;
                temp.transform.name = $"CanMove|{r}|{c}";
                temp.transform.SetParent(transform);
                temp.gameObject.SetActive(false);
            }

        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
            {
                GameObject temp = Instantiate(TCPChess_Data.instance.chessBoardPiece, new Vector3(c - 4, r - 3.2f, 0) * dis, Quaternion.identity);
                warningPoint[r, c] = temp.GetComponent<Image>();
                warningPoint[r, c].color = TCPChess_Data.instance.warningColor;
                temp.transform.name = $"Warning|{r}|{c}";
                temp.transform.SetParent(transform);
                temp.gameObject.SetActive(false);
            }

        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
            {
                GameObject temp = Instantiate(TCPChess_Data.instance.selectCheck.gameObject, new Vector3(c - 4, r - 3.2f, 0) * dis, Quaternion.identity);
                temp.transform.name = $"SelectCheck|{r}|{c}";
                temp.transform.SetParent(transform);
                EventTrigger.Entry pClick = new EventTrigger.Entry();
                pClick.eventID = EventTriggerType.PointerClick;
                int ar = r; int ac = c;
                pClick.callback.AddListener((data) =>{ TCPChess_PieceControl.instance.NipPiece(ar,ac); });
                temp.GetComponent<EventTrigger>().triggers.Add(pClick);
            }

        CreatePiece(0, 0, ChessPiece.Rook, Team.White);
        CreatePiece(0, 1, ChessPiece.Knight, Team.White);
        CreatePiece(0, 2, ChessPiece.Bishop, Team.White);
        CreatePiece(0, 3, ChessPiece.Queen, Team.White);
        CreatePiece(0, 4, ChessPiece.King, Team.White);
        CreatePiece(0, 5, ChessPiece.Bishop, Team.White);
        CreatePiece(0, 6, ChessPiece.Knight, Team.White);
        CreatePiece(0, 7, ChessPiece.Rook, Team.White);
        for(int i = 0; i < 8; i++)
            CreatePiece(1, i, ChessPiece.Pawn, Team.White);

        CreatePiece(7, 0, ChessPiece.Rook, Team.Black);
        CreatePiece(7, 1, ChessPiece.Knight, Team.Black);
        CreatePiece(7, 2, ChessPiece.Bishop, Team.Black);
        CreatePiece(7, 3, ChessPiece.Queen, Team.Black);
        CreatePiece(7, 4, ChessPiece.King, Team.Black);
        CreatePiece(7, 5, ChessPiece.Bishop, Team.Black);
        CreatePiece(7, 6, ChessPiece.Knight, Team.Black);
        CreatePiece(7, 7, ChessPiece.Rook, Team.Black);
        for (int i = 0; i < 8; i++)
            CreatePiece(6, i, ChessPiece.Pawn, Team.Black);
    }

    public void CreatePiece(int r, int c, ChessPiece chessPiece, Team team)
    {
        TCPChess_Piece temp = Instantiate(TCPChess_Data.instance.pieceObject, pos[r, c], Quaternion.identity);
        temp.r = r;
        temp.c = c;
        temp.chessPiece = chessPiece;
        temp.team = team;
        temp.Init();
        temp.transform.SetParent(transform);
        chessMap[r, c] = temp;
    }
}
