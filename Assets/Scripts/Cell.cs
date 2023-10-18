using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2Int coordinates;//当前cell的坐标
    public Tile tile;//绑定的tile



    public bool IsEmpty => tile == null;

    public bool IsOccupied=> tile != null;
    

    
}
