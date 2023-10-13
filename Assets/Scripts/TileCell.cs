using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int cooordinates;//当前cell的坐标
    public Tile tile;//关联的tile



    public bool IsEmpty()
    {
        return tile == null;
    }

    public bool IsOccupied()
    {
        return tile != null;
    }

    
}
