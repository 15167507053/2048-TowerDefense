using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///事件与组件管理
public class EventManager : MonoBehaviour
{

    public static EventManager Instance;   //供其他组件的脚本调用

    private GameManager gm;     //用于调用gamemanager脚本的方法

    #region 游戏对象
    public GameObject Construction;     //建造菜单
    public GameObject MessageBox;       //建筑说明面板

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

    #region 建造菜单面板
    //被点击的方块的坐标
    public int x;
    public int y;

    //并不直接通过按钮调用 而是【按钮调用tile脚本的函数后】再调用本函数发生事件
    public void ConstructionOn(Vector3 position)
    {
        TouchInputManager.Instance.isSwipe = false; //打断触点的移动状态

        //控制台输出方块的（x,y）
        //string s = x + "，" + y;
        //Debug.Log(s);

        Construction.SetActive(true);       //显示面板

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

        Construction.transform.position = position;     //指定坐标跟随
    }
    public void ConstructionOff()
    {
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

    //建筑说明面板 非按钮事件 在建造按钮被按下后触发 参数是按钮指向的建筑类型
    private void MessageBoxOn(ElementType type)
    {
        switch (type)
        {
            case ElementType.Tower:
                //载入建筑说明文字
                //载入建造价格 和 判断并显示是否可建造
                //若不可建造 隐藏建造按钮
                //显示面板
                break;
            case ElementType.Landmine:
                break;
            case ElementType.Wall:
                break;
            case ElementType.Power:
                break;
            case ElementType.Mall:
                break;
        }
        MessageBox.SetActive(true);     //设为可见
        //在取消建造菜单、或是建造成功后关闭面板
    }

    //确认建造按钮
    public void Confirm()
    {

    }

    #endregion

    #region 建造
    //建造事件
    public void Tower()
    {
        //Debug.Log("攻击塔");

        int premium = gm.CountOff(ElementType.Tower) * 2;   //系数 造一座塔涨价 [x*2]
        int MaterialPrice = 2 * 8;            //造价.建材 = 2的n倍 + 加价
        int MoneyPrice = 10 * 5 + premium;    //造价.金钱 = 10的n倍 + 加价

        //如果有足够的建材进行建造
        if (Material.Instance.Numerical - MaterialPrice >= 0)
        {
            //生成相应建筑
            //gm.Generate(ElementType.Tower);
            if (gm.Generate(ElementType.Tower, x, y))
            {
                //仅在建造成功时消耗资源
                Consume(MaterialPrice, MoneyPrice);
            }
            else
            {
                enough(1);
            }
        }
        else
        {
            enough(2);
        }
        //关闭建造菜单
        ConstructionOff();
    }
    public void Wall()
    {
        //Debug.Log("防御墙");

        int MaterialPrice = 2 * 1;
        int MoneyPrice = 10 * 0;

        if (Material.Instance.Numerical - MaterialPrice >= 0)
        {
            //gm.Generate(ElementType.Wall);
            if (gm.Generate(ElementType.Wall, x, y))
            {
                Consume(MaterialPrice, MoneyPrice);
            }
            else
            {
                enough(1);
            }
        }
        else
        {
            enough(2);
        }
        ConstructionOff();
    }
    public void Landmine()
    {
        //Debug.Log("地雷");

        int premium = 5 * gm.count;    //每造一个雷贵10块
        int MaterialPrice = 2 * 0;
        int MoneyPrice = 10 * 1 + premium;

        if (Money.Instance.Numerical >= 10)
        {
            //gm.Generate(ElementType.Landmine);
            if (gm.Generate(ElementType.Landmine, x, y))
            {
                Consume(MaterialPrice, MoneyPrice);
                gm.count++;
            }
            else
            {
                enough(1);
            }
        }
        else
        {
            enough(2);
        }
        ConstructionOff();
    }

    public void Power()
    {
        //Debug.Log("发电站");

        int premium = gm.CountOff(ElementType.Power);
        if (premium < 5)
        {
            premium = 0;
        }
        else
        {
            premium -= 3;
        }
        int MaterialPrice = 2 * 2 + premium;
        int MoneyPrice = 10 * 1 + premium;

        if (Material.Instance.Numerical - MaterialPrice >= 0)
        {
            //gm.Generate(ElementType.Power);
            if (gm.Generate(ElementType.Power, x, y))
            {
                Consume(MaterialPrice, MoneyPrice);
            }
            else
            {
                enough(1);
            }
        }
        else
        {
            enough(2);
        }
        ConstructionOff();
    }
    public void Mall()
    {
        //Debug.Log("商场");

        int premium = gm.CountOff(ElementType.Mall);
        if (premium < 3)
        {
            premium = 0;
        }
        else
        {
            premium -= 2;
        }
        int MaterialPrice = 2 * 3 + premium;
        int MoneyPrice = 10 * 2 + premium;

        if (Material.Instance.Numerical - MaterialPrice >= 0)
        {
            //gm.Generate(ElementType.Mall);
            if (gm.Generate(ElementType.Mall, x, y))
            {
                Consume(MaterialPrice, MoneyPrice);
            }
            else
            {
                enough(1);
            }
        }
        else
        {
            enough(2);
        }
        ConstructionOff();
    }

    //消耗资源
    private void Consume(int MaterialPrice, int MoneyPrice)
    {
        Material.Instance.Numerical -= MaterialPrice;   //减去相应的建材
        Money.Instance.Numerical -= MoneyPrice;         //减去相应的金钱（可负债
    }
    //造价不足时的提示信息
    private void enough(int m)
    {
        switch (m)
        {
            case 1:
                Debug.Log("建造失败");
                break;
            case 2:
                Debug.Log("造价不足");
                break;
        }
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

}
