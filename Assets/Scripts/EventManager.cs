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

    //重新游戏
    public void RestartBtn()
    {
        Debug.Log("重新游戏");
        Application.LoadLevel(Application.loadedLevel);
    }

    #region 建造菜单面板
    //被点击的方块的坐标
    public int x;
    public int y;

    //并不直接通过按钮调用 而是【按钮调用tile脚本的函数后】再调用本函数发生事件
    public void ConstructionOn(Vector3 position)
    {
        //控制台输出方块的（x,y）
        //string s = x + "，" + y;
        //Debug.Log(s);

        Construction.SetActive(true);       //显示面板

        #region 处理面板溢出屏幕的情况
        //获取到自身的宽度和窗口的宽度
        float selfWidth = Construction.GetComponent<RectTransform>().sizeDelta.x;
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
        Construction.SetActive(false);
    }
    #endregion

    #region 建造
    //建造事件
    public void Tower()
    {
        //Debug.Log("攻击塔");

        int MaterialPrice = 2 * 8;  //造价.建材 = 2的n倍
        int MoneyPrice = 10 * 5;    //造价.金钱 = 10的n倍
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
        ConstructionOff();      //关闭建造菜单
    }
    public void Power()
    {
        //Debug.Log("发电站");

        int MaterialPrice = 2 * 2;
        int MoneyPrice = 10 * 2;
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

        int MaterialPrice = 2 * 4;
        int MoneyPrice = 10 * 3;
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
    public void Wall()
    {
        //Debug.Log("防御墙");

        int MaterialPrice = 2 * 1;
        int MoneyPrice = 10 * 1;
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
        Debug.Log("地雷");

        int MaterialPrice = 2 * 0;
        int MoneyPrice = 10 * 1;
        if (Money.Instance.Numerical > 10)
        {
            //gm.Generate(ElementType.Landmine);
            if (gm.Generate(ElementType.Landmine, x, y))
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
        //将菜单设为可见
        OptionsPanel.SetActive(true);

        //暂停接受输入
        //gm.State = GameState.GameSuspension;
        gm.over = false;
    }
    //关闭菜单
    public void OptionsOff()
    {
        //恢复菜单的默认状态
        RuleText.SetActive(false);          //关闭规则文字
        IntroductionText.SetActive(false);  //关闭说明文字
        ButtonList.SetActive(true);         //恢复菜单面板上按键的显示

        //关闭菜单面板
        OptionsPanel.SetActive(false);

        //重新接受输入
        //gm.State = GameState.Playing;
        gm.over = true;
    }

    //说明文字
    public void ruleOn()
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
    #endregion

}
