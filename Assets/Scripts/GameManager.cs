using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public CellBoard board;//关联的board
    public CanvasGroup gameOver;//关联的gameover画布
    public TextMeshProUGUI txtScore;//当前得分
    public TextMeshProUGUI bestScore;//最佳得分
    public float time = 20;//限时模式时间

    private int gameMode = 0;//游戏模式


    private int score = 0;//初始化分数
    private Tweener gameover;


    public void ReStartGame()
    {
        if (gameMode == 0)
        {
            StartOrdinaryMode();
        }
        if (gameMode == 1)
        {
            
            StartTimeLimitedMode();
        }
    }


    public void NewGame()//开始游戏：还原分数、加载最高分数、隐藏游戏结束面板、初始化核心逻辑
    {
        SetScore(0);

        gameOver.alpha = 0f;
        gameOver.interactable = false;

        bestScore.text = LoadBestScore().ToString();//加载最高分数

        board.ReSetLimitedTimeMode();
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()//结束游戏：关闭board、保存分数、显示游戏结束面板
    {
        
        board.enabled = false;

        

        gameover = gameOver.DOFade(1, 1f);
        gameover.OnComplete(()=> gameOver.interactable = true);
        
        PlayerPrefs.Save();//保存分数
    }

    public void StartOrdinaryMode()
    {
        gameMode = 0;
        NewGame();
        //board.TimeLimitedMode(time);
    }


    public void StartTimeLimitedMode()
    {
        gameMode = 1;
        NewGame();
        board.TimeLimitedMode(time);
    }


    public void InCreaseScore(int Points)//增加分数
    {
        SetScore(score + Points);
    }

    public void SaveBestScore()//保存最佳历史分数
    {
        int HistoryScore = LoadBestScore();
        if (score > HistoryScore)
        {
            PlayerPrefs.SetInt("HistoryScore", score);
        }
    }

    private void SetScore(int num)//设定分数
    {
        this.score = num;
        txtScore.text = score.ToString();
        //保存分数
        SaveBestScore();

    }

    public int LoadBestScore()//加载最佳历史分数
    {
        return PlayerPrefs.GetInt("HistoryScore",0);
    }



    // Start is called before the first frame update
    void Start()
    {
        NewGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
