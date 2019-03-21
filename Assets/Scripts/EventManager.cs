using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///事件与组件管理
public class EventManager : MonoBehaviour
{
    public static EventManager Instance;   //供其他组件的脚本调用

    private GameManager gm;     //用于调用gamemanager脚本的方法

    #region 游戏对象
    public GameObject Construction;     //建造菜单
    public GameObject MessageBox;       //建筑说明面板
    public Text Message;                //说明文字
    public GameObject Confirm;          //建造确认

    public GameObject OptionsPanel;     //菜单面板
    public GameObject ButtonList;       //默认的按钮面板
    public GameObject RuleText;         //规则说明
    public GameObject IntroductionText; //单位说明
    #endregion

    void Awake()
    {
        //实例化自身
        Instance = this;

        //获取到gamemanager
        gm = FindObjectOfType<GameManager>();
    }

    #region 菜单功能
    //悔棋
    public void RegretBtn()
    {
        Debug.Log("悔棋");
    }
    //重新开始
    public void RestartBtn()
    {
        Debug.Log("重新开始");

        //如果已经获得了胜利 则保存分数
        if (gm.won)
        {
            PlayerPrefs.SetInt("Account", Money.Instance.Numerical);
        }

        Application.LoadLevel(Application.loadedLevel); //重新开始
    }
    //重置本关
    public void ResetBtn()
    {
        Debug.Log("重置");

        //清空一切资产
        PlayerPrefs.SetInt("Account", 100);

        Application.LoadLevel(Application.loadedLevel);
    }
    #endregion

    #region 菜单
    //菜单界面
    public void OptionsOn()
    {
        defaultStyle();                 //恢复菜单的默认状态

        OptionsPanel.SetActive(true);   //将菜单设为可见

        gm.State = GameState.GameSuspension;    //暂停接受输入
        //gm.over = false;
    }
    //关闭菜单
    public void OptionsOff()
    {
        OptionsPanel.SetActive(false);  //关闭菜单面板

        gm.State = GameState.Playing;   //重新接受输入
        //gm.over = true;
    }

    //菜单的默认样式
    private void defaultStyle()
    {
        RuleText.SetActive(false);          //关闭规则文字
        IntroductionText.SetActive(false);  //关闭说明文字
        ButtonList.SetActive(true);         //恢复菜单面板上按键的显示
    }

    //说明文字
    public void RuleOn()
    {
        RuleText.SetActive(true);       //显示说明文字
        ButtonList.SetActive(false);    //隐藏菜单面板上的按键
    }
    //介绍文字
    public void IntroductionOn()
    {
        IntroductionText.SetActive(true);
        ButtonList.SetActive(false);
    }

    //显示玩法
    public void TempRule()
    {
        defaultStyle();
        RuleOn();
        OptionsPanel.SetActive(true);
    }
    //单位说明
    public void TempIntro()
    {
        defaultStyle();
        IntroductionOn();
        OptionsPanel.SetActive(true);
    }
    #endregion

    #region 建造菜单
    //被点击的方块的坐标
    public int x;
    public int y;

    private Vector3 pos;     //菜单坐标

    private ElementType type;   //被建造的建筑
    private int MaterialPrice = 0;  //造价.建材
    private int MoneyPrice = 0;     //造价.金钱

    //建造菜单 并不直接通过按钮调用 而是【按钮调用tile脚本的函数后】再调用本函数发生事件
    public void ConstructionOn(Vector3 position)
    {
        TouchInputManager.Instance.isSwipe = false; //打断触点的移动状态

        //设为可见
        #region 处理面板溢出屏幕的情况
        //获取到自身的宽度和窗口的宽度
        float selfWidth = Construction.GetComponent<RectTransform>().sizeDelta.x * transform.localScale.x;  //自身宽度 * 缩放比
        //float selfHeight = Construction.GetComponent<RectTransform>().sizeDelta.y;

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
        #region 当触点发生在上半屏时 将面板位于触点下方显示
        //float selfHeight = Construction.GetComponent<RectTransform>().sizeDelta.y * transform.localScale.y;
        //if (position.y - selfHeight > Screen.height / 2)
        //{
        //    position.y -= selfHeight * 2;
        //}
        #endregion

        Construction.transform.position = pos = position;     //指定坐标跟随
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

        MessageBox.SetActive(false);    //关闭建筑的说明面板
        Construction.SetActive(false);  //关闭菜单
    }

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

    //说明面板 非按钮事件 在建造按钮被按下后触发 参数是按钮指向的建筑类型 以及显示坐标
    private void MessageBoxOn(ElementType type)
    {
        string s1 = ""; //名称
        string s2 = ""; //效果
        string s3 = "\n 建材：" + MaterialPrice + " , お金：" + MoneyPrice; //造价
        string s4 = ""; //可否建造

        switch (type)
        {
            case ElementType.Tower:
                #region 塔
                //载入建筑说明文字
                s1 = "<b> 攻撃タワー </b>";
                s2 = "\n 毎ターンは自身の左右上下直線３単位内の「敵」を一体攻撃します。攻撃の際、敵レベル×2の電力を消耗します、しかも、もし消耗していたら、残りの電力が20以下になってしまう場合は、攻撃しません";

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
                s1 = "<b> 地雷 </b>";
                s2 = "\n レベル８以下の「敵」に踏まれたら、自分と「敵」一緒に消滅します、レベル８以上の「敵」だったら、自分の消滅と共に、「敵」の動きを止め、そのレベルを半分にします、値段は10金ずつ増えます";

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
                s1 = "<b> 防御壁 </b>";
                s2 = "\n 壁は単純に全てのコマを移動を阻止するだけ、他のイベントはありません。";

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
                s1 = "<b> 発電所 </b>";
                s2 = "\n ボードに存在する発電所は、一個で毎ターン10点電力を提供します。ただし、「主」は移動していないターンは資源を提供しません。";

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
                s1 = "<b> 商店 </b>";
                s2 = "\n ボードに存在する商店は、一個で毎ターン５点お金を提供します。「主」は移動していないターンは資源を提供しません。";

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
        //设置信息文字
        Message.text = s1 + s2 + s3 + s4;

        //将面板设为可见
        MessageBox.transform.position = pos;    //坐标跟随
        MessageBox.SetActive(true);             //设为可见
    }
    //在取消建造菜单、或是建造成功后关闭面板

    //确认建造按钮
    public void ConfirmBtn()
    {
        MessageBox.SetActive(false);    //关闭说明面板
        ConstructionOff();              //关闭建造菜单

        //消耗资源
        Material.Instance.Numerical -= MaterialPrice;   //减去相应的建材
        Money.Instance.Numerical -= MoneyPrice;         //减去相应的金钱（可负债

        //根据类型调用建造函数
        gm.Generate(type, x, y);
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
