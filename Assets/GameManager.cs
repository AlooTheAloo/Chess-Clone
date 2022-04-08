using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    private int myPlayer;
    public static List<Movement> board = new List<Movement>();
    public static GameManager instance;
    List<GameObject> myPieces = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer) return;
        instance = this;
        
        CmdNewPlayer();
    }

    [Command(requiresAuthority =false)]
    public void CmdDestroy(int x, int y)
    {
        if (isLocalPlayer) //The server called this
        {
            NetworkClient.localPlayer.GetComponent<GameManager>().RPCDestroy(7 -x, 7-y);
        }
        else
        {
            RPCDestroy(7 - x, 7 - y);
        }
    }

    [ClientRpc(includeOwner = true)]
    public void RPCDestroy(int x, int y)
    {
        Destroy(PieceExists(x, y));
    }


    [Command(requiresAuthority = false)]
    private void CmdNewPlayer()
    {
        if (!isServer) return;
        Debug.Log("New Connextion! There are now " + NetworkServer.connections.Count + "and the name of my object is " + gameObject.name);
        int serverPlayer = Random.Range(0, 2); //0 - black, 1 - white
        if(NetworkServer.connections.Count == 2) RPCChangeColor(serverPlayer);
    }
    [ClientRpc]
    private void RPCChangeColor(int newColor)
    {
        myPlayer = newColor;
        if (isServer) NetworkClient.localPlayer.GetComponent<GameManager>().SetMyPlayer(newColor);
        else NetworkClient.localPlayer.GetComponent<GameManager>().SetMyPlayer(1 - newColor);
    }

    public void SetMyPlayer(int newPlayer)
    {
        this.myPlayer = newPlayer;
        if(newPlayer == 0) GameObject.Find("Pieces").transform.Find("AsWhite").gameObject.SetActive(true);
        else GameObject.Find("Pieces").transform.Find("AsBlack").gameObject.SetActive(true);

        board = new List<Movement>(Object.FindObjectsOfType<Movement>());
        foreach (Movement go in board)
            print(go.gameObject.name);
    }


    private void  OnConnectedToServer()
    {
        Debug.Log("Connection to server!");
    }


    [Command(requiresAuthority = false)]
    public void CmdMovePiece(int con, int xO, int yO, int xF, int yF)
    {


        RPCMovePiece(xO, yO, xF, yF);   
    }


    [ClientRpc(includeOwner = false)]
    public void RPCMovePiece(int xO, int yO, int xF, int yF)
    {
        Debug.Log("Piece at " + xO + ", " + yO + " has been moved to " + xF + ", " + yF);
        PieceExists(7 - xO, 7 - yO).GetComponent<Movement>().MovePiece(7 - xF, 7 - yF);
        
    }

    

    public static GameObject PieceExists(int destX, int destY)
    {
        foreach (Movement piece in GameManager.board)
        {
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
