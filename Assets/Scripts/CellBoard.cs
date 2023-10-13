using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBoard : MonoBehaviour
{
    public GameManager gameManager;//关联的game manager
    public Tile tilePrefab;//tile预制体
    public TileState[] tileStates;//保存tilestate数组

    public CellGrid cellGrid;//关联cellgrid
    public List<Tile> tiles;//游戏中存在的tile List
    private bool waiting;//操作等待标识

    public void ClearBoard()//清除所有cell上绑定的tile、删除所有之前创建的tile
    {

    }

    public void CreateTile()//创建tile：初始化tile并设置其状态、放到一个随机的空位置
    {
        

    }

    private void Move()//移动及规则函数
    {
        
    }

    private bool MoveTile()
    {
        return true;
    }

    private bool CanMerge(Tile a,Tile b)
    {
        return true;
    }

    private void MergeTile(Tile a, Tile b)
    {

    }

    public bool CheckForGameOver()
    {
        return true;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()//处理玩家的操作，根据input keycode映射移动方向
    {
        
    }
}
