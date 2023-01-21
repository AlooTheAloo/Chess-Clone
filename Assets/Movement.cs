using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;
using Mirror;

public class Movement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PieceType type;
    public Team team;
    private const float maxBoardSize = 150f;
    private Vector2 origPos;
    private Vector2 origLocalPos;
    public bool isDestroyed;
    public bool dragging;
    private void Start()
    {
        NetworkServer.SpawnObjects();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (team != Team.MINE || !GameManager.instance.myTurn || dragging) return;
        origPos = transform.position;
        origLocalPos = transform.localPosition;
        dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (team != Team.MINE || !GameManager.instance.myTurn || !dragging) { 
            eventData.pointerDrag = null;
            return;
        }
        gameObject.transform.position = eventData.position;
    }


    public List<string> GetValidMoves()
    {
        //All valid moves, stored as the form x|y
        List<string> retVal = new List<string>();
        for (int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if(validate(i, j)) retVal.Add(i + "|" + j);
            }
        }


        return retVal;
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        if (team != Team.MINE || !GameManager.instance.myTurn) return;
        dragging = false;
       //Out the board, cancel move
        if (transform.localPosition.x > maxBoardSize ||
            transform.localPosition.x < -maxBoardSize ||
            transform.localPosition.y > maxBoardSize ||
            transform.localPosition.y < -maxBoardSize
            )
        {
            transform.position = origPos;

        }

        //In the board
        else
        {
            print("In the board!");
            Vector2 rtn = RoundToNearest(transform.localPosition, 40); //Verifies if the move is valid
            transform.localPosition = rtn;
            if (rtn == origLocalPos) return;
            if (GameManager.instance.CheckForCheck(false))
            {
                GameManager.instance.DestroyMarkedPieces(false);
                print("You are in check, you can't move there!");
                transform.localPosition = origLocalPos;
                RefreshAllPos();
                return;
            }


            if (type == PieceType.Rook)
            {
                GetComponent<Rook>().moved = true;
            }

            GameManager.instance.DestroyMarkedPieces(true);
            GameManager.instance.myTurn = false;
            foreach(Movement m in GameManager.board)
            {
                if(m.type == PieceType.Pawn) {
                    m.GetComponent<Pawn>().canGetEnPassented = false;
                }
            }
            //Check for Checkmate
            GameManager.instance.CmdMovePiece(NetworkClient.connection.connectionId, WorldToScreen(origLocalPos.x), WorldToScreen(origLocalPos.y), WorldToScreen(rtn.x), WorldToScreen(rtn.y));
            StartCoroutine(WaitforCheckmate());
        }
    }


    private IEnumerator WaitforCheckmate()
    {
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.CheckForCheckMate();
    }

    public void RefreshAllPos()
    {
        switch (type)
        {
            case PieceType.Pawn: GetComponent<Pawn>().FindForBack(); break;
            case PieceType.Knight: GetComponent<Knight>().FindForBack(); break;
            case PieceType.Bishop: GetComponent<Bishop>().FindForBack(); break;
            case PieceType.Rook: GetComponent<Rook>().FindForBack(); break;
            case PieceType.Queen: GetComponent<Queen>().FindForBack(); break;
            case PieceType.King: GetComponent<King>().FindForBack(); break;
        }
        
    }



    


    public void MovePiece(int destX, int destY, bool changeSprite)
    {
        if(changeSprite) transform.localPosition = new Vector2(ScreenToWorld(destX), ScreenToWorld(destY));
        switch (type) {
            case PieceType.Pawn: GetComponent<Pawn>().RefreshPos(destX, destY); break;
            case PieceType.Knight: GetComponent<Knight>().RefreshPos(destX, destY); break;
            case PieceType.Bishop: GetComponent<Bishop>().RefreshPos(destX, destY); break;
            case PieceType.Rook: GetComponent<Rook>().RefreshPos(destX, destY); break;
            case PieceType.Queen: GetComponent<Queen>().RefreshPos(destX, destY); break;
            case PieceType.King: GetComponent<King>().RefreshPos(destX, destY); break;
        }


    }


    public bool SimulateMovePiece(int destX, int destY, bool castle = false)
    {
        int origX = (int) GetPosition().x;
        int origY = (int) GetPosition().y;
        GameObject pe = GameManager.PieceExists(destX, destY);
        
        if (pe)
        {
            pe.GetComponent<Movement>().isDestroyed = true;
        }
        MovePiece(destX, destY, false);
        bool retVal = GameManager.instance.CheckForCheck(!castle);
        GameManager.instance.DestroyMarkedPieces(false);

        MovePiece(origX, origY, false);

        return retVal;
    }

    private int WorldToScreen(float pos)
    {
        List<float> list = new List<float>();
        for (int i = (int)-maxBoardSize + 10; i < maxBoardSize; i += 40)
        {
            list.Add(i);
        }
        return list.IndexOf(pos);
    }

    private float ScreenToWorld(int pos)
    {
        List<float> list = new List<float>();
        for (int i = (int)-maxBoardSize + 10; i < maxBoardSize; i += 40)
        {
            list.Add(i);
        }
        return list[pos];
    }

    public bool validate(int destX, int destY)
    {
        switch (type)
        {
            case PieceType.Pawn: return GetComponent<Pawn>().Validate(destX, destY);
            case PieceType.Knight: return GetComponent<Knight>().Validate(destX, destY);
            case PieceType.Bishop: return GetComponent<Bishop>().Validate(destX, destY);
            case PieceType.Rook: return GetComponent<Rook>().Validate(destX, destY);
            case PieceType.Queen: return GetComponent<Queen>().Validate(destX, destY);
            case PieceType.King: return GetComponent<King>().Validate(destX, destY);
        }
        
        return false;
    }

    public List<string> Endanger(){
        List<string> retval = new List<string>();
        switch (type)
        {
            case PieceType.Pawn: retval = GetComponent<Pawn>().FindEndangeredPositions(); break;
            case PieceType.Knight: retval = GetComponent<Knight>().FindEndangeredPositions(); break;
            case PieceType.Bishop: retval = GetComponent<Bishop>().FindEndangeredPositions(); break;
            case PieceType.Rook: retval = GetComponent<Rook>().FindEndangeredPositions(); break;
            case PieceType.Queen: retval = GetComponent<Queen>().FindEndangeredPositions(); break;
            case PieceType.King: retval = GetComponent<King>().FindEndangeredPositions(); break;
        }
        return retval;
    }

    public Vector2 GetPosition()
    {
        Vector2 retVal = new Vector2();

        switch (type)
        {
            case PieceType.Pawn: retVal.x = GetComponent<Pawn>().currPosX; retVal.y = GetComponent<Pawn>().currPosY; break;
            case PieceType.Knight: retVal.x = GetComponent<Knight>().currPosX; retVal.y = GetComponent<Knight>().currPosY; break;
            case PieceType.Bishop: retVal.x = GetComponent<Bishop>().currPosX; retVal.y = GetComponent<Bishop>().currPosY; break;
            case PieceType.Rook: retVal.x = GetComponent<Rook>().currPosX; retVal.y = GetComponent<Rook>().currPosY; break;
            case PieceType.Queen: retVal.x = GetComponent<Queen>().currPosX; retVal.y = GetComponent<Queen>().currPosY; break;
            case PieceType.King: retVal.x = GetComponent<King>().currPosX; retVal.y = GetComponent<King>().currPosY; break;
        }
        return retVal;

    }

    private Vector2 RoundToNearest(Vector2 pos, int mult)
    {
        List<float> list = new List<float>();
        for (int i =(int) -maxBoardSize + 10; i < maxBoardSize; i += mult)
        {
            list.Add(i);
        }

        int origX = list.IndexOf(Mathf.Round(pos.x));
        int origY = list.IndexOf(Mathf.Round(pos.y));
        float targetX = pos.x;
        float targetY = pos.y;

        targetX = list.Aggregate((x, y) => Mathf.Abs(x - targetX) < Mathf.Abs(y - targetX) ? x : y);
        targetY = list.Aggregate((x, y) => Mathf.Abs(x - targetY) < Mathf.Abs(y - targetY) ? x : y);

        int destX = list.IndexOf(Mathf.Round(targetX));
        int destY = list.IndexOf(Mathf.Round(targetY));

        bool valid = false;

        switch (type)
        {
            case PieceType.Pawn: valid = GetComponent<Pawn>().Validate(destX, destY); break;
            case PieceType.Knight: valid = GetComponent<Knight>().Validate(destX, destY); break;
            case PieceType.Bishop: valid = GetComponent<Bishop>().Validate(destX, destY); break;
            case PieceType.Rook: valid = GetComponent<Rook>().Validate(destX, destY); break;
            case PieceType.Queen: valid = GetComponent<Queen>().Validate(destX, destY); break;
            case PieceType.King: valid = GetComponent<King>().Validate(destX, destY, true); break;
        }
        if (valid)
        {
            if (GameManager.PieceExists(destX, destY))
            {
                GameManager.PieceExists(destX, destY).GetComponent<Movement>().isDestroyed = true;
                //Destroy(GameManager.PieceExists(destX, destY));
                //Debug.Log("Piece destroyed!");
               //GameManager.instance.CmdDestroy(destX, destY);            
            }
            switch (type)
            {
                case PieceType.Pawn: GetComponent<Pawn>().RefreshPos(destX, destY); break;
                case PieceType.Knight: GetComponent<Knight>().RefreshPos(destX, destY); break;
                case PieceType.Bishop: GetComponent<Bishop>().RefreshPos(destX, destY); break;
                case PieceType.Rook: GetComponent<Rook>().RefreshPos(destX, destY); break;
                case PieceType.Queen: GetComponent<Queen>().RefreshPos(destX, destY); break;
                case PieceType.King: GetComponent<King>().RefreshPos(destX, destY); break;
            }
            if(type == PieceType.Pawn && destY == 7)
            {
               GetComponent<Pawn>().isQueen = true;
                GetComponent<UnityEngine.UI.Image>().sprite = GameManager.instance.myPlayer == 0 ? GameManager.instance.whiteQueen : GameManager.instance.blackQueen;
            }
            return new Vector2(targetX, targetY);

        }
        else return origLocalPos;

    }



    

}
