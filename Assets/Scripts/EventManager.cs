using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///事件与组件管理
public class EventManager : MonoBehaviour
{
    public static EventManager Instance;   //供其他组件的脚本调用

    private GameManager gm;     //用于调用gamemanager脚本的方法

    public GameObject Tile;             //用于测距
    public GameObject Construction;     //建造菜单
    public GameObject MessageBox;       //建筑说明面板
    public GameObject Confirm;          //建造确认
    public Text Message;                //说明文字
    //public GameObject MessageCloseBtn;     //关闭说明面板

    void Awake()
    {
        //实例化自身
        Instance = this;

        //获取到gamemanager
        gm = FindObjectOfType<GameManager>();
    }

    //被点击的方块的坐标
    public int x;
    public int y;

    private Vector3 MenuPos;    //菜单坐标
    private Vector3 MsgPos;     //信息窗坐标
    private bool isAdjustment = false;  //是否已经发生过调整

    private ElementType type;       //被建造的建筑
    private int MaterialPrice = 0;  //造价.建材
    private int MoneyPrice = 0;     //造价.金钱

    #region 建造菜单
    //建造菜单 并不直接通过按钮调用 而是【按钮调用tile脚本的函数后】再调用本函数发生事件
    public void ConstructionOn(Vector3 position)
    {
        TouchInputManager.Instance.isSwipe = false; //打断触点的移动状态

        #region 处理面板溢出屏幕的情况
        float selfWidth = Construction.GetComponent<RectTransform>().sizeDelta.x * transform.localScale.x;  //自身宽度 * 缩放比

        //左侧溢出 0 + 宽/2
        if (position.x - selfWidth / 2 < 0)
        {
            position.x = selfWidth / 2;
        }
        //右侧溢出 max - 宽/2
        else if (position.x + selfWidth / 2 > Screen.width)
        {
            position.x = Screen.width - selfWidth / 2;
        }
        #endregion
        MenuPos = MsgPos = position;    //保存当前坐标 并赋值给信息窗
        #region 设定菜单出现的位置
        float selfHeight = (Tile.GetComponent<RectTransform>().sizeDelta.y) * transform.localScale.y;   //获取一个格子的高度
        //如果触点在上半屏 (下6上5
        if (position.y - selfHeight > Screen.height / 2)
        {
            position.y -= selfHeight;   //将面板的坐标向下移动一个单位的距离
            //divide = true;
        }
        //如果在下半屏
        else
        {
            position.y += selfHeight;   //将面板的坐标向上移动一个单位的距离
        }
        #endregion

        //设为可见
        Construction.transform.position = position;     //指定坐标跟随
        Construction.SetActive(true);       //显示面板
    }
    public void ConstructionOff()
    {
        //恢复按钮的样式（通过启用按钮
        gm.AllTiles[y, x].GetComponent<Button>().interactable = true;

        //短时间内不接受移动输入操作
        //float timer = Time.time;    //关闭菜单的时间
        //while (Time.time - timer < 0.5)
        //{
        //    gm.State = GameState.Playing;
        //    break;
        //}

        gm.State = GameState.Playing;   //将游戏状态恢复到可以操作

        MessageClose();                 //关闭建筑的说明面板
        Construction.SetActive(false);  //关闭菜单
    }

