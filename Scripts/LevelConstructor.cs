using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level Creation")]
public class LevelConstructor : ScriptableObject
{
    public int Row;
    public int Column;
    public List<Vector3> Pieces;
    
}
