using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    [HideInInspector]
    public Cell cell;//绑定的cell
    [HideInInspector]
    public TileState state;//绑定的state
    [HideInInspector]
    public bool locked;//对tile操作锁
    public bool beMoved = false;//是否处于移动状态
    public Image backgoundImage;// 背景图片   
    public TextMeshProUGUI textNumber;//背景数字
    private Vector2 pivot;
    //[HideInInspector]
    public float playTime = 0.25f;//动画播放时间：需要在tilePrefab中更改

    private Tweener DTmove;
    private Tweener DTchangeColor;
    private Tweener DTdisapppearText;
    private Tweener DTappearText;


    private Tweener DTcreateTile1;
    private Tweener DTcreateTile2;
    private Tweener DTbeMergedTile1;
    private Tweener DTbeMergedTile2;
    private Tweener DTdestoryTile1;
    private Tweener DTdestoryTile2;



    public void Awake()//获得Image、textNumber对象
    {
        backgoundImage = GetComponent<Image>();
        textNumber = GetComponentInChildren<TextMeshProUGUI>();
        pivot = GetComponent<RectTransform>().pivot;
    }

    public void SetState(TileState state)//设定新tile的state并设定背景图片的相关属性信息
    {
        this.state = state;
     
        backgoundImage.color = state.backgroundcolor;
        textNumber.color = state.textcolor;
        textNumber.text = state.number.ToString();
       
    }

    //public void ChangeState(TileState state)//更改tile的state并更改背景图片的相关属性信息
    //{
        
    //    preText = this.state.number.ToString();
    //    this.state = state;
    //    afterText = state.number.ToString();


    //}

    public void LinkCellAndCoord(Cell cell)//链接绑定的cell并更改position到对应cell
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

        if (this.cell != null)
            {
                this.cell.tile = null;
            }


        this.cell = cell;

        this.cell.tile = this;

        beMoved = true;

        DoTweenMoveAnimator(cell.transform.position);
        //StartCoroutine(MoveAnimator(cell.transform.position));

    }

    public void MoveTo(Cell cell, bool Merge,TileState state)//当tile合并移动到一个cell时，tile更换绑定cell为新cell、播放移动动画
    {

        if (this.cell != null)
        {
            this.cell.tile = null;
        }


        this.cell = cell;

        this.cell.tile = this;

        beMoved = true;

        DoTweenMoveAnimator(cell.transform.position, Merge,state);
        //StartCoroutine(MoveAnimator(cell.transform.position));

    }

    


    public void MergeTo(Cell cell,TileState state, Vector2Int direction)//当tile移动到一个含有相同state的tile的cell时，升级自身并删除对应cell上的tile、播放移动动画
    {
        if (cell.IsOccupied)
        {
            DoTweenBeMergedTileAnimator(cell.tile,direction);

            //Destroy(cell.tile.gameObject);

            cell.tile = null;

            MoveTo(cell,true,state);
            


        }
        
    }

    public IEnumerator MoveAnimator(Vector3 to)//插值移动动画:更改beMoved状态
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

    private void DoTweenMoveAnimator(Vector3 to)
    {

        DTmove = transform.DOMove(to, playTime);
        DTmove.OnComplete(() => beMoved = false);

  

    }


    private void DoTweenMoveAnimator(Vector3 to,bool Merge,TileState state)
    {
        
        DTmove = transform.DOMove(to, playTime);
        DTmove.OnComplete(() => beMoved = false);

        if (Merge)
        {
            DoTweenMergeTileAnimator(state);
        }

    }


    public void DoTweenCreateTileAnimator()
    {
       
        var Alpha0Color = new Color(backgoundImage.color.r, backgoundImage.color.g, backgoundImage.color.b, 0f);
        var Alpha1Color = new Color(backgoundImage.color.r, backgoundImage.color.g, backgoundImage.color.b, 1f);

        DTcreateTile1 = DOTween.To(() => Alpha0Color, value => backgoundImage.color = value, Alpha1Color, playTime/2);//将tile的alpha通道值由0到1
        DTcreateTile2 = DOTween.To(() => new Vector3(0, 0, 0), value => transform.localScale = value, new Vector3(1, 1, 1), playTime/2);//将tile的Scale由0到1
    }

    public void DoTweenDestoryTileAnimator(Tile tile)
    {
        var Alpha0Color = new Color(backgoundImage.color.r, backgoundImage.color.g, backgoundImage.color.b, 0f);
        var Alpha1Color = new Color(backgoundImage.color.r, backgoundImage.color.g, backgoundImage.color.b, 1f);

        DTdestoryTile1 = DOTween.To(() => Alpha1Color, value => tile.backgoundImage.color = value, Alpha0Color, playTime);//将tile的alpha通道值由1到0
        DTdestoryTile2 = DOTween.To(() => new Vector3(1, 1, 1), value => tile.transform.localScale = value, new Vector3(0, 0, 0), playTime);//将tile的Scale由1到0

        DTdestoryTile1.OnComplete(()=> Destroy(tile.gameObject));
        
    }

    private void DoTweenBeMergedTileAnimator(Tile tile, Vector2Int direction)
    {
        var Alpha0Color = new Color(backgoundImage.color.r, backgoundImage.color.g, backgoundImage.color.b, 0f);
        var Alpha1Color = new Color(backgoundImage.color.r, backgoundImage.color.g, backgoundImage.color.b, 1f);
        Vector2 PrePivot = pivot;
        //Vector3 pivot = transform.pivot;
        


        DTbeMergedTile1 = DOTween.To(() => Alpha1Color, value => tile.backgoundImage.color = value, Alpha0Color, playTime);//将tile的alpha通道值由1到0

        if (direction == Vector2Int.up)
        {
            tile.pivot = new Vector2(0.5f, 1f);
            DTbeMergedTile2 = DOTween.To(() => new Vector3(1, 1, 1), value => tile.transform.localScale = value, new Vector3(1, 0, 1), playTime);
            
            
        }
        if (direction == Vector2Int.down)
        {
            tile.pivot = new Vector2(0.5f, 0f);
            DTbeMergedTile2 = DOTween.To(() => new Vector3(1, 1, 1), value => tile.transform.localScale = value, new Vector3(1, 0, 1), playTime);
            //DTbeMergedTile2.OnComplete(() => tile.pivot = PrePivot);
        }
        if (direction == Vector2Int.left)
        {
            tile.pivot = new Vector2(0f, 0.5f);
            DTbeMergedTile2 = DOTween.To(() => new Vector3(1, 1, 1), value => tile.transform.localScale = value, new Vector3(0, 1, 1), playTime);
            //DTbeMergedTile2.OnComplete(() => tile.pivot = PrePivot);
        }
        if (direction == Vector2Int.right)
        {
            tile.pivot = new Vector2(1f, 0.5f);
            DTbeMergedTile2 = DOTween.To(() => new Vector3(1, 1, 1), value => tile.transform.localScale = value, new Vector3(0, 1, 1), playTime);
            //DTbeMergedTile2.OnComplete(() => tile.pivot = PrePivot);
        }

        //DTdestoryTile2 = DOTween.To(() => new Vector3(1, 1, 1), value => tile.transform.localScale = value, new Vector3(0, 0, 0), playTime);//将tile的Scale由1到0

        DTbeMergedTile1.OnComplete(() => Destroy(tile.gameObject));

    }

    private void DoTweenMergeTileAnimator(TileState state)
    {
        
        this.state = state;
      

        DTchangeColor = backgoundImage.DOColor(state.backgroundcolor, playTime);
        DTchangeColor = textNumber.DOColor(state.textcolor, playTime);

        ;

        DTdisapppearText = textNumber.DOFade(0, playTime / 2);
        DTappearText = textNumber.DOFade(1, playTime / 2);

        

        DTappearText.Pause();
        DTdisapppearText.OnComplete(() =>
        {
            textNumber.text = state.number.ToString();
            DTappearText.PlayForward();
        }
        );


        
    }
}
