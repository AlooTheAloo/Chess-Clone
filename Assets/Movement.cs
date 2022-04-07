using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class Movement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PieceType type;
    public Team team;
    private const float maxBoardSize = 150f;
    private Vector2 origPos;
    private Vector2 origLocalPos;
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (team != Team.MINE) return;
        origPos = transform.position;
        origLocalPos = transform.localPosition;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (team != Team.MINE) return;
        gameObject.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (team != Team.MINE) return;
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
        }
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
        }
        print(valid);
        if (valid)
            return new Vector2(targetX, targetY);
        else return origLocalPos;

    }
}
