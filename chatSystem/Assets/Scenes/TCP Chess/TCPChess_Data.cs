using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ChessPiece
{
    King, Queen, Knight, Rook, Bishop, Pawn , Empty
};

public enum Team
{
    Black,White
};

public class TCPChess_Data : MonoBehaviour
{
    public static TCPChess_Data instance;  

    public TCPChesss_Board chessBoard;
    public GameObject chessBoardPiece;

    public EventTrigger selectCheck;

    public TCPChess_Piece pieceObject;

    private void Awake() => instance = this;

    private void Start() => chessBoard.enabled = true;

    public Color[] pieceColor = { new Color(0.2f, 0.2f, 0.2f), new Color(1f, 1f, 1f) };
    public Color[] boardColor = { new Color(0.2f, 0.2f, 0.2f), new Color(1f, 1f, 1f) };
    public Color canMoveColor;
    public Color warningColor;

    public Sprite[] pieceImage;

}
