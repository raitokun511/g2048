using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public static float MOVE_DELTA_THRESHOLD = 1;//Change by screen size
    public enum GameState
    {
        Begin,
        WaitChoose,
        Move,
        Done,
        Win,
        Fail
    };
    public static GameState state;
    public Tap tap;

    public GameObject[,] boardItem;
    public LevelSetting levelSetting;
    public GameObject winPanel;


    GameObject listObj;
    GameObject pieceSelected;
    GameObject previousPiece;
    GameObject rootSelected;
    int currentColortype;
    LevelConstructor levelInfo;


    int countMove = 0;
    bool gameStart = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

        GM.Init();

        listObj = new GameObject("Pieces");
        state = GameState.Begin;
        GameObject tapObj = new GameObject("Tap");
        tap =  tapObj.AddComponent<Tap>();
    }

    // Update is called once per frame
    void Update()
    {
        //----------------Init When Start Game-------------------------
        if (!gameStart)
        {
            gameStart = true;
            MakeLevel();
            StartCoroutine(WaitAndDo(1f, () => {
                state = GameState.WaitChoose;
            }));
        }


        if (state == GameState.Begin)
            return;

        //------------------End Init-----------------------------------
        //Debug.Log("State " + state);


        //-----------------------Win-Fail-----------------------------------
        if (state == GameState.Win)
        {

            winPanel.SetActive(true);
            if (Camera.main.transform.position.z > -26f)
                Camera.main.transform.position -= new Vector3(0, 0, 0.1f);
            return;
        }


        //-----------------------Win-Fail-----------------------------------


        //Wait Player Choose Block
        if (state == GameState.WaitChoose)
        {
            if (Input.GetMouseButtonDown(1) && pieceSelected != null)
            {
                pieceSelected.transform.Rotate(Vector3.right, 180);
            }

            if (tap.TapBegin())
            {
                Vector3 tapRay = Input.mousePosition;
                tapRay.z = 15;
                Ray touchPosition = Camera.main.ScreenPointToRay(tapRay);
                RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(tapRay), 1000.0f);

                //Debug.DrawRay(transform.position, Camera.main.transform.forward * 100f, Color.red);
               
                //Debug.Log("Mouse " + tap.WorldPosition);
                float min = 1000000;
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];
                    Debug.Log("Hit Block " + hits[i].transform.gameObject.name +"  " + hits[i].transform.GetComponent<Block>().Type);
                    if (hit.transform.GetComponent<Block>())
                    {
                        if (hit.transform.GetComponent<Block>().Type == Block.BlockType.Root ||
                             hit.transform.GetComponent<Block>().State == Block.BlockState.Obtain)// && hit.distance < min)
                        {
                            pieceSelected = hit.transform.gameObject;
                            Debug.DrawRay(touchPosition.origin, touchPosition.direction * 100f, Color.green);
                            min = hit.distance;
                        }
                    }
                }
                if (pieceSelected != null)
                {
                    Debug.Log("Select Block " + pieceSelected.name);
                    rootSelected = pieceSelected;
                    if (pieceSelected.transform.GetComponent<Block>().State == Block.BlockState.Obtain)
                        DestroyPathSelected(pieceSelected.transform.GetComponent<Block>());
                    pieceSelected.GetComponent<Block>().State = Block.BlockState.Obtain;

                    if (pieceSelected.GetComponent<Block>().Type == Block.BlockType.Root)
                    {
                        DestroyPathSelected(pieceSelected.GetComponent<Block>().otherRoot);
                        pieceSelected.GetComponent<Block>().otherRoot.State = Block.BlockState.Empty;
                    }
                    currentColortype = pieceSelected.GetComponent<Block>().ColorType;
                    state = GameState.Move;
                }
            }
           
        }
        if (state == GameState.Move)
        {
            if (tap.TapMove() && pieceSelected != null)
            {
                Ray touchPosition = Camera.main.ScreenPointToRay(Input.mousePosition);// : Camera.main.ScreenPointToRay(Input.touches[0].position);
                RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 100.0f);
                GameObject hitblock = null;
                for (int i = 0; i < hits.Length; i++)
                    if (hits[i].transform.GetComponent<Block>())
                    {
                        hitblock = hits[i].transform.gameObject;
                    }
                    if (hitblock == null ||  hitblock.transform.gameObject == pieceSelected)
                        return;

                if (true)//hitblock.transform.GetComponent<Block>())// && hit.transform.GetComponent<Block>().State == Block.BlockState.Empty)
                {
                    Debug.Log("HitMove  " + hitblock.name + "  " + (hitblock.GetComponent<Block>().prevBlock != null ?hitblock.GetComponent<Block>().prevBlock.gameObject.name : " "));
                    //Gặp root màu khác thì bỏ qua
                    if (hitblock.transform.GetComponent<Block>().Type == Block.BlockType.Root && hitblock.transform.GetComponent<Block>().ColorType != currentColortype)
                        return;

                    //Đi cùng màu 1 vòng rồi quay lại root ban đầu => Xóa toàn bộ path
                    if (hitblock.transform.GetComponent<Block>().Type == Block.BlockType.Root && hitblock.transform.GetComponent<Block>().State != Block.BlockState.Empty)
                    {
                        DestroyPathSelected(hitblock.transform.GetComponent<Block>());
                        pieceSelected = hitblock;
                        hitblock.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = levelSetting.dotSprite;
                        return;
                    }

                    Vector2 coor = hitblock.transform.GetComponent<Block>().Coordinates;
                    Vector2 coorRoot = pieceSelected.GetComponent<Block>().Coordinates;
                    //Debug.Log("State Move " + hit.transform.name +"  " + (Mathf.Abs(coor.x - coorRoot.x) + Mathf.Abs(coor.y - coorRoot.y)) +"   " + pieceSelected.GetComponent<Block>().Type);


                    if (Mathf.Abs(coor.x - coorRoot.x) + Mathf.Abs(coor.y - coorRoot.y) == 1)
                    {

                        boardItem[(int)coor.x, (int)coor.y].transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = levelSetting.dotlineSprite;
                        if (pieceSelected.GetComponent<Block>().Type == Block.BlockType.Root)
                        {
                            Debug.Log("Root Type");
                            pieceSelected.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = levelSetting.dotlineSprite;
                            //Debug.Log("Current Sprite " + pieceSelected.name +"   " + pieceSelected.GetComponent<SpriteRenderer>().sprite.name);
                            if (coor.x < coorRoot.x)
                                pieceSelected.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, -90);
                            else if (coor.x > coorRoot.x)
                                pieceSelected.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 90);
                            else if (coor.y < coorRoot.y)
                                pieceSelected.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 0);
                            else if (coor.y > coorRoot.y)
                                pieceSelected.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 180);


                        }
                        else
                        {
                            previousPiece = pieceSelected.GetComponent<Block>().prevBlock.gameObject;
                            Debug.Log("NotRoot Type  Hit: " + hitblock.name + "  Select: " + pieceSelected.name + "  Prev: " + previousPiece.name);

                            if (previousPiece != null)
                            {
                                Vector2 coorPrev = previousPiece.GetComponent<Block>().Coordinates;
                                if (coorPrev.x == coor.x || coorPrev.y == coor.y)
                                {
                                    pieceSelected.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = levelSetting.fulllineSprite;
                                }
                                else
                                {
                                    pieceSelected.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = levelSetting.twolineSprite;

                                    if (coor.x - coorPrev.x == 1 && coor.y - coorPrev.y == -1 && coor.x == coorRoot.x)
                                        pieceSelected.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 0);
                                    if (coor.x - coorPrev.x == -1 && coor.y - coorPrev.y == 1 && coor.y == coorRoot.y)
                                        pieceSelected.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 0);

                                    if (coor.x - coorPrev.x == 1 && coor.y - coorPrev.y == 1 && coor.y == coorRoot.y)
                                        pieceSelected.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 90);
                                    if (coor.x - coorPrev.x == -1 && coor.y - coorPrev.y == -1 && coor.x == coorRoot.x)
                                        pieceSelected.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 90);

                                    if (coor.x - coorPrev.x == 1 && coor.y - coorPrev.y == 1 && coor.x == coorRoot.x)
                                        pieceSelected.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, -90);
                                    if (coor.x - coorPrev.x == -1 && coor.y - coorPrev.y == -1 && coor.y == coorRoot.y)
                                        pieceSelected.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, -90);

                                    if (coor.x - coorPrev.x == 1 && coor.y - coorPrev.y == -1 && coor.y == coorRoot.y)
                                        pieceSelected.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 180);
                                    if (coor.x - coorPrev.x == -1 && coor.y - coorPrev.y == 1 && coor.x == coorRoot.x)
                                        pieceSelected.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 180);

                                }
                                //Nếu back, rồi đi lại tiếp

                            }
                        }

                        //Nếu là đụng trúng ô màu (đang Back quay lại / đang quay 1 vòng tròn về trúng path đã đi / đụng trúng đường màu khác)
                        if (hitblock.transform.GetComponent<Block>().State != Block.BlockState.Empty)
                        {
                            Debug.Log("Hit NOT Empty " + hitblock.transform.name + "   " + pieceSelected.name + "   " + previousPiece.name);
                            DestroyPathSelected(hitblock.transform.GetComponent<Block>());

                            //Nếu đụng chính đường màu hiện tại
                            if (hitblock.transform.GetComponent<Block>().ColorType == currentColortype)
                            {
                                pieceSelected = hitblock.GetComponent<Block>().prevBlock.gameObject;
                                previousPiece = pieceSelected.GetComponent<Block>().prevBlock != null ? pieceSelected.GetComponent<Block>().prevBlock.gameObject : pieceSelected;
                                Debug.Log("Hit Same Color : " + hitblock.transform.name + "   " + pieceSelected.name + "   " + previousPiece.name);

                            }
                            //Nếu đụng trúng đường màu khác
                            else
                            {
                                hitblock.GetComponent<Block>().ColorType = currentColortype;
                                if (hitblock.transform.GetComponent<Block>().prevBlock != null)
                                {
                                    hitblock.transform.GetComponent<Block>().prevBlock.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = levelSetting.dotlineSprite;
                                    hitblock.transform.GetComponent<Block>().prevBlock.nextBlock = null;
                                    if (hitblock.transform.GetComponent<Block>().prevBlock.prevBlock != null)
                                    {
                                        Vector3 cur = hitblock.transform.GetComponent<Block>().prevBlock.Coordinates;
                                        Vector3 prev = hitblock.transform.GetComponent<Block>().prevBlock.prevBlock.Coordinates;
                                        if (prev.x > cur.x)
                                            hitblock.transform.GetComponent<Block>().prevBlock.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 90);
                                        else if (prev.x < cur.x)
                                            hitblock.transform.GetComponent<Block>().prevBlock.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, -90);
                                        else if (prev.y > cur.y)
                                            hitblock.transform.GetComponent<Block>().prevBlock.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 180);
                                        else if (prev.y < cur.y)
                                            hitblock.transform.GetComponent<Block>().prevBlock.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 0);
                                    }
                                    else //Root
                                    {
                                        hitblock.transform.GetComponent<Block>().prevBlock.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = levelSetting.dotSprite;

                                    }

                                }
                                Debug.Log("Hit Other Color: " + hitblock.transform.name + "   " + pieceSelected.name + "   " + previousPiece.name);
                                Debug.Log("Hit  Other Color : " + hitblock.transform.name + "  WITH PREV: " + hitblock.transform.GetComponent<Block>().prevBlock.gameObject.name);
                                
                            }


                            if (hitblock.GetComponent<Block>().Type == Block.BlockType.Root)
                            {

                            }
                            else
                            {
                                Vector2 coorPre = previousPiece.GetComponent<Block>().Coordinates;
                                coorRoot = pieceSelected.GetComponent<Block>().Coordinates;

                                //Nếu là back lại ô góc
                                if (Mathf.Abs(coor.x - coorPre.x) == 1 && Mathf.Abs(coor.y - coorPre.y) == 1)
                                {
                                    pieceSelected.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = levelSetting.twolineSprite;
                                }   
                            }
                        }

                        if (coor.x < coorRoot.x)
                            boardItem[(int)coor.x, (int)coor.y].transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 90);
                        else if (coor.x > coorRoot.x)
                            boardItem[(int)coor.x, (int)coor.y].transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, -90);
                        else if (coor.y < coorRoot.y)
                            boardItem[(int)coor.x, (int)coor.y].transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 180);
                        else if (coor.y > coorRoot.y)
                            boardItem[(int)coor.x, (int)coor.y].transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 0);

                        boardItem[(int)coor.x, (int)coor.y].GetComponent<Block>().ColorType = currentColortype;
                        boardItem[(int)coor.x, (int)coor.y].transform.Find("sprite").GetComponent<SpriteRenderer>().color = levelSetting.getColor(currentColortype);



                        if (hitblock.transform.GetComponent<Block>().Type == Block.BlockType.Root)
                            state = GameState.Done;

                        pieceSelected.GetComponent<Block>().nextBlock = hitblock.transform.GetComponent<Block>();
                        hitblock.transform.GetComponent<Block>().prevBlock = pieceSelected.GetComponent<Block>();
                        //if (hitblock.transform.GetComponent<Block>().State == Block.BlockState.Empty)
                        //    previousPiece = pieceSelected;
                        pieceSelected = hitblock.transform.gameObject;
                        pieceSelected.GetComponent<Block>().State = Block.BlockState.Obtain;



                    }
                }
                
                if (pieceSelected != null)
                {
                    //state = GameState.Move;
                }

                
            }
        }

        if (checkSuccess())
        {
            state = GameState.Win;
        }


        if (tap.TapEnd())
            state = GameState.WaitChoose;

        /*
        string s = "";
        foreach (Vector3 coor in levelInfo.Pieces)
            s += boardItem[(int)coor.x, (int)coor.y].GetComponent<Block>().State +"  ";
        Debug.Log("TypeRoot  " + s);
        */
      
    }

    public void MakeLevel()
    {
        levelInfo = levelSetting.Levels[GM.Level - 1];
        GM.boardCol = levelInfo.Column;
        GM.boardRow = levelInfo.Row;
        boardItem = new GameObject[GM.boardCol, GM.boardRow];


        for (int i = 0; i < GM.boardCol; i++)
            for (int j = 0; j < GM.boardRow; j++)
            {
                boardItem[i, j] = Instantiate(levelSetting.PiecePrefabs[0], new Vector3(i * 1.4f ,j * 1.4f), Quaternion.Euler(0, 0, 0));
                boardItem[i, j].name += "_" + i + "_" + j;
                boardItem[i, j].transform.localScale = Vector3.one * 1.5f;
                boardItem[i, j].transform.Find("sprite").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                boardItem[i, j].GetComponent<Block>().Coordinates = new Vector2(i, j);
                boardItem[i, j].GetComponent<Block>().State = Block.BlockState.Empty;
                boardItem[i, j].GetComponent<Block>().Type = Block.BlockType.Empty;

            }
        foreach (Vector3 coor in levelInfo.Pieces)
        {
            boardItem[(int)coor.x, (int)coor.y].GetComponent<Block>().Type = Block.BlockType.Root;
            boardItem[(int)coor.x, (int)coor.y].GetComponent<Block>().ColorType = (int)coor.z;
            boardItem[(int)coor.x, (int)coor.y].transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = levelSetting.dotSprite;
            boardItem[(int)coor.x, (int)coor.y].transform.Find("sprite").GetComponent<SpriteRenderer>().color = levelSetting.getColor((int)coor.z);

        }
        foreach (Vector3 coor1 in levelInfo.Pieces)
            foreach (Vector3 coor2 in levelInfo.Pieces)
                if ((coor1.x != coor2.x || coor1.y != coor2.y) && (int)coor1.z == (int)coor2.z)
                {
                    boardItem[(int)coor1.x, (int)coor1.y].GetComponent<Block>().otherRoot = boardItem[(int)coor2.x, (int)coor2.y].GetComponent<Block>();
                    boardItem[(int)coor2.x, (int)coor2.y].GetComponent<Block>().otherRoot = boardItem[(int)coor1.x, (int)coor1.y].GetComponent<Block>();
                }
                state = GameState.WaitChoose;

    }

    void DestroyPathSelected(Block blockBegin)
    {
        int count = 0;
        Block current = blockBegin;
        Debug.Log("BEGINDestroy " + blockBegin.gameObject.name);

        for (int i = 0; i < GM.boardCol; i++)
            for (int j = 0; j < GM.boardRow; j++)
                if (boardItem[i,j].GetComponent<Block>().nextBlock == blockBegin)
                {
                    blockBegin.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = levelSetting.dotlineSprite;
                    //blockBegin.transform.Find("sprite").transform.rotation = Quaternion.Euler(boardItem[i, j].transform.Find("sprite").transform.rotation.eulerAngles);
                    if (blockBegin.Coordinates.y < boardItem[i, j].GetComponent<Block>().Coordinates.y)
                        blockBegin.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 180);
                    else if (blockBegin.Coordinates.y > boardItem[i, j].GetComponent<Block>().Coordinates.y)
                        blockBegin.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 0);
                    else if (blockBegin.Coordinates.x < boardItem[i, j].GetComponent<Block>().Coordinates.x)
                        blockBegin.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, 90);
                    else if (blockBegin.Coordinates.x > boardItem[i, j].GetComponent<Block>().Coordinates.x)
                        blockBegin.transform.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, -90);

                    //Debug.Log("Previous Block : " + boardItem[i, j].name + " to " + blockBegin.gameObject.name);
                    //previousPiece = boardItem[i, j];
                    //pieceSelected = boardItem[i, j];
                    //if (pieceSelected.GetComponent<Block>().prevBlock != null)
                    //    previousPiece = pieceSelected.GetComponent<Block>().prevBlock.gameObject;
                }

        while (current.nextBlock != null && count++ < 500)
        {
            current.nextBlock.transform.Find("sprite").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            Debug.Log("Destroy " + current.nextBlock.gameObject.name);
            current.nextBlock.GetComponent<Block>().State = Block.BlockState.Empty;
            //current.nextBlock.GetComponent<Block>().Type = Block.BlockType.Empty;
            Block tmp = current.nextBlock;
            current.nextBlock = null;
            current = tmp;
        }
        DebugLog();
    }
    public bool checkSuccess()
    {
        for (int i = 0; i < GM.boardCol; i++)
            for (int j = 0; j < GM.boardRow; j++)
                if (boardItem[i, j].GetComponent<Block>().State != Block.BlockState.Obtain)
                    return false;
        return true;
    }

    public void DebugLog()
    {

        int countOb = 0;
        int countEmOb = 0;
        for (int i = 0; i < GM.boardCol; i++)
            for (int j = 0; j < GM.boardRow; j++)
            {
                if (boardItem[i, j].GetComponent<Block>().State != Block.BlockState.Obtain && boardItem[i, j].GetComponent<Block>().Type == Block.BlockType.Root)
                {
                    boardItem[i, j].transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = levelSetting.dotSprite;
                    boardItem[i, j].transform.Find("sprite").GetComponent<SpriteRenderer>().color = levelSetting.getColor(boardItem[i, j].GetComponent<Block>().ColorType);
                    countEmOb++;
                }
                if (boardItem[i, j].GetComponent<Block>().Type == Block.BlockType.Root)
                    countOb++;
            }
        Debug.Log("CountOB  " + countOb + "   EmOB  " + countEmOb);

    }



    public IEnumerator WaitAndDo(float time, System.Action task)
    {
        yield return new WaitForSeconds(time);
        task();

    }

    public void reloadLevel()
    {
        SceneManager.LoadScene("GameMain");
    }

    public void nextStage()
    {
        GM.Level++;

        for (int i = 0; i < GM.boardCol; i++)
            for (int j = 0; j < GM.boardRow; j++)
                Destroy(boardItem[i, j]);

        MakeLevel();
        winPanel.SetActive(false);

    }


}
