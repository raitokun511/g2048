using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBoard : MonoBehaviour
{
    List<GameObject> board;
    float timeMoveBlock;

    void Start()
    {
        board = new List<GameObject>();
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public Block GetBlock(int x, int y)
    {
        foreach (GameObject go in board)
        {
            Block bl = go.GetComponent<Block>();
            if ((int)bl.Coordinates.x == x && (int)bl.Coordinates.y == y)
                return bl;
        }
        return null;
    }
    public void Add(GameObject block)
    {
        if (board == null)
            board = new List<GameObject>();
        board.Add(block);
    }
    public int Count
    {
        get { return board.Count; }
    }

}