    //说明面板 非按钮事件 在建造按钮被按下后触发 参数是按钮指向的建筑类型 以及显示坐标
    private void MessageBoxOn(ElementType type)
    {
        #region 设置信息文字
        //string s1 = ""; //名称
        string s2 = ""; //说明
        string s3 = "\n 建材：" + MaterialPrice + " , お金：" + MoneyPrice; //造价
        string s4 = ""; //可否建造

        switch (type)
        {
            case ElementType.Tower:
                #region 塔
                //载入建筑说明文字
                //s1 = "<b> 攻撃タワー </b>";
                s2 = document(ElementType.Tower);

                //判断是否可建造
                if (Material.Instance.Numerical - MaterialPrice < 0)
                {
                    s4 = "<color=#ff0000>\n 建材不足</color>";
                    //隐藏建造按钮
                    Confirm.SetActive(false);
                }
                //else if (true)
                //{
                //    s4 = "<color=#ff0000>\\nn 借金になります</color>";
                //}
                else
                {
                    s4 = "<color=#00ff00>\n 建造可能</color>";
                    //显示建造按钮
                    Confirm.SetActive(true);
                }
                #endregion
                break;

            case ElementType.Landmine:
                #region 地
                //s1 = "<b> 地雷 </b>";
                s2 = document(ElementType.Landmine);

                if (Money.Instance.Numerical - MoneyPrice < 0)
                {
                    s4 = "<color=#ff0000>\n お金不足</color>";
                    Confirm.SetActive(false);
                }
                else
                {
                    s4 = "<color=#00ff00>\n 建造可能</color>";
                    Confirm.SetActive(true);
                }
                #endregion
                break;

            case ElementType.Wall:
                #region 墙
                //s1 = "<b> 防御壁 </b>";
                s2 = document(ElementType.Wall);

                if (Material.Instance.Numerical - MaterialPrice < 0)
                {
                    s4 = "<color=#ff0000>\n 建材不足</color>";
                    Confirm.SetActive(false);
                }
                else
                {
                    s4 = "<color=#00ff00>\n 建造可能</color>";
                    Confirm.SetActive(true);
                }
                #endregion
                break;

            case ElementType.Power:
                #region 电
                //s1 = "<b> 発電所 </b>";
                s2 = document(ElementType.Power);

                if (Material.Instance.Numerical - MaterialPrice < 0)
                {
                    s4 = "<color=#ff0000>\n 建材不足</color>";
                    Confirm.SetActive(false);
                }
                else
                {
                    s4 = "<color=#00ff00>\n 建造可能</color>";
                    Confirm.SetActive(true);
                }
                #endregion
                break;

            case ElementType.Mall:
                #region 商
                //s1 = "<b> 商店 </b>";
                s2 = document(ElementType.Mall);

                if (Material.Instance.Numerical - MaterialPrice < 0)
                {
                    s4 = "<color=#ff0000>\n 建材不足</color>";
                    Confirm.SetActive(false);
                }
                else
                {
                    s4 = "<color=#00ff00>\n 建造可能</color>";
                    Confirm.SetActive(true);
                }
                #endregion
                break;
        }
        //Message.text = s1 + s2 + s3 + s4;
        Message.text = s2 + s3 + s4;
        #endregion

        #region 设定信息面板的位置
        //pos = Construction.transform.position;      //首先先恢复pos的位置
        float selfHeight = MessageBox.GetComponent<RectTransform>().sizeDelta.y * transform.localScale.y;   //获取自身的高度
        float tileHeight = Tile.GetComponent<RectTransform>().sizeDelta.y * transform.localScale.y;         //格子的高度
        float MenuHeight = Construction.GetComponent<RectTransform>().sizeDelta.y * transform.localScale.y; //菜单的高度

        //仅在第一次触发时修正坐标的位置
        if (!isAdjustment)
        {
            //如果在上半屏 使其出现在下方 判断坐标 = pos - 单位高度/2 - 菜单高度
            if (MsgPos.y > Screen.height / 2)
            {
                MsgPos.y = (MsgPos.y - tileHeight / 2 - MenuHeight) - selfHeight / 2;
            }
            //如果在下半屏 使其出现在上方
            else
            {
                //设定坐标 = pos（菜单坐标） + 方块高度/2 + 菜单高度 + 信息窗高度/2
                MsgPos.y = (MsgPos.y + tileHeight / 2 + MenuHeight) + selfHeight / 2;
            }
            isAdjustment = true;
        }

        //处理下侧溢 (纵坐标-高度/2 < 0
        if (MsgPos.y - selfHeight / 2 < 0)
        {
            MsgPos.y = selfHeight / 2;
        }
        //上侧溢出(纵坐标+高度/2 > 屏幕top
        else if (MsgPos.y + selfHeight / 2 > Screen.height)
        {
            MsgPos.y = Screen.height - selfHeight / 2;
        }
        #endregion

        //将面板设为可见
        MessageBox.transform.position = MsgPos;    //坐标跟随
        MessageBox.SetActive(true);             //设为可见
    }
    //在取消建造菜单、或是建造成功后关闭面板
    public void MessageClose()
    {
        //MessageCloseBtn.SetActive(false);     //隐藏关闭按钮
        MessageBox.SetActive(false);    //关闭面板
        MsgPos = MenuPos;               //恢复坐标的位置
        isAdjustment = false;           //恢复为没有调整过坐标的状态

        //如果建造菜单还处于打开状态 则不回复可造作状态
        if (!Construction.active)
        {
            gm.State = GameState.Playing;   //恢复可以接受操作的状态
        }

    }

    //确认建造按钮
    public void ConfirmBtn()
    {
        MessageClose();    //关闭说明面板
        ConstructionOff();              //关闭建造菜单

        //消耗资源
        Material.Instance.Numerical -= MaterialPrice;   //减去相应的建材
        Money.Instance.Numerical -= MoneyPrice;         //减去相应的金钱（可负债

        //根据类型调用建造函数
        gm.Generate(type, x, y);
    }
    #endregion

