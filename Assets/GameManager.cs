using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static List<Movement> board = new List<Movement>();
    List<GameObject> myPieces = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        board = new List<Movement>(Object.FindObjectsOfType<Movement>());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
