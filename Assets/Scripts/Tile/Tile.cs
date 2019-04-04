using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///单个方块管理
public class Tile : MonoBehaviour
{
    //构造函数 每当引用该类的实例被调用时就会执行
    public ElementType TileType
    {
        //返回当前类型
        get
        {
            return tileType;
        }
        //更新类型
        set
        {
            tileType = value;
            UpdateTile();       //更新显示样式
        }
    }

    private GameManager gm;

    #region 自身变量
    private ElementType tileType;   //方块的单位类型
    private Image TileImage;        //子Image的样式

    private Text TileText;      //方块的数字
    public int TileLevel = 0;   //方块的等级

    public bool moveThisTurn = false;       //记录方块是否发生过移动
    public bool mergedThisTurn = false;     //判断本回合是否已经进行过合并
    public bool SlowBuff = false;           //是否被减速

    //在游戏管理器中存储关于所有图块的信息 该对象的坐标
    public int indRow;  //从0,0到7,7
    public int indCol;  //0,0到10,10
    #endregion

    void Awake()
    {
        gm = FindObjectOfType<GameManager>();

        //获得到方块的Image和Text组件
        TileImage = transform.Find("TileBreedCell").GetComponent<Image>();
        TileText = GetComponentInChildren<Text>();
    }

    #region 修改样式
    //更新方块的显示样式
    public void UpdateTile()
    {
        //如果类型为空 则不显示内容
        if (tileType == ElementType.Empty)
        {
            SetEmpty();     //关闭
        }
        //否则根据类型来修改方块的样式
        else
        {
            ApplyStyle(tileType);   //确定样式
            SetVisible();           //启用
        }
    }

    //样式修改 以单位类型作为参数 对其样式进行修改
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
                //这是一个没有等级的单位
                TileLevel = 0;
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
            case ElementType.Landmine:      //地雷
                TileLevel = 0;
                ApplyStyleFromHolder(7);
                break;
            case ElementType.Trap:          //陷阱
                ApplyStyleFromHolder(8);
                break;
            case ElementType.Refuge:        //避难所
                ApplyStyleFromHolder(9);
                break;
            case ElementType.Magnetic:      //干扰磁场
                ApplyStyleFromHolder(10);
                break;
            case ElementType.TowerEnemy:    //远程型敌人
                ApplyStyleFromHolder(11);
                break;
            case ElementType.BuilderEnemy:  //造墙型敌人
                ApplyStyleFromHolder(12);
                break;
            case ElementType.AssistedEnemy: //支援型敌人
                ApplyStyleFromHolder(13);
                break;
            case ElementType.Access:
                ApplyStyleFromHolder(14);
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
    #endregion

    #region 设为可见
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
    private void SetEmpty()
    {
        //若方块内容为空则不显示任何东西
        TileImage.enabled = false;
        TileText.enabled = false;
    }
    #endregion

    //自身的按钮事件
    public void Tilebtn()
    {
        //如果建造菜单处于开启状态/游戏处于不可进行状态 不进行传值
        if (gm.State == GameState.Playing)
        {
            //获取到自身坐标并传值
            EventManager.Instance.x = indCol;
            EventManager.Instance.y = indRow;
        }

        //若自身为空 则开启建造菜单
        if (TileType == ElementType.Empty)
        {
            if (EventManager.Instance.Construction.activeSelf)
            {
                //如果菜单已经处于开启状态 将其关闭
                EventManager.Instance.ConstructionOff();
            }
            else
            {
                //TileImage.sprite = Resources.Load<Sprite>("Select");      //将自身样式改为【选中】

                //更改样式为选中（通过禁用自身
                this.GetComponent<Button>().interactable = false;

                EventManager.Instance.ConstructionOn(transform.position);    //显示建造面板
            }
        }
        //若不为空 则显示内容介绍
        else
        {
            EventManager.Instance.other(TileType);
        }
    }

}
