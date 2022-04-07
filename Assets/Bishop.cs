using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : MonoBehaviour
{
    private const float maxBoardSize = 150f;
    public int currPosX = 0;
    public int currPosY = 0;
    void RefreshPos(int destX, int destY)
    {
        currPosX = destX;
        currPosY = destY;
    }
    private void Start()
    {
        FindCurrentPos(transform.localPosition.x, transform.localPosition.y);
    }

    void FindCurrentPos(float destX, float destY)
    {
        List<float> list = new List<float>();
        for (int i = (int)-maxBoardSize + 10; i < maxBoardSize; i += 40)
        {
            list.Add(i);
        }
        currPosX = list.IndexOf(Mathf.Round(destX));
        currPosY = list.IndexOf(Mathf.Round(destY));
    }

    private GameObject PieceExists(int destX, int destY)
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
            }
        }
        return null;
    }

    public bool Validate(int destX, int destY)
    {
   

        if (Mathf.Abs(destX -currPosX) == Mathf.Abs(destY -currPosY)) { 
            
            
            for (int i = 0; i < Mathf.Abs(destX - currPosX); i++)
                if (PieceExists(
                    destX > currPosX ? 
                    currPosX + i : currPosX - i,
                    destY > currPosY ?
                    currPosY + i : currPosY - i)) return false;
            RefreshPos(destX, destY);
            return true; 
        }
        else return false;


    }
}
