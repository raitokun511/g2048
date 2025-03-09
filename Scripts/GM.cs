using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
    public static float unit;


    public static int Level = 1;
    public static int boardCol = 10;
    public static int boardRow = 10;
    public static int GameMode;


    public static void Init()
    {
        unit = Screen.width / 10f;
        GameController.MOVE_DELTA_THRESHOLD = 0.07f;// Screen.width / 6f;


        Debug.Log("Init - unit:" + unit + " - threshold:" + GameController.MOVE_DELTA_THRESHOLD);
    }
}
