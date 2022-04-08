using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : MonoBehaviour
{
    private const float maxBoardSize = 150f;
    public int currPosX = 0;
    public int currPosY = 0;
    public void RefreshPos(int destX, int destY)
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



    public bool Validate(int destX, int destY)
    {
        if (destY == currPosY && destX == currPosX) return false;
        Debug.Log(currPosX + ", " + currPosY + " to " + destX + ", " + destY);
        if (Mathf.Abs(destX - currPosX) != 0 && Mathf.Abs(destY - currPosY) == 0
            || Mathf.Abs(destY - currPosY) != 0 && Mathf.Abs(destX - currPosX) == 0)
        {
            for(int i = 1; i < Mathf.Max(Mathf.Abs(destX - currPosX),Mathf.Abs(destY - currPosY)); i++)
            {
                if(GameManager.PieceExists(destX - currPosX == 0 ? currPosX : destX - currPosX > 0 ? currPosX + i : currPosX - i,
                    destY - currPosY == 0 ? currPosY : destY - currPosY > 0 ? currPosY + i : currPosY - i))
                {
                    return false;
                }
            }
            
            if(GameManager.PieceExists(destX, destY))
            {
                if (GameManager.PieceExists(destX, destY).GetComponent<Movement>().team == Team.MINE) return false;
                else
                {
                    RefreshPos(destX, destY);
                    return true;
                }

            }
            else
            {
                RefreshPos(destX, destY);
                return true;
            }
            
        }

        return false;


    }
}
