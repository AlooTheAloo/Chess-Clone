using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static List<Movement> board = new List<Movement>();
    public static GameManager instance;
    List<GameObject> myPieces = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        board = new List<Movement>(Object.FindObjectsOfType<Movement>());

    }

    


    [Command(requiresAuthority = false)]
    public void CmdMovePiece(int xO, int yO, int xF, int yF)
    {
        RPCMovePiece(xO, yO, xF, yF);
    }


    [ClientRpc]
    public void RPCMovePiece(int xO, int yO, int xF, int yF)
    {
        Debug.Log("Piece at " + xO + ", " + yO + " has been moved to " + xF + ", " + yF);
    }

    public static GameObject PieceExists(int destX, int destY)
    {
        foreach (Movement piece in GameManager.board)
        {
            switch (piece.type)
            {
                case PieceType.Pawn:
                    if (piece.GetComponent<Pawn>().currPosX == destX &&
                    piece.GetComponent<Pawn>().currPosY == destY) return piece.gameObject; break;
                case PieceType.Knight:
                    if (piece.GetComponent<Knight>().currPosX == destX &&
                    piece.GetComponent<Knight>().currPosY == destY) return piece.gameObject; break;
                case PieceType.Bishop:
                    if (piece.GetComponent<Bishop>().currPosX == destX &&
                    piece.GetComponent<Bishop>().currPosY == destY) return piece.gameObject; break;
                case PieceType.Rook:
                    if (piece.GetComponent<Rook>().currPosX == destX &&
                    piece.GetComponent<Rook>().currPosY == destY) return piece.gameObject; break;
                case PieceType.Queen:
                    if (piece.GetComponent<Queen>().currPosX == destX &&
                    piece.GetComponent<Queen>().currPosY == destY) return piece.gameObject; break;
                case PieceType.King:
                    if (piece.GetComponent<King>().currPosX == destX &&
                    piece.GetComponent<King>().currPosY == destY) return piece.gameObject; break;
            }
        }
        return null;
    }            
}
