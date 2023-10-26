using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct StepMap
{
    public int score;
    public int[,] map;

    public StepMap(int height, int width)
    {
        score = 0;
        map = new int[height, width];
    }
};

public class HistoryStack : MonoBehaviour
{

    
    public Stack<StepMap> historyStack = new Stack<StepMap>();

    public StepMap InitStepMap(int height, int width)
    {
        return new StepMap(height, width);
    }

}
