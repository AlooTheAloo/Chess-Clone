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
                    if (GameManager.PieceExists(destX, destY) || GameManager.PieceExists(destX, destY - 1)) return false;
                }
                else if (GameManager.PieceExists(destX, destY)) return false;
                RefreshPos(destX, destY);
                return true;
            }
            else
            {
                if (GameManager.PieceExists(destX, destY) && Mathf.Abs(destX - currPosX) == 1 && destX - currPosX == 1)
                {
                    if (GameManager.PieceExists(destX, destY).GetComponent<Movement>().team == Team.MINE) return false;
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
                if (GameManager.PieceExists(destX, destY)) return false;
                RefreshPos(destX, destY);
                return true;
            }
            else
            {
                if (GameManager.PieceExists(destX, destY) && Mathf.Abs(destX - currPosX) == 1 && destX - currPosX == 1)
                {
                    if (GameManager.PieceExists(destX, destY).GetComponent<Movement>().team == Team.MINE) return false;
                    print(GameManager.PieceExists(destX, destY).GetComponent<Movement>().team);
                    //Eat the piece
                    RefreshPos(destX, destY);
                    return true;
                }
                else return false;
            }
        }
    }

    

}
