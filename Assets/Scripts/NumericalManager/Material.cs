using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///各项资源数值管理
public class Material : MonoBehaviour {

    private int material;   //建材

    public static Material Instance;    //对象实例 用于从外界调用该类的构造函数

    //UI显示面板
    public Text materialText;

    public int Numerical
    {
        get
        {
            return material;
        }
        set
        {
            material = value;
            //实时更新当前资源
            materialText.text = material.ToString();
        }
    }

    //在新游戏开始时被调用
    void Awake()
    {
        Instance = this;

        ///初始化面板
        materialText.text = "0";    //初始建材为0
        material = int.Parse(materialText.text);
    }

}
