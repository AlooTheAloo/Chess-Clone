using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    private const float maxBoardSize = 150f;
    bool hasDoneFirstMove;
    public int currPosX = 0;
    public int currPosY = 0;

    // Start is called before the first frame update
    void Start()
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
    
    void RefreshPos(int destX, int destY)
    {
        hasDoneFirstMove = true;
        currPosX = destX;
        currPosY = destY;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Validate(int destX, int destY)
    {
        if (destX == currPosX && destY == currPosY) return false;




        Debug.Log(currPosX + ", " + currPosY + " to " + destX + ", " + destY);
        //Can do double
        if (!hasDoneFirstMove)
        {
            if (destX - currPosX == 0 && destY - currPosY <= 2)
            {
                if (destY - currPosY == 2)
                {
                    if (PieceExists(destX, destY) || PieceExists(destX, destY - 1)) return false;
                }
                else if (PieceExists(destX, destY)) return false;
                RefreshPos(destX, destY);
                return true;
            }
            else
            {
                if (PieceExists(destX, destY) && Mathf.Abs(destX - currPosX) == 1 && destX - currPosX == 1)
                {
                    if (PieceExists(destX, destY).GetComponent<Movement>().team == Team.MINE) return false;
                    //Eat the piece
                    RefreshPos(destX, destY);
                    return true;
                }
                else return false;
            }
        }
        else
        {
            if (destX - currPosX == 0 && destY - currPosY == 1)
            {
                if (PieceExists(destX, destY)) return false;
                RefreshPos(destX, destY);
                return true;
            }
            else
            {
                if (PieceExists(destX, destY) && Mathf.Abs(destX - currPosX) == 1 && destX - currPosX == 1)
                {
                    if (PieceExists(destX, destY).GetComponent<Movement>().team == Team.MINE) return false;
                    print(PieceExists(destX, destY).GetComponent<Movement>().team);
                    //Eat the piece
                    RefreshPos(destX, destY);
                    return true;
                }
                else return false;
            }
        }
    }

    private GameObject PieceExists(int destX, int destY)
    {
        foreach(Movement piece in GameManager.board)
        {
            switch (piece.type)
            {
                case PieceType.Pawn: if (piece.GetComponent<Pawn>().currPosX == destX &&
                        piece.GetComponent<Pawn>().currPosY == destY) return piece.gameObject; break;
                case PieceType.Knight: if (piece.GetComponent<Knight>().currPosX == destX &&
                 piece.GetComponent<Knight>().currPosY == destY) return piece.gameObject; break;
            }
        }
        return null;
    }

}
