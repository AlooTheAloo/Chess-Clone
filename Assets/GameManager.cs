using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static List<Movement> board = new List<Movement>();
    List<GameObject> myPieces = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        board = new List<Movement>(Object.FindObjectsOfType<Movement>());
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            }
        }
        return null;
    }            
}
