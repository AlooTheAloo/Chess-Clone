using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    private const float maxBoardSize = 150f;
    public int currPosX = 0;
    public int currPosY = 0;
    public void RefreshPos(int destX, int destY)
    {
        Debug.Log("Called with " + destX + ", " + destY);
        currPosX = destX;
        currPosY = destY;
    }
    private void Start()
    {
        FindCurrentPos(transform.localPosition.x, transform.localPosition.y);
    }

    void FindCurrentPos(float destX, float destY)
    {
        Debug.Log(gameObject.name + "Called with " + destX + ", " + destY);
        List<float> list = new List<float>();
        for (int i = (int)-maxBoardSize + 10; i < maxBoardSize; i += 40)
        {
            list.Add(i);
        }
        currPosX = list.IndexOf(Mathf.Round(destX));
        currPosY = list.IndexOf(Mathf.Round(destY));
        Debug.Log("Finished math with " + currPosX + ", " + currPosY);
    }



    public bool Validate(int destX, int destY)
    {
        if (destY == currPosY && destX == currPosX) return false;

        if ((Mathf.Abs(destX - currPosX) == 2 && Mathf.Abs(destY - currPosY) == 1) || 
            (Mathf.Abs(destX - currPosX) == 1 && Mathf.Abs(destY - currPosY) == 2))
        {
            //move valide
            if (GameManager.PieceExists(destX, destY))
            {
                //Eat
                if (GameManager.PieceExists(destX, destY).GetComponent<Movement>().team == Team.MINE) return false;
                else
                {
                    //Actually eat
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
        else return false;
        

    }
}
