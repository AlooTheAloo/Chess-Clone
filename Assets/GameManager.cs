using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    [HideInInspector]
    public int myPlayer;
    public bool myTurn = false;
    public static List<Movement> board = new List<Movement>();
    public static GameManager instance;
    public Sprite whiteQueen;
    public Sprite blackQueen;
    List<GameObject> myPieces = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer) return;
        instance = this; //This is the local player, so we assign it to 'instance'

        CmdNewPlayer(); //We alert the server that we joined.
    }

    public void DestroyMarkedPieces(bool destroy)
    {
        foreach (Movement m in board)
        {
            if (m == null) continue;
            if (m.isDestroyed)
            {
                if (destroy)
                {
                    int destX = (int)m.GetPosition().x;
                    int destY = (int)m.GetPosition().y;
                    Destroy(GameManager.PieceExistsMine(destX, destY));
                    GameManager.instance.CmdDestroy(destX, destY);
                }
                else m.isDestroyed = false;
            }
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdDestroy(int x, int y)
    {
        if (isLocalPlayer) //The server called this
            NetworkClient.localPlayer.GetComponent<GameManager>().RPCDestroy(7 - x, 7 - y); //We cal the localplayers destroy
        else //The server did not call this
            RPCDestroy(7 - x, 7 - y); // We normally destroy

    }

    [ClientRpc(includeOwner = false)]
    public void RPCDestroy(int x, int y)
    {
        Destroy(PieceExists(x, y));
    }

    public bool CheckForCheck(bool mine)
    {
        List<string> endangeredPositions = new List<string>(); //All positions on the board that are in danger, stored as 1|1 for example
        foreach (Movement m in board)
        {

            if (m == null) continue; //Has been destroyed
            if (m.isDestroyed) continue; //To destroy 

            //Is on the team we dont care about
            if (mine && m.team != Team.MINE) continue;
            if (!mine && m.team == Team.MINE) continue;

            List<string> newEndangeredPos = m.Endanger(); //The endangered pos for this GO


            //We add those new pos to the endangeredPositions list
            foreach (string pos in newEndangeredPos)
            {
                print(m + ", " + pos);
                endangeredPositions.Add(pos);
            }
        }

        foreach (string s in endangeredPositions)
        {
            //For example, you have -1|1 it will convert to {-1, 1}, and -1 as a string is length > 1
            if (s.Split('|')[0].Length > 1 || s.Split('|')[1].Length > 1) continue;

            if (PieceExists(s.Split('|')[0].ToCharArray()[0] - '0', s.Split('|')[1].ToCharArray()[0] - '0')) {
                if (PieceExists(s.Split('|')[0].ToCharArray()[0] - '0', s.Split('|')[1].ToCharArray()[0] - '0').GetComponent<Movement>().type == PieceType.King)
                {

                    //The king is in danger!
                    return true;
                }
            }
        }
        return false;
    }

    [Command(requiresAuthority = false)]
    public void CmdCheckmated()
    {
        RPCCheckmated();
    }

    [ClientRpc(includeOwner = false)]
    public void RPCCheckmated()
    {
        GameObject.Find("EndScreen").transform.GetChild(0).gameObject.SetActive(true);
    }



    public bool CheckForCheckMate()
    {

        if (!CheckForCheck(true))
        {
            return false; //If not in check currently, theres no possibility of a checkmate.
        }

        foreach (Movement m in board)
        {
            if (m == null) continue;
            if (m.team == Team.MINE) continue;
            if (m.isDestroyed) continue;


            List<string> moves = m.GetValidMoves();



            foreach (string s in moves)
            {
                if (s.Split('|')[0].Length > 1 || s.Split('|')[1].Length > 1) continue;

                if (!m.SimulateMovePiece(s.Split('|')[0].ToCharArray()[0] - '0', s.Split('|')[1].ToCharArray()[0] - '0'))
                {
                    print("You did not checkmate! Piece " + m.gameObject.name + " can do " + s + "!");
                    return false;
                }
            }
        }
        instance.CmdCheckmated();
        GameObject.Find("EndScreen").transform.GetChild(1).gameObject.SetActive(true);
        print("You checkmated your opponent lmao you won (they are very mad right now)");
        return true;
    }

    [Command(requiresAuthority = false)]
    private void CmdNewPlayer()
    {
        //New player has joined, we need to send this to the other clients
        if (!isServer) return;
        Debug.Log("New Connection! There are now " + NetworkServer.connections.Count + " and the name of my object is " + gameObject.name);
        int serverPlayer = Random.Range(0, 2); //0 - black, 1 - white
        if (NetworkServer.connections.Count == 2) RPCChangeColor(serverPlayer);
    }
    [ClientRpc]
    private void RPCChangeColor(int newColor)
    {
        //Create a color
        myPlayer = newColor;
        if (isServer) NetworkClient.localPlayer.GetComponent<GameManager>().SetMyPlayer(newColor);
        else NetworkClient.localPlayer.GetComponent<GameManager>().SetMyPlayer(1 - newColor);
    }

    //Change viewport depending on color
    public void SetMyPlayer(int newPlayer)
    {
        myTurn = newPlayer == 0;
        print("MyTurn : " + myTurn + ". New player : " + newPlayer);
        myPlayer = newPlayer;
        if (newPlayer == 0) GameObject.Find("Pieces").transform.Find("AsWhite").gameObject.SetActive(true);
        else GameObject.Find("Pieces").transform.Find("AsBlack").gameObject.SetActive(true);

        board = new List<Movement>(Object.FindObjectsOfType<Movement>());
    }


    //Move the piece on the server
    [Command(requiresAuthority = false)]
    public void CmdMovePiece(int con, int xO, int yO, int xF, int yF)
    {
        RPCMovePiece(xO, yO, xF, yF); //RPC that broadcast to the other client
    }


    [ClientRpc(includeOwner = false)]
    public void RPCMovePiece(int xO, int yO, int xF, int yF)
    {
        //If for some reason the piece is bugged, we dont want to continue this operation that will be flagged as hacking
        if (!PieceExists(7 - xO, 7 - yO)) return;
        instance.myTurn = true;
        //Move the piece
        PieceExists(7 - xO, 7 - yO).GetComponent<Movement>().MovePiece(7 - xF, 7 - yF, true);
        // Change to queen if its a pawn that just got to the other side
        StartCoroutine(ChangeIntoQueen(7 - xF, 7 - yF));



    }
    private IEnumerator ChangeIntoQueen(int destX, int destY)
    {
        yield return new WaitForEndOfFrame();
        if (PieceExists(destX, destY).GetComponent<Movement>().type == PieceType.Pawn && destY == 0)
        {
            Debug.Log("Changing to queen");
            PieceExists(destX, destY).GetComponent<UnityEngine.UI.Image>().sprite = myPlayer == 0 ? whiteQueen : blackQueen;
            PieceExists(destX, destY).GetComponent<Pawn>().isQueen = true;
        }
    }

    
    //Checks for all positions to see if the piece exists at that position, if it does, return it, if not return null
    //This method can be used as a boolean as well
    public static GameObject PieceExists(int destX, int destY)
    {
        foreach (Movement piece in GameManager.board)
        {
            if (piece == null) continue;
            if (piece.isDestroyed) continue;
            switch (piece.type)
            {
                case PieceType.Pawn:
                    if (piece.GetComponent<Pawn>().currPosX == destX &&
                    piece.GetComponent<Pawn>().currPosY == destY) return piece.gameObject;
                    break;
                case PieceType.Knight:
                    if (piece.GetComponent<Knight>().currPosX == destX &&
                    piece.GetComponent<Knight>().currPosY == destY) return piece.gameObject; break;
                case PieceType.Bishop:
                    if (piece.GetComponent<Bishop>().currPosX == destX &&
                    piece.GetComponent<Bishop>().currPosY == destY) return piece.gameObject; break;
                case PieceType.Rook:
                    if (piece.GetComponent<Rook>().currPosX == destX &&
                    piece.GetComponent<Rook>().currPosY == destY) return piece.gameObject; break;
                case PieceType.Queen:
                    if (piece.GetComponent<Queen>().currPosX == destX &&
                    piece.GetComponent<Queen>().currPosY == destY) return piece.gameObject; break;
                case PieceType.King:
                    if (piece.GetComponent<King>().currPosX == destX &&
                    piece.GetComponent<King>().currPosY == destY) return piece.gameObject; break;
            }
        }
        return null;
    }

    public static GameObject PieceExistsMine(int destX, int destY)
    {
        foreach (Movement piece in GameManager.board)
        {
            if (piece.team == Team.MINE) continue; 
            if (piece == null) continue;
            switch (piece.type)
            {
                case PieceType.Pawn:
                    if (piece.GetComponent<Pawn>().currPosX == destX &&
                    piece.GetComponent<Pawn>().currPosY == destY) return piece.gameObject;
                    break;
                case PieceType.Knight:
                    if (piece.GetComponent<Knight>().currPosX == destX &&
                    piece.GetComponent<Knight>().currPosY == destY) return piece.gameObject; break;
                case PieceType.Bishop:
                    if (piece.GetComponent<Bishop>().currPosX == destX &&
                    piece.GetComponent<Bishop>().currPosY == destY) return piece.gameObject; break;
                case PieceType.Rook:
                    if (piece.GetComponent<Rook>().currPosX == destX &&
                    piece.GetComponent<Rook>().currPosY == destY) return piece.gameObject; break;
                case PieceType.Queen:
                    if (piece.GetComponent<Queen>().currPosX == destX &&
                    piece.GetComponent<Queen>().currPosY == destY) return piece.gameObject; break;
                case PieceType.King:
                    if (piece.GetComponent<King>().currPosX == destX &&
                    piece.GetComponent<King>().currPosY == destY) return piece.gameObject; break;
            }
        }
        return null;
    }


}
