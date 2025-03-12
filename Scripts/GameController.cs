using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public Block[,] boardItem;
    public int[,] boardValue;
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
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                StartCoroutine(MoveTiles(-1, 0));
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                StartCoroutine(MoveTiles(1, 0));
            }
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                StartCoroutine(MoveTiles(0, 1));
            }
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                StartCoroutine(MoveTiles(0, -1));
            }
        }
        if (state == GameState.Move)
        {
            
        }

        if (checkSuccess())
        {
            state = GameState.Win;
        }


        if (tap.TapEnd())
            state = GameState.WaitChoose;

    }

    public void MakeLevel()
    {
        levelInfo = levelSetting.Levels[GM.Level - 1];
        GM.boardCol = levelInfo.Column;
        GM.boardRow = levelInfo.Row;
        boardItem = new Block[GM.boardCol, GM.boardRow];
        boardValue = new int[GM.boardCol, GM.boardRow];


        for (int i = 0; i < GM.boardCol; i++)
            for (int j = 0; j < GM.boardRow; j++)
            {
                GameObject blockObject = Instantiate(levelSetting.PiecePrefabs[0], new Vector3(i * 2f ,j * 2f), Quaternion.Euler(0, 0, 0));
                blockObject.name += "_" + i + "_" + j;
                blockObject.transform.localScale = Vector3.one * 1.5f;
                blockObject.transform.Find("sprite").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                boardItem[i, j] = blockObject.GetComponent<Block>();
                boardItem[i, j].Coordinates = new Vector2(i, j);
                boardItem[i, j].State = Block.BlockState.Empty;
                boardItem[i, j].Type = Block.BlockType.Empty;

            }
        int firstValue = 2;
        int secondValue = 4;
        int firstRand = Random.Range(0, GM.boardCol * GM.boardRow);
        int secondRand = 0;
        if (firstRand > (GM.boardCol * GM.boardRow) / 2)
        {
            secondRand = Random.Range(0, firstRand);
        }
        else
        {
            secondRand = Random.Range(firstRand, GM.boardCol * GM.boardRow);
        }
        Debug.Log("First:" + firstRand + ", Sencond " + secondRand);
        int firstX = firstRand % GM.boardCol;
        int firstY = firstRand / GM.boardCol;
        int secondX = secondRand % GM.boardCol;
        int secondY = secondRand / GM.boardCol;
        Debug.Log("Index: " + firstX + "," + firstY + " = " + firstValue + " ; " + secondX + "," + secondY +" = " + secondValue);

        boardValue[firstX, firstY] = firstValue;
        boardValue[secondX, secondY] = secondValue;
        boardItem[firstX, firstY].SetValue(firstValue);
        boardItem[secondX, secondY].SetValue(secondValue);

        state = GameState.WaitChoose;

    }

    IEnumerator MoveTiles(int offsetX, int offsetY)
    {
        //Left
        bool existMove = true;
        int count = 0;
        while (existMove || count > 20)
        {
            existMove = false;
            count++;
            //Move Left
            if (offsetX < 0)
            {
                for (int i = 1; i < GM.boardCol; i++)
                {
                    existMove = existMove || moveHorizontal(i, i - 1);
                }
                    
            }
            //Move Right
            else if (offsetX > 0)
            {
                for (int i = GM.boardCol - 2; i >= 0; i--)
                {
                    existMove = existMove || moveHorizontal(i, i + 1);
                }
            }
            //Move Down
            else if (offsetY < 0)
            {
                for (int j = 1; j < GM.boardRow; j++)
                {
                    existMove = existMove || moveVertical(j, j - 1);
                }
            }
            //Move Up
            else
            {
                for (int j = GM.boardRow - 2; j >= 0; j--)
                {
                    existMove = existMove || moveVertical(j, j + 1);
                }
            }
            //yield return null;
            yield return new WaitForSeconds(0.02f);
        }
        Debug.Log("End with Count:" + count);
        GenerateNew();
    }
    bool moveHorizontal(int i, int iNext)
    {
        bool existMove = false;
        for (int j = 0; j < GM.boardRow; j++)
        {
            if (boardValue[i, j] > 0)
            {
                if (boardValue[iNext, j] <= 0)
                {
                    Debug.Log("board{" + iNext + ";" + j + "} get board{" + i + ";" + j + "} with" + boardValue[i, j]);
                    boardValue[iNext, j] = boardValue[i, j];
                    boardValue[i, j] = 0;
                    boardItem[i, j].SetValue(0);
                    boardItem[iNext, j].SetValue(boardValue[iNext, j]);
                    existMove = true;
                }
                else if (boardValue[i,j] == boardValue[iNext, j])
                {
                    boardValue[iNext, j] = boardValue[i, j] * 2;
                    boardValue[i, j] = 0;
                    boardItem[i, j].SetValue(0);
                    boardItem[iNext, j].SetValue(boardValue[iNext, j]);
                    existMove = true;
                }
            }
        }
        return existMove;
    }
    bool moveVertical(int j, int jNext)
    {
        bool existMove = false;
        for (int i = 0; i < GM.boardCol; i++)
        {
            if (boardValue[i, j] > 0)
            {
                if (boardValue[i, jNext] <= 0)
                {
                    Debug.Log("board{" + i + ";" + jNext + "} get board{" + i + ";" + j + "} with" + boardValue[i, j]);
                    boardValue[i, jNext] = boardValue[i, j];
                    boardValue[i, j] = 0;
                    boardItem[i, j].SetValue(0);
                    boardItem[i, jNext].SetValue(boardValue[i, jNext]);
                    existMove = true;
                }
                else if (boardValue[i,j] == boardValue[i,jNext])
                {
                    boardValue[i, jNext] = boardValue[i, j] * 2;
                    boardValue[i, j] = 0;
                    boardItem[i, j].SetValue(0);
                    boardItem[i, jNext].SetValue(boardValue[i, jNext]);
                    existMove = true;
                }
            }
        }
        return existMove;
    }
    void GenerateNew()
    {
        List<(int,int)> listEmpty= new List<(int, int)>();
        for (int i = 0; i < GM.boardCol; i++)
            for (int j = 0; j < GM.boardRow; j++)
                if (boardValue[i, j] == 0)
                {
                    listEmpty.Add((i, j));
                }
        int newValue = 2;
        (int,int) newRand = listEmpty[Random.Range(0, listEmpty.Count)];
        int x = newRand.Item1;
        int y = newRand.Item2;
        Debug.Log("Index: " + x + "," + y + " = " + newValue);

        boardValue[x, y] = newValue;
        boardItem[x, y].SetValue(newValue);

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
