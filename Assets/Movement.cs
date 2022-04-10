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

    private void Start()
    {
        NetworkServer.SpawnObjects();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (team != Team.MINE || !GameManager.instance.myTurn) return;
        origPos = transform.position;
        origLocalPos = transform.localPosition;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (team != Team.MINE || !GameManager.instance.myTurn) return;
        gameObject.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (team != Team.MINE || !GameManager.instance.myTurn) return;
        if (transform.localPosition.x > maxBoardSize ||
            transform.localPosition.x < -maxBoardSize ||
            transform.localPosition.y > maxBoardSize ||
            transform.localPosition.y < -maxBoardSize
            )
        {
            print(transform.localPosition.x);
            print(transform.localPosition.y);
            transform.position = origPos;
        }
        //In the board
        else
        {
            Vector2 rtn = RoundToNearest(transform.localPosition, 40);
            transform.localPosition = rtn;
            if (rtn == origLocalPos) return;
            if (GameManager.instance.CheckForCheck(false))
            {

                return;
            }
            GameManager.instance.myTurn = false;
            GameManager.instance.CmdMovePiece(NetworkClient.connection.connectionId, WorldToScreen(origLocalPos.x), WorldToScreen(origLocalPos.y), WorldToScreen(rtn.x), WorldToScreen(rtn.y));
                       
        }
    }

    public void MovePiece(int destX, int destY)
    {
        transform.localPosition = new Vector2(ScreenToWorld(destX), ScreenToWorld(destY));
        switch (type) {
            case PieceType.Pawn: GetComponent<Pawn>().RefreshPos(destX, destY); break;
            case PieceType.Knight: GetComponent<Knight>().RefreshPos(destX, destY); break;
            case PieceType.Bishop: GetComponent<Bishop>().RefreshPos(destX, destY); break;
            case PieceType.Rook: GetComponent<Rook>().RefreshPos(destX, destY); break;
            case PieceType.Queen: GetComponent<Queen>().RefreshPos(destX, destY); break;
            case PieceType.King: GetComponent<King>().RefreshPos(destX, destY); break;
        }

             
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
            case PieceType.King: valid = GetComponent<King>().Validate(destX, destY); break;
        }
        if (valid)
        {
            if (GameManager.PieceExists(destX, destY))
            {
                Destroy(GameManager.PieceExists(destX, destY));
                GameManager.instance.CmdDestroy(destX, destY);            
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
                GetComponent<UnityEngine.UI.Image>().sprite = GameManager.instance.myPlayer == 1 ? GameManager.instance.whiteQueen : GameManager.instance.blackQueen;
            }
            return new Vector2(targetX, targetY);

        }
        else return origLocalPos;

    }



    

}
