using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    //[HideInInspector]
    public Cell cell;//绑定的cell
    //[HideInInspector]
    public TileState state;//绑定的state
    [HideInInspector]
    public bool locked;//对tile操作锁
    public bool beMoved = false;//是否处于移动状态
    public Image backgoundImage;// 背景图片   
    public TextMeshProUGUI textNumber;//背景数字


    public void Awake()//获得Image、textNumber对象
    {
        backgoundImage = GetComponent<Image>();
        textNumber = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetState(TileState state)//更改tile的state并更改背景图片的相关属性信息
    {
        this.state = state;
        backgoundImage.color = state.backgroundcolor;
        textNumber.color = state.textcolor;
        textNumber.text = state.number.ToString();
    }

    public void ChangeCellAndCoord(Cell cell)//更改绑定的cell并更改position
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        this.transform.position = cell.transform.position;
    }

    public void MoveTo(Cell cell)//当tile移动到一个空cell时，tile更换绑定cell为新cell、播放移动动画
    {
        if (cell.IsEmpty)
        {
            if (this.cell != null)
            {
                this.cell.tile = null;
            }

            this.cell = cell;
            this.cell.tile = this;

            
            StartCoroutine(MoveAnimator(cell.transform.position));
            
        }
    }

    public void MergeTo(Cell cell)//当tile移动到一个含有相同state的tile的cell时，升级自身并删除对应cell上的tile、播放移动动画
    {
        if (cell.IsOccupied)
        {
            Destroy(cell.tile.gameObject);
            cell.tile = null;

            if (this.cell != null)
            {
                this.cell.tile = null;
            }

            this.cell = cell;
            this.cell.tile = this;

            
            StartCoroutine(MoveAnimator(cell.transform.position));



        }
        
    }

    public IEnumerator MoveAnimator(Vector3 to)//移动动画
    {
        float elapsed = 0f;
        float duration = 0.1f;
        Vector3 from = transform.position;
        beMoved = true;
        
        
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = to;
        beMoved = false;
        
  
    }
}
