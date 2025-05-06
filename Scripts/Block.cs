using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer sprite;
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
            if (value > 512)
            {
                text.fontSize = 4;
            }
            else if (value > 64)
            {
                text.fontSize = 5;
            }
        }
        SetColor(value);
    }

    public void SetColor(int value)
    {
        Color c = sprite.color;
        if (value <= 0)
        {
            c = new Color(0.7f, 0.68f, 0.68f, 1);
        }
        if (value == 2)
        {
            c = new Color(1f, 0.92f, 0.81f, 1);
        }
        if (value == 4)
        {
            c = new Color(0.97f, 0.88f, 0.73f, 1);
        }
        if (value == 8)
        {
            c = new Color(1f, 0.75f, 0.35f, 1);
        }
        if (value == 16)
        {
            c = new Color(1f, 0.65f, 0.4f, 1);
        }
        if (value == 32)
        {
            c = new Color(1f, 0.55f, 0.4f, 1);
        }
        if (value == 64)
        {
            c = new Color(1f, 0.3f, 0.27f, 1);
        }
        if (value == 128)
        {
            c = new Color(1f, 0.93f, 0.53f, 1);
        }
        if (value == 256)
        {
            c = new Color(1f, 0.88f, 0.51f, 1);
        }
        if (value == 512)
        {
            c = new Color(1f, 0.85f, 0.36f, 1);
        }
        if (value == 1024)
        {
            c = new Color(1f, 0.85f, 0.36f, 1);
        }
        if (value == 2048)
        {
            c = new Color(1f, 0.85f, 0.36f, 1);
        }
        sprite.color = c;

    }
}
