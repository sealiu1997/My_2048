using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


public class CellBoard : MonoBehaviour
{
    public GameManager gameManager;//关联的game manager
    public Tile tilePrefab;//tile预制体
    public TileStateList tileStates;//保存tilestate数组

    public CellGrid cellGrid;//关联cellgrid
    public List<Tile> tiles;//游戏中存在的tile List
    public Timer timer;
    public HistoryStack stackManager;

    public bool backState = false;
    //public bool firstBack = true;
    private bool waiting = false;//操作等待标识
    private int MAXSTATENUM = 16;//state最大序列号
    private bool change = false;//tile移动或合并标识位
    private StepMap stepMap;
    

    


    //public static int Count = 0;


    public void ClearBoard()//清除所有cell上绑定的tile、删除所有之前创建的tile
    {
     
        foreach (var cell in cellGrid.AllCells)
        {
            cell.tile = null;
        }

        foreach(var tile in tiles)
        {

            tile.DoTweenDestoryTileAnimator(tile);

            //Destroy(tile.gameObject);
        }
        tiles.Clear();
    }

    public bool StackIsEmpty()
    {
        if (stackManager.historyStack.Count > 0)
        {
            return false;
        }
        return true;
    }


    public void RefreshBoard()
    {


        if (stackManager.historyStack.Count == 1)
            {
                stepMap = stackManager.historyStack.Peek();
            }
            else
            {
                stackManager.historyStack.Pop();
                stepMap = stackManager.historyStack.Pop();
            }

        gameManager.SetScore(stepMap.score);

        for (int i = 0; i < cellGrid.height; i++)
        {
            for (int j = 0; j < cellGrid.width; j++)
            {
                if (stepMap.map[i, j]!= 0 )
                {
                    CreateTile(new Vector2Int(j,i),stepMap.map[i, j]);
                }
                
            }
        }
        CollectStepMapInformation();
    }

    public void CollectStepMapInformation()
    {
        stepMap = stackManager.InitStepMap(cellGrid.height, cellGrid.width);
        for (int i = 0; i < cellGrid.height; i++)
        {
            for (int j = 0; j < cellGrid.width; j++)
            {
                if (cellGrid.GetCell(new Vector2Int(j, i)).IsOccupied)
                {
                    stepMap.map[i, j] = cellGrid.GetCell(new Vector2Int(j, i)).tile.state.number;
                }
                else
                {
                    stepMap.map[i, j] = 0;
                }

            }
        }
        stepMap.score = gameManager.ReturnNowScore();

        stackManager.historyStack.Push(stepMap);
        //firstBack = true;

    }


    public int ReturnRandomNum()
    {
        Random r = new Random(unchecked((int)DateTime.Now.Ticks));
        int n = r.Next(1, 100);
        return (n >= 90 ? 1 : 0);
    }

    public void CreateTile()//创建tile：初始化tile并设置其状态、放到一个随机的空位置
    {
        

        Tile tile = Instantiate(tilePrefab, cellGrid.transform);
        
        tile.SetState(tileStates.tileStatesList[ReturnRandomNum()]);

        Cell RandomCell = cellGrid.GetRandomEmptyCell();

        tile.LinkCellAndCoord(RandomCell);
       
        tiles.Add(tile);
        tile.DoTweenCreateTileAnimator();
       
    }

    public void CreateTile(Vector2Int coord,int stateNum)//创建tile：初始化tile并设置其状态为stateNum对应状态
    {


        Tile tile = Instantiate(tilePrefab, cellGrid.transform);


        int Num = (int)Math.Log(stateNum, 2) - 1;
        tile.SetState(tileStates.tileStatesList[Num]);

        Cell cell = cellGrid.GetCell(coord);

        tile.LinkCellAndCoord(cell);

        tiles.Add(tile);
        tile.DoTweenCreateTileAnimator();

    }

    private void MoveLevel(Vector2Int direction,int startX,int xStepLength,int startY,int yStepLength)//遍历所有cell，判断是否含有tile、是否需要移动
    {
        for (int x = startX; x>=0 && x < cellGrid.width; x+=xStepLength)
        {
            for (int y = startY; y >=0 && y < cellGrid.height; y+=yStepLength)
            {
                Vector2Int coord = new Vector2Int(x,y);
                
                Cell cell = cellGrid.GetCell(coord);
                
                if (cell.IsOccupied)
                {

                    //StartCoroutine(JudgeMoveTile(cell.tile, direction));
                    //MoveTile(cell.tile,direction);
                    change |= MoveTileLoop(cell.tile, direction);
                    if (timer.fistChange == false)
                    {
                        timer.fistChange = change;
                    }
                }
            }
        }
        waiting = true;
        //if (change)
        //{
        //    CollectStepMapInformation();
        //}
    }

