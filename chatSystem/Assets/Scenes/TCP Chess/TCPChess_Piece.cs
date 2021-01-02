using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TCPChess_Piece : MonoBehaviour
{
    public ChessPiece chessPiece;

    public Team team;

    public int r, c;

    [System.NonSerialized]
    public Image pieceImage;
    public Outline outline;

    public bool firstMove = false;

    public void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (pieceImage == null)
            pieceImage = transform.Find("Img").GetComponent<Image>();
        pieceImage.color = TCPChess_Data.instance.pieceColor[(int)team];
        pieceImage.sprite = TCPChess_Data.instance.pieceImage[(int)chessPiece];
        firstMove = true;
    }

    public void Select(bool b)
    {
        if (outline == null)
            outline = transform.Find("Img").GetComponent<Outline>();
        outline.enabled = b;
    }
}
