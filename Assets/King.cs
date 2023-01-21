using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class King : MonoBehaviour
{
    private const float maxBoardSize = 150f;
    public int currPosX = 0;
    public int currPosY = 0;
    public bool moved;
    public void RefreshPos(int destX, int destY)
    {
        currPosX = destX;
        currPosY = destY;
    }
    private void Start()
    {
        FindForBack();
    }

    public void FindForBack()
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


    public List<string> FindEndangeredPositions()
    {
        List<string> retval = new List<string>();
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (Validate(i, j))
                    retval.Add(i + "|" + j);
        return retval;
    }
    public bool Validate(int destX, int destY, bool playerMove = false)
    {
        if (destY == currPosY && destX == currPosX) return false;

        if (Mathf.Abs(destX - currPosX) <= 1 && Mathf.Abs(destY - currPosY) <= 1)
        {
            if (GameManager.PieceExists(destX, destY))
                return !(GameManager.PieceExists(destX, destY).GetComponent<Movement>().team == GetComponent<Movement>().team);    
            else
                return true;
        }
        else if (currPosX - destX == (GameManager.instance.myPlayer == 1 ? 2 : -2) && 
            destY == currPosY && playerMove && !GameManager.instance.CheckForCheck(false))
        {
            Rook targetRook = Rook.rooks.Where(x => x.side == Side.KingSide).ToArray()[0];
            if (!targetRook.moved)
            {
                int middle = currPosX - (GameManager.instance.myPlayer == 1 ? 1 : -1);
                if (!GetComponent<Movement>().SimulateMovePiece(middle, currPosY, true)
                && !GetComponent<Movement>().SimulateMovePiece(destX, destY, true))
                {
                    if (!GameManager.PieceExists(middle, destY) && !GameManager.PieceExists(destX, destY))
                    {
                        GetComponent<Movement>().MovePiece(destX, destY, true);
                        int rookOrigX = targetRook.currPosX;
                        int rookOrigY = targetRook.currPosY;
                        targetRook.GetComponent<Movement>().MovePiece(middle, destY, true);
                        GameManager.instance.CmdMovePiece(NetworkClient.connection.connectionId, rookOrigX, rookOrigY, targetRook.currPosX, targetRook.currPosY);

                        return true;
                    }
                }
            }
            return false;
        }

        else if (currPosX - destX == (GameManager.instance.myPlayer == 1 ? -2 : 2)
            && destY - currPosY == 0 && playerMove && !GameManager.instance.CheckForCheck(false))
        {
            Rook targetRook = Rook.rooks.Where(x => x.side == Side.QueenSide).ToArray()[0];

            if (!targetRook.moved)
            {
                int middle = currPosX - (GameManager.instance.myPlayer == 1 ? -1 : 1);
                if (!GetComponent<Movement>().SimulateMovePiece(middle, currPosY, true)
                && !GetComponent<Movement>().SimulateMovePiece(destX, destY, true))
                {
                    if (!GameManager.PieceExists(middle, destY) && !GameManager.PieceExists(destX, destY) && 
                        !GameManager.PieceExists(destX + (GameManager.instance.myPlayer == 1 ? -1 : 1), destY) )
                    {
                        GetComponent<Movement>().MovePiece(destX, destY, true);
                        int rookOrigX = targetRook.currPosX;
                        int rookOrigY = targetRook.currPosY;
                        targetRook.GetComponent<Movement>().MovePiece(middle, destY, true);
                        GameManager.instance.CmdMovePiece(NetworkClient.connection.connectionId, rookOrigX, rookOrigY, targetRook.currPosX, targetRook.currPosY);

                        return true;
                    }
                }
            }
            return false;
        }
        else { return false; }


    }
}
