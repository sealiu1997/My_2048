using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float nowTime = 0;
    public float targetTime = 0;
    public bool Actived = false;
    public bool fistChange = false;
    public bool ItIsTime => (nowTime >= targetTime) && targetTime!= 0 ;

    public void SetTargetTime(float time)
    {
        targetTime = time;
    }

    public void IncreaseTime()
    {
        if (Actived && fistChange)
        {
            nowTime += Time.deltaTime;
        }
        
    }
}
