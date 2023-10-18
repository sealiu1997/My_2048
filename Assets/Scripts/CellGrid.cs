using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid : MonoBehaviour
{
    public CellRow[] cellRows;//含有cell的Row数组
    public Cell[] AllCells;//含有所有cell的数组
    
    public int size;//cell数量
    public int height;//行高即行数
    public int width;//列长即列数
public void SetAllCellsCoordinates()//给所有cell赋值坐标
    {
        for(int i=0 ;i<cellRows.Length ;i++ )
        {
            for(int j = 0; j < cellRows[i].cells.Length ;j++)
            {
                cellRows[i].cells[j].coordinates = new Vector2Int(j,i);//遍历的顺序是行列，而坐标（x，y）的标识顺序是：列、行
            }
        }
    }

public Cell GetCell(Vector2Int coord)//根据坐标获取任一cell
    {

        if (coord.x >= 0 && coord.x < width && coord.y >= 0 && coord.y < height)
        {
            //print("5");
            return cellRows[coord.y].cells[coord.x];
            
        }
        else
        {
            //print("6");
            return null;
        }

    }

public Cell GetDirectionNerborCell(Cell cell,Vector2Int direction)//根据方向返回cell该方向的邻居
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x +=direction.x;
        coordinates.y -=direction.y;

        return GetCell(coordinates);
    }



public Cell GetRandomEmptyCell()//在空cell List中返回任一空cell
    {
        List<Cell> emptyCells = new List<Cell>();//空cell List
        foreach (var cell in AllCells)
        {
            if (cell.IsEmpty)
            {
                emptyCells.Add(cell);
            }
        }
        if (emptyCells.Count>0)
        {
            return emptyCells[Random.Range(0,emptyCells.Count-1)];
        }
        return null;
    }

    public void Awake()//初始化row数组和allcells数组，获得行高和列长
    {
        cellRows = GetComponentsInChildren<CellRow>();
        AllCells = GetComponentsInChildren<Cell>();
        size = AllCells.Length;
        width = cellRows.Length;
        height = size / cellRows.Length;
    }

    public void Start()//初始化所有cell坐标，统计空cell
    {
        SetAllCellsCoordinates();

        //FindEmptyCells();
    }
}