    private void MoveVertical(Vector2Int direction, int startX, int xStepLength, int startY, int yStepLength)//遍历所有cell，判断是否含有tile、是否需要移动
    {
        for (int y = startY; y >= 0 && y < cellGrid.height; y += yStepLength)
        {
            for (int x = startX; x >= 0 && x < cellGrid.width; x += xStepLength)
            {
                Vector2Int coord = new Vector2Int(x, y);

                Cell cell = cellGrid.GetCell(coord);

                if (cell.IsOccupied)
                {

                    //StartCoroutine(JudgeMoveTile(cell.tile, direction));
                    //MoveTile(cell.tile,direction);
                    change |= MoveTileLoop(cell.tile, direction);
                    if (timer.fistChange == false)
                    {
                        timer.fistChange = change;
                    }
                }
            }

        }
        waiting = true;
        //if (change)
        //{
        //    CollectStepMapInformation();
        //}
    }


    

    private bool MoveTileLoop(Tile tile, Vector2Int direction)//移动判定函数

    {
        Cell cell = null;
        Cell nearCell = cellGrid.GetDirectionNerborCell(tile.cell, direction);

        while (nearCell != null)
        {
            if (nearCell.IsOccupied)
            {

                if (CanMerge(tile, nearCell.tile))
                {

                    MergeTile(tile, nearCell.tile, direction);
                    return true;
                }
                break;
            }
            
                cell = nearCell;//nearcell
                nearCell = cellGrid.GetDirectionNerborCell(nearCell, direction);
            
        }
        if (cell != null)
        {
            tile.MoveTo(cell);
            return true;
        }

        return false;

    }


    


    private bool CanMerge(Tile a,Tile b)//判定两个tile是否能合并
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

    private void MergeTile(Tile a, Tile b, Vector2Int direction)//tile合并操作函数
    {
        change = true;

        
        TileState newState = ReturnHighState(a);

        tiles.Remove(b);
        a.MergeTo(b.cell,newState,direction);
        a.locked = true;
  
        
        gameManager.InCreaseScore(newState.number);
        
    }

    private TileState ReturnHighState(Tile tile)
    {
        int index = tileStates.tileStatesList.IndexOf(tile.state) + 1;
        if (index > MAXSTATENUM) { index = MAXSTATENUM; }
       
        return tileStates.tileStatesList[index];
    }

    public void TimeLimitedMode(float time)
    {
        timer.SetTargetTime(time);
        timer.Actived = true;

    }
    public void ReSetLimitedTimeMode()
    {
        timer.Actived = false;
        timer.fistChange = false;
        timer.nowTime = 0;
    }

    public int GetRemainedTime()
    {
        if(timer.Actived && timer.fistChange)
        {
            return (int)(timer.targetTime - timer.nowTime);
        }
        return 0;
    }


    private void UnLockedTile()//解除所有tile锁
    {
        foreach(var tile in tiles)
        {
            tile.locked = false;
        }
    }

    private bool CheckTileExistMove()//检查是否存在tile处于beMove状态
    {
        bool existMove = false;
        foreach(var tile in tiles)
        {
            if (tile.beMoved)
            {
                existMove = true;
            }
        }
        return existMove;
    }

    public bool CheckForGameOver()//检查是否符合游戏结束条件
    {
        if(timer.Actived && timer.ItIsTime)//限时模式逻辑
        {
            
            return true;
        }

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

   




    private void Awake()//获取cellgird
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
        
        if (!waiting && !backState)
        {
            if (Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveVertical(Vector2Int.up,0,1,1,1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveVertical(Vector2Int.down,0,1,cellGrid.height-2,-1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveLevel(Vector2Int.left,1,1,0,1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveLevel(Vector2Int.right,cellGrid.width-2,-1,0,1);
            }
        }

        timer.IncreaseTime();
        



    }

    private void LateUpdate()
    {
        if (!CheckTileExistMove() && !backState)
        {
            UnLockedTile();

            if (tiles.Count != cellGrid.size && change)
            {

                CreateTile();
                CollectStepMapInformation();
            }
            if (CheckForGameOver())
            {
                gameManager.GameOver();
                timer.Actived = false;
            }
            change = false;
            waiting = false;
            
        }
    }
}
