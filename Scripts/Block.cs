using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum RotateDirection
    {
        Left,
        Right,
        Up,
        Down
    };
    public enum BlockType
    {
        Empty,
        Root
    };
    public enum BlockState
    {
        Empty,
        Obtain
    };
    public BlockType Type
    {
        get;
        set;
    }
    public BlockState State
    {
        get;
        set;
    }
    public int ColorType
    {
        get;
        set;
    }
    public Vector2 Coordinates
    {
        get;
        set;
    }
    public bool Selected
    {
        get; set;
    }
    Block nBlock;
    public Block nextBlock
    {
        set {
            //Debug.Log("NextBlock Set HEre");
            nBlock = value; }
        get { return nBlock; }
    }
    Block pBlock;
    public Block prevBlock
    {
        set
        {
            //Debug.Log("NextBlock Set HEre");
            pBlock = value;
        }
        get { return pBlock; }
    }
    public Block otherRoot = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   
    public static int BlockDistance(Block startBlock, Block endBlock)
    {
        return Mathf.Abs((int)startBlock.Coordinates.x - (int)endBlock.Coordinates.x) + Mathf.Abs((int)startBlock.Coordinates.y - (int)endBlock.Coordinates.y);
    }

    public void Move(GameObject movePiece, GameObject targetPiece, RotateDirection direction)
    {
        targetPiece.transform.transform.position = movePiece.transform.position + new Vector3(0, 0, -0.2f);
    }
    public void AddBlock(GameObject blockSelectRev)
    {
        GameObject findTail = blockSelectRev;
        while (findTail.transform.childCount > 0)
            findTail = findTail.transform.GetChild(0).gameObject;
        Debug.Log("FindTail " + findTail.name +"  with select " + blockSelectRev +"  Tail2Tail " + gameObject.name);

        gameObject.transform.parent = findTail.transform;
        blockSelectRev.transform.position = gameObject.transform.position + new Vector3(0, 0, -0.2f);
    }

}
