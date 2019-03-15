using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///单个方块管理
public class Tile : MonoBehaviour {

    public bool mergedThisTurn = false;     //判断本回合是否已经进行过合并

    //在游戏管理器中存储关于所有图块的信息
    public int indRow;  //该对象的坐标
    public int indCol;  //从0，0到10，10

    //构造函数 每当引用该类的实例被调用时就会执行
    public ElementType TileType
    {
        //返回当前标识数字
        get
        {
            return tileType;
        }
        //更新标识数字
        set
        {
            tileType = value;
            UpdateTile();       //更新显示样式
        }
    }

    private ElementType tileType;       //方块的单位类型id
    private Image TileImage;            //方块的样式
    public int TileLevel = 0;           //方块的等级
    private Text TileText;              //方块的数字
    public bool isMove = false;         //记录方块是否发生过移动

    void Awake()
    {
        //获得到方块的Image和Text组件
        TileImage = transform.Find("TileBreedCell").GetComponent<Image>();
        TileText = GetComponentInChildren<Text>();
    }

    //更新方块的显示样式
    public void UpdateTile()
    {
        //如果标识数字为0 则方块为空
        if (tileType == ElementType.Empty)
        {
            SetEmpty();     //关闭
        }
        //否则根据标识数字来修改方块的样式
        else
        {
            ApplyStyle(tileType);   //确定样式
            SetVisible();           //启用
        }
    }

    //样式修改 以单位类型id作为参数 对其样式进行修改
    void ApplyStyleFromHolder(int index)
    {
        TileImage.sprite = TileStyleHolder.Instance.TileStyles[index].image;    //更新单位的样式

        //如果这是一个有等级的单位
        if (TileLevel > 0)
        {
            TileText.text = TileLevel.ToString();    //更新单位的等级
        }
        
    }

    //样式判断 以单位类型id作为参数 调用函数对方块的样式进行修改
    void ApplyStyle(ElementType type)
    {
        switch (type)
        {
            case ElementType.Player:        //主角
                TileLevel = 0;  //这是一个没有等级的单位
                ApplyStyleFromHolder(0);
                break;
            case ElementType.Enemy:         //敌人
                ApplyStyleFromHolder(1);
                break;
            case ElementType.Material:      //建材
                ApplyStyleFromHolder(2);
                break;
            case ElementType.Tower:         //攻击塔
                TileLevel = 0;
                ApplyStyleFromHolder(3);
                break;
            case ElementType.Power:         //发电站
                TileLevel = 0;
                ApplyStyleFromHolder(4);
                break;
            case ElementType.Mall:          //商店
                TileLevel = 0;
                ApplyStyleFromHolder(5);
                break;
            case ElementType.Wall:          //防御墙
                TileLevel = 0;
                ApplyStyleFromHolder(6);
                break;
            default:
                Debug.LogError("Check the numbers that you pass to ApplyStyle!");
                break;
        }
        #region 源码备用
        //switch (type)
        //{
        //    case 1:     //主角
        //        ApplyStyleFromHolder(0);
        //        break;
        //    case 2:     //敌人
        //        ApplyStyleFromHolder(1);
        //        break;
        //    case 3:     //建材
        //        ApplyStyleFromHolder(2);
        //        break;
        //    case 6:     //攻击塔
        //        ApplyStyleFromHolder(3);
        //        break;
        //    case 7:     //发电站
        //        ApplyStyleFromHolder(4);
        //        break;
        //    case 8:     //商店
        //        ApplyStyleFromHolder(5);
        //        break;
        //    case 9:     //防御墙
        //        ApplyStyleFromHolder(6);
        //        break;
        //    default:
        //        Debug.LogError("Check the numbers that you pass to ApplyStyle!");
        //        break;
        //}
        #endregion
    }

    //设为可见
    private void SetVisible()
    {
        TileImage.enabled = true;   //设置图片样式为可见

        if (TileLevel > 0)
        {
            TileText.enabled = true;    //如果是个带等级的单位 则将其的等级也设为可见
        }
        else
        {
            TileText.enabled = false;
        }
    }

    //设置为空
    private void SetEmpty()
    {
        //若方块内容为空则不显示任何东西
        TileImage.enabled = false;
        TileText.enabled = false;
    }

}
