using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public TextMeshProUGUI text;

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
    public void SetValue(int value)
    {
        if (value > 0)
        {
            text.gameObject.SetActive(true);
            text.text = value.ToString();
        }
        else
        {
            text.gameObject.SetActive(false);
        }
    }
}