    #region 各个建筑按钮
    public void Tower()
    {
        //定义造价
        int premium = gm.CountOff(ElementType.Tower) * 2;   //系数 造一座塔涨价 [x*2]
        MaterialPrice = 2 * 8;            //造价.建材 = 2的n倍 + 加价
        MoneyPrice = 10 * 5 + premium;    //造价.金钱 = 10的n倍 + 加价

        //触发信息面板
        type = ElementType.Tower;
        MessageBoxOn(ElementType.Tower);
    }
    public void Wall()
    {
        MaterialPrice = 2 * 1;
        MoneyPrice = 10 * 0;

        type = ElementType.Wall;
        MessageBoxOn(ElementType.Wall);
    }
    public void Landmine()
    {
        int premium = 10 * gm.count;    //每造一个雷贵10块
        MaterialPrice = 2 * 0;
        MoneyPrice = 10 * 1 + premium;

        type = ElementType.Landmine;
        MessageBoxOn(ElementType.Landmine);
    }
    public void Power()
    {
        int premium = gm.CountOff(ElementType.Power);
        if (premium < 3)
        {
            premium = 0;    //如果数量小于3个 不加价
        }
        else
        {
            premium -= 3;   //大于3个后加数量-3的价格
        }
        MaterialPrice = 2 * 2 + premium;
        MoneyPrice = 10 * 1 + premium;

        type = ElementType.Power;
        MessageBoxOn(ElementType.Power);
    }
    public void Mall()
    {
        int premium = gm.CountOff(ElementType.Mall);
        if (premium < 3)
        {
            premium = 0;
        }
        else
        {
            premium -= 2;
        }
        MaterialPrice = 2 * 3 + premium;
        MoneyPrice = 10 * 2 + premium;

        type = ElementType.Mall;
        MessageBoxOn(ElementType.Mall);
    }
    #endregion

    #region 说明文档
    private string document(ElementType type)
    {
        string s = "";  //说明文字
        switch (type)
        {
            case ElementType.Player:
                s = "<b>主人公</b>\n ボード上に移動しながら、「建」を取集する。「敵」に突き当たられたらGameOverになります";
                break;

            case ElementType.Material:
                s = "<b>建材</b>\n これを取集するのはゲームのメイン内容です。同レベルの同類が接触したら、レベルを2倍にして合併します。「主」と「建」が接触したら、その「建」レベル分の点数をもらえる。その用途は【資源の説明】一節で説明します。ゲーム開始の時ボード上ランダムの位置に2個がいて、その後毎ターン１個が出てきます";
                break;

            case ElementType.Enemy:
                s = "<b>敵</b>\n 同レベルの同類が接触したら、レベルを2倍にして合併します。壁以外のコマに突き当たたら、それを壊します。ゲーム開始の3ターン後ランダムの位置に1体出現、その後毎５ターン1体を出現します";
                break;

            case ElementType.Power:
                s = "<b> 発電所 </b>\n ボードに存在する発電所は、一個で毎ターン10点電力を提供します。ただし、「主」は移動していないターンは資源を提供しません";
                break;

            case ElementType.Mall:
                s = "<b> 商店 </b>\n ボードに存在する商店は、一個で毎ターン５点お金を提供します。「主」は移動していないターンは資源を提供しません";
                break;

            case ElementType.Wall:
                s = "<b> 防御壁 </b>\n 壁は単純に全てのコマを移動を阻止するだけ、他のイベントはありません";
                break;

            case ElementType.Landmine:
                s = "<b> 地雷 </b>\n レベル８以下の「敵」に踏まれたら、自分と「敵」一緒に消滅します、レベル８以上の「敵」だったら、自分の消滅と共に、「敵」の動きを止め、そのレベルを半分にします、値段は10金ずつ増えます";
                break;

            case ElementType.Tower:
                s = "<b> 攻撃タワー </b>\n 毎ターンは自身の左右上下直線３単位内の「敵」を一体攻撃します。攻撃の際、敵レベル×2の電力を消耗します、しかも、もし消耗していたら、残りの電力が20以下になってしまう場合は、攻撃しません";
                break;

            default:
                s = "まだ準備出来ていない";
                break;
        }
        return s;
    }

