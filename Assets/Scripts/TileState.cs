using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Tile State")]
public class TileState : ScriptableObject
{
    public Color backgroundcolor;// 背景颜色    
    public Color textcolor;//文字颜色
    public int number;//数字
}
