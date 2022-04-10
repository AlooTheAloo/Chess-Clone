using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : MonoBehaviour
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


    public List<int> FindEndangeredPositions()
    {
        List<int> retval = new List<int>();
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
                if (Validate(i, j))
                    retval.Add(i * 10 + j);
        return retval;
    }
    public bool Validate(int destX, int destY)
    {
        if (destY == currPosY && destX == currPosX) return false;

        if (Mathf.Abs(destX - currPosX) <= 1 && Mathf.Abs(destY - currPosY) <= 1)
        {
            if (GameManager.PieceExists(destX, destY))
                return !(GameManager.PieceExists(destX, destY).GetComponent<Movement>().team == GetComponent<Movement>().team);    
            else
                return true;
        }
        else { return false; }


    }
}