    //在非空方块被点击时 显示说明文件
    public void other(ElementType type)
    {
        ConstructionOff();      //如果建造菜单被打开 将其关闭
        gm.State = GameState.GameSuspension;    //暂停接受输入

        //显示面板
        string s = document(type);  //获得介绍文字
        Message.text = s;           //更新文字
        MessageBox.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);    //指定面板的位置（屏幕中心）
        MessageBox.SetActive(true); //显示信息面板
        Confirm.SetActive(false);   //隐藏确认按钮
        //MessageCloseBtn.SetActive(true); //显示关闭按钮
    }
    #endregion

    #region 建造（源码备份
    //建造事件
    //public void Tower()
    //{
    //    //Debug.Log("攻击塔");

    //    int premium = gm.CountOff(ElementType.Tower) * 2;   //系数 造一座塔涨价 [x*2]
    //    int MaterialPrice = 2 * 8;            //造价.建材 = 2的n倍 + 加价
    //    int MoneyPrice = 10 * 5 + premium;    //造价.金钱 = 10的n倍 + 加价

    //    //如果有足够的建材进行建造
    //    if (Material.Instance.Numerical - MaterialPrice >= 0)
    //    {
    //        //生成相应建筑
    //        //gm.Generate(ElementType.Tower);
    //        if (gm.Generate(ElementType.Tower, x, y))
    //        {
    //            //仅在建造成功时消耗资源
    //            Consume(MaterialPrice, MoneyPrice);
    //        }
    //        else
    //        {
    //            enough(1);
    //        }
    //    }
    //    else
    //    {
    //        enough(2);
    //    }
    //    //关闭建造菜单
    //    ConstructionOff();
    //}
    //public void Wall()
    //{
    //    //Debug.Log("防御墙");

    //    int MaterialPrice = 2 * 1;
    //    int MoneyPrice = 10 * 0;

    //    if (Material.Instance.Numerical - MaterialPrice >= 0)
    //    {
    //        //gm.Generate(ElementType.Wall);
    //        if (gm.Generate(ElementType.Wall, x, y))
    //        {
    //            Consume(MaterialPrice, MoneyPrice);
    //        }
    //        else
    //        {
    //            enough(1);
    //        }
    //    }
    //    else
    //    {
    //        enough(2);
    //    }
    //    ConstructionOff();
    //}
    //public void Landmine()
    //{
    //    //Debug.Log("地雷");

    //    int premium = 5 * gm.count;    //每造一个雷贵10块
    //    int MaterialPrice = 2 * 0;
    //    int MoneyPrice = 10 * 1 + premium;

    //    if (Money.Instance.Numerical >= 10)
    //    {
    //        //gm.Generate(ElementType.Landmine);
    //        if (gm.Generate(ElementType.Landmine, x, y))
    //        {
    //            Consume(MaterialPrice, MoneyPrice);
    //            gm.count++;
    //        }
    //        else
    //        {
    //            enough(1);
    //        }
    //    }
    //    else
    //    {
    //        enough(2);
    //    }
    //    ConstructionOff();
    //}

    //public void Power()
    //{
    //    //Debug.Log("发电站");

    //    int premium = gm.CountOff(ElementType.Power);
    //    if (premium < 5)
    //    {
    //        premium = 0;
    //    }
    //    else
    //    {
    //        premium -= 3;
    //    }
    //    int MaterialPrice = 2 * 2 + premium;
    //    int MoneyPrice = 10 * 1 + premium;

    //    if (Material.Instance.Numerical - MaterialPrice >= 0)
    //    {
    //        //gm.Generate(ElementType.Power);
    //        if (gm.Generate(ElementType.Power, x, y))
    //        {
    //            Consume(MaterialPrice, MoneyPrice);
    //        }
    //        else
    //        {
    //            enough(1);
    //        }
    //    }
    //    else
    //    {
    //        enough(2);
    //    }
    //    ConstructionOff();
    //}
    //public void Mall()
    //{
    //    //Debug.Log("商场");

    //    int premium = gm.CountOff(ElementType.Mall);
    //    if (premium < 3)
    //    {
    //        premium = 0;
    //    }
    //    else
    //    {
    //        premium -= 2;
    //    }
    //    int MaterialPrice = 2 * 3 + premium;
    //    int MoneyPrice = 10 * 2 + premium;

    //    if (Material.Instance.Numerical - MaterialPrice >= 0)
    //    {
    //        //gm.Generate(ElementType.Mall);
    //        if (gm.Generate(ElementType.Mall, x, y))
    //        {
    //            Consume(MaterialPrice, MoneyPrice);
    //        }
    //        else
    //        {
    //            enough(1);
    //        }
    //    }
    //    else
    //    {
    //        enough(2);
    //    }
    //    ConstructionOff();
    //}

    //消耗资源
    //private void Consume(int MaterialPrice, int MoneyPrice)
    //{
    //    Material.Instance.Numerical -= MaterialPrice;   //减去相应的建材
    //    Money.Instance.Numerical -= MoneyPrice;         //减去相应的金钱（可负债
    //}
    //造价不足时的提示信息
    //private void enough(int m)
    //{
    //    switch (m)
    //    {
    //        case 1:
    //            Debug.Log("建造失败");
    //            break;
    //        case 2:
    //            Debug.Log("造价不足");
    //            break;
    //    }
    //}
    #endregion

}
