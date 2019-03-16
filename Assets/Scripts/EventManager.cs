using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///事件与组件管理
public class EventManager : MonoBehaviour {

    public static EventManager Instance;   //供其他组件的脚本调用

    private GameManager gm;     //用于调用gamemanager脚本的方法

    public GameObject Construction;     //建造菜单

    public GameObject OptionsPanel;     //菜单面板
    public GameObject ButtonList;       //默认的按钮面板
    public GameObject RuleText;         //规则说明
    public GameObject IntroductionText; //单位说明

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

    #region 方块按钮事件
    //参数是面板显示的坐标 和需要显示的文字（x，y）
    //并不直接通过按钮调用 而是【按钮调用tile脚本的函数后】再调用本函数发生事件
    public void ConstructionOn(Vector3 position, string s)
    {
        Construction.SetActive(true);   //显示面板
        Construction.transform.position = position;   //指定坐标跟随
        Debug.Log(s);   //控制台输出方块的（x,y）
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
        int MaterialPrice = 2 * 5;  //造价.建材 = 2的n倍
        int MoneyPrice = 10 * 2;    //造价.金钱 = 10的n倍
        //如果有足够的建材进行建造
        if (Material.Instance.Numerical - MaterialPrice >= 0)
        {
            Material.Instance.Numerical -= MaterialPrice;   //减去相应的建材
            Money.Instance.Numerical -= MoneyPrice;         //减去相应的金钱（可负债
            gm.Generate(ElementType.Tower);                 //生成相应建筑

            //Debug.Log("攻击塔");
        }
        else
        {
            enough();
        }
    }
    public void Power()
    {
        int MaterialPrice = 2 * 2;
        int MoneyPrice = 10 * 2;
        if (Material.Instance.Numerical - MaterialPrice >= 0)
        {
            Material.Instance.Numerical -= MaterialPrice;
            Money.Instance.Numerical -= MoneyPrice;
            gm.Generate(ElementType.Power);

            //Debug.Log("发电站");
        }
        else
        {
            enough();
        }
    }
    public void Mall()
    {
        int MaterialPrice = 2 * 4;
        int MoneyPrice = 10 * 2;
        if (Material.Instance.Numerical - MaterialPrice >= 0)
        {
            Material.Instance.Numerical -= MaterialPrice;
            Money.Instance.Numerical -= MoneyPrice;
            gm.Generate(ElementType.Mall);

            //Debug.Log("商场");
        }
        else
        {
            enough();
        }
    }
    public void Wall()
    {
        int MaterialPrice = 2 * 3;
        int MoneyPrice = 10 * 2;
        if (Material.Instance.Numerical - MaterialPrice >= 0)
        {
            Material.Instance.Numerical -= MaterialPrice;
            Money.Instance.Numerical -= MoneyPrice;
            gm.Generate(ElementType.Wall);

            //Debug.Log("防御墙");
        }
        else
        {
            enough();
        }
    }

    //造价不足时的提示信息
    private void enough()
    {
        Debug.Log("造价不足");
    }
    #endregion

    #region 菜单
    //菜单界面
    public void OptionsOn()
    {
        //将菜单设为可见
        OptionsPanel.SetActive(true);
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
