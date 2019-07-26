using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitPoint : MonoBehaviour
{
    private int HP;      //血量 每被攻击一次-1 <=0时游戏失败
    public Text hpText;

    public int Numerical
    {
        get
        {
            return HP;
        }
        set
        {
            HP = value;
            hpText.text = HP.ToString();
        }
    }

    public static HitPoint Instance;

    //在新游戏开始时被调用
    void Awake()
    {
        Instance = this;

        ///初始化面板
        HP = 5;
        hpText.text = HP.ToString();
    }

}
