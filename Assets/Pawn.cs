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
    public bool isQueen = false;

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
    
    public void RefreshPos(int destX, int destY)
    {
        hasDoneFirstMove = true;
        currPosX = destX;
        currPosY = destY;
    }

    public List<string> FindEndangeredPositions()
    {
        List<string> retval = new List<string>();
        if(GetComponent<Movement>().team == Team.MINE)
        {
            print("My pawn");
            retval.Add(currPosX + 1 + "|" + (currPosY + 1));
            retval.Add(currPosX - 1 + "|" + (currPosY + 1));
        }
        else
        {
            print("Other pawn");
            retval.Add(currPosX + 1 + "|" + (currPosY - 1));
            retval.Add(currPosX - 1 + "|" + (currPosY - 1));
        }
        return retval;
    }
    

    public bool Validate(int destX, int destY)
    {
       if(!isQueen) {
        if (destX == currPosX && destY == currPosY) return false;
        if (destY <= currPosY) return false;
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
                    return true;
                }
                else
                {
                    if (GameManager.PieceExists(destX, destY) && Mathf.Abs(destX - currPosX) == 1 && destX - currPosX == 1)
                    {
                        if (GameManager.PieceExists(destX, destY).GetComponent<Movement>().team == GetComponent<Movement>().team) return false;
                        //Eat the piece
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
                    return true;
                }
                else
                {
                    if (GameManager.PieceExists(destX, destY) && Mathf.Abs(destX - currPosX) == 1)
                    {
                        if (GameManager.PieceExists(destX, destY).GetComponent<Movement>().team == GetComponent<Movement>().team) return false;
                        print(GameManager.PieceExists(destX, destY).GetComponent<Movement>().team);
                        //Eat the piece
                        return true;
                    }
                    else return false;
                }
            }
        }
        else
        {
            if (destY == currPosY && destX == currPosX) return false;
            if (Mathf.Abs(destX - currPosX) != 0 && Mathf.Abs(destY - currPosY) == 0
                || Mathf.Abs(destY - currPosY) != 0 && Mathf.Abs(destX - currPosX) == 0)
            {
                for (int i = 1; i < Mathf.Max(Mathf.Abs(destX - currPosX), Mathf.Abs(destY - currPosY)); i++)
                {
                    if (GameManager.PieceExists(destX - currPosX == 0 ? currPosX : destX - currPosX > 0 ? currPosX + i : currPosX - i,
                        destY - currPosY == 0 ? currPosY : destY - currPosY > 0 ? currPosY + i : currPosY - i))
                    {
                        return false;
                    }
                }

                if (GameManager.PieceExists(destX, destY))
                {
                    if (GameManager.PieceExists(destX, destY).GetComponent<Movement>().team == GetComponent<Movement>().team) return false;
                    else
                    {
                        return true;
                    }

                }
                else
                {
                    return true;
                }

            }
            if (Mathf.Abs(destX - currPosX) == Mathf.Abs(destY - currPosY))
            {
                for (int i = 1; i < Mathf.Abs(destX - currPosX); i++)
                    if (GameManager.PieceExists(
                        destX > currPosX ?
                        currPosX + i : currPosX - i,
                        destY > currPosY ?
                        currPosY + i : currPosY - i)) return false;


                if (!GameManager.PieceExists(destX, destY)) { return true; }
                if (GameManager.PieceExists(destX, destY).GetComponent<Movement>().team == GetComponent<Movement>().team) return false;
                else
                {
                    return true;
                }
            }
            else return false;
        }

    }

    

}
