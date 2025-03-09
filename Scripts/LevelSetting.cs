using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetting : MonoBehaviour
{
    public static LevelSetting instance;
    public List<LevelConstructor> Levels;
    public GameObject[] PieceRootPrefabs;
    public GameObject[] PiecePrefabs;
    public Sprite dotSprite;
    public Sprite dotlineSprite;
    public Sprite fulllineSprite;
    public Sprite twolineSprite;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public GameObject InitBlock(Vector3 position)
    {
        int iPrefab = (int)position.z % 100;
        int irPrefab = (int)position.z % 100;
        Vector3 blockPos = new Vector3(position.x, position.y, 0);
        if ((int)position.z / 100 > 0)
        {
            Debug.Log("Init " + position.x + "  " + position.y + "  " + position.z + "  " + iPrefab + "  " + PiecePrefabs[iPrefab].name);
            GameObject rpiece = Instantiate(PieceRootPrefabs[irPrefab], blockPos, PieceRootPrefabs[irPrefab].transform.rotation);
            rpiece.GetComponent<Block>().Coordinates = new Vector2(position.x, position.y);
            return rpiece;
        }
        GameObject piece = Instantiate(PiecePrefabs[iPrefab], blockPos, PiecePrefabs[iPrefab].transform.rotation);
        piece.GetComponent<Block>().Coordinates = new Vector2(position.x, position.y);
        return piece;
    }

    public Color getColor(int id)
    {
        switch (id)
        {
            case 1: return Color.red;
            case 2: return Color.green;
            case 3: return Color.yellow;
            case 4: return Color.blue;
            case 5: return Color.magenta;
            default: return Color.white;
        }
    }
}
