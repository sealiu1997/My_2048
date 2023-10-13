using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2Int coordinates;//当前cell的坐标
    public Tile tile;//绑定的tile



    public bool IsEmpty()//判断是否为空
    {
        return tile == null;
    }

    public bool IsOccupied()//判断是否有tile
    {
        return tile != null;
    }

    
}
