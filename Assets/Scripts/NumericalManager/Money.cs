using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///各项资源数值管理
public class Money : MonoBehaviour
{

    private int money;      //金钱 可负债

    public Text moneyText;  //UI显示面板

    public static Money Instance;    //对象实例 用于从外界调用该类的构造函数

    public int Numerical
    {
        get
        {
            return money;
        }
        set
        {
            money = value;
            //实时更新当前资源
            //PlayerPrefs.SetInt("Account", money);
            moneyText.text = money.ToString();
        }
    }

    //在新游戏开始时被调用
    void Awake()
    {
        Instance = this;

        ///初始化面板
        //如果是第一次进行游戏 则初始化金钱为100
        if (!PlayerPrefs.HasKey("Account"))
        {
            PlayerPrefs.SetInt("Account", 100);
        }
        //获取到上一局所得的金钱
        money = PlayerPrefs.GetInt("Account");
        moneyText.text = money.ToString();

        //moneyText.text = PlayerPrefs.GetInt("Account").ToString();
        //money = int.Parse(moneyText.text);
    }

}
