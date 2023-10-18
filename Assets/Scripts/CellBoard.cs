using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBoard : MonoBehaviour
{
    public GameManager gameManager;//关联的game manager
    public Tile tilePrefab;//tile预制体
    public TileStateList tileStates;//保存tilestate数组

    public CellGrid cellGrid;//关联cellgrid
    public List<Tile> tiles;//游戏中存在的tile List
    private bool waiting = false;//操作等待标识
    private int MAXSTATENUM = 16;
    private bool change = false;


    public void ClearBoard()//清除所有cell上绑定的tile、删除所有之前创建的tile
    {
        foreach (var cell in cellGrid.AllCells)
        {
            cell.tile = null;
        }

        foreach(var tile in tiles)
        {
            Destroy(tile.gameObject);
        }
        tiles.Clear();
    }

    public void CreateTile()//创建tile：初始化tile并设置其状态、放到一个随机的空位置
    {

        Tile tile = Instantiate(tilePrefab, cellGrid.transform);
        //print("1");
        tile.SetState(tileStates.tileStatesList[0]);
        //print("2");
        tile.ChangeCellAndCoord(cellGrid.GetRandomEmptyCell());
        //print("3");
        tiles.Add(tile);
       // print("4");
    }

    private void Move(Vector2Int direction,int startX,int xStepLength,int startY,int yStepLength)//移动及规则函数
    {
        for (int x = startX; x>=0 && x < cellGrid.width; x+=xStepLength)
        {
            for (int y = startY; y >=0 && y < cellGrid.height; y+=yStepLength)
            {
                Vector2Int coord = new Vector2Int(x,y);
                //print("6");
                Cell cell = cellGrid.GetCell(coord);
                //print("7");
                if (cell.IsOccupied)
                {
                    //print("1");
                    MoveTile(cell.tile,direction);
                }
            }
        }
        
        StartCoroutine(WaitForInput());
    }

    private void MoveTile(Tile tile,Vector2Int direction)
    {
        Cell cell = tile.cell;
        Cell nearCell = cellGrid.GetDirectionNerborCell(cell,direction);

        while (nearCell != null)
        {
            if (nearCell.IsOccupied)
            {
                print("12");
                if (CanMerge(cell.tile, nearCell.tile))
                {
                    
                    MergeTile(cell.tile, nearCell.tile);
                }
                break;
            }
            else
            {
                if (nearCell.IsEmpty)
                {

                    change = true;
                    tile.MoveTo(nearCell);
                }
                cell = tile.cell;//nearcell
                nearCell = cellGrid.GetDirectionNerborCell(cell, direction);
            }
        }

       
    }

    private bool CanMerge(Tile a,Tile b)
    {
        if (a.state == b.state && b.locked==false)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    private void MergeTile(Tile a, Tile b)
    {
        change = true;

        int index =tileStates.tileStatesList.IndexOf(a.state) + 1;
        if (index >MAXSTATENUM) { index = MAXSTATENUM; }
        TileState newState = tileStates.tileStatesList[index];
        tiles.Remove(b);
        a.MergeTo(b.cell);
        a.locked = true;
  
        a.SetState(newState);
        gameManager.InCreaseScore(newState.number);
        
    }

    private void UnLockedTile()
    {
        foreach(var tile in tiles)
        {
            tile.locked = false;
        }
    }

    public bool CheckForGameOver()
    {
        if (tiles.Count != cellGrid.size)
        {
            return false;
        }

        foreach(var tile in tiles)
        {
            Cell up = cellGrid.GetDirectionNerborCell(tile.cell, Vector2Int.up);
            Cell down = cellGrid.GetDirectionNerborCell(tile.cell, Vector2Int.down);
            Cell left = cellGrid.GetDirectionNerborCell(tile.cell, Vector2Int.left);
            Cell right = cellGrid.GetDirectionNerborCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile))
            {
                return false;
            }
            if (down != null && CanMerge(tile, down.tile))
            {
                return false;
            }
            if (left != null && CanMerge(tile, left.tile))
            {
                return false;
            }
            if (right != null && CanMerge(tile, right.tile))
            {
                return false;
            }
        }

        print("game over!");
        return true;
    }

    private IEnumerator WaitForInput()
    {
        waiting = true;
        yield return new WaitForSeconds(0.4f);

        UnLockedTile();

        if (tiles.Count != cellGrid.size && change)
        {
            print("0");
            CreateTile();
        }
        if (CheckForGameOver())
        {
            gameManager.GameOver();
        }
        change = false;
        waiting = false;
    }

    private void Awake()
    {
        cellGrid = GetComponentInChildren<CellGrid>();
        


        tiles = new List<Tile>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()//处理玩家的操作，根据input keycode映射移动方向
    {
        
        if (!waiting)
        {
            if (Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow))
            {
                Move(Vector2Int.up,0,1,1,1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                Move(Vector2Int.down,0,1,cellGrid.height-2,-1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Move(Vector2Int.left,1,1,0,1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                Move(Vector2Int.right,cellGrid.width-2,-1,0,1);
            }
        }
    }
}
