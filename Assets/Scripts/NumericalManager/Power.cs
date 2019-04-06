using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///各项资源数值管理
public class Power : MonoBehaviour
{

    private int power;      //电力 <0时游戏失败

    public Text powerText;  //UI显示面板

    public static Power Instance;    //对象实例 用于从外界调用该类的构造函数

    public int Numerical
    {
        get
        {
            return power;
        }
        set
        {
            power = value;
            //实时更新当前资源
            powerText.text = power.ToString();
        }
    }

    //在新游戏开始时被调用
    void Awake()
    {
        Instance = this;

        ///初始化面板
        power = 100;        //初始电力为100
        powerText.text = power.ToString();

        //powerText.text = "100";      //初始电力为100
        //power = int.Parse(powerText.text);
    }

}
