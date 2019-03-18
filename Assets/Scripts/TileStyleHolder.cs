using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]   //序列化
public class TileStyle
{
    public ElementType type;    //单位类型
    public Sprite image;        //单位样式
    //public int type;
    //public int level;       //合并等级
}

///定义方块的风格
public class TileStyleHolder : MonoBehaviour
{

    //单个方块
    public static TileStyleHolder Instance;

    //样式列表
    public TileStyle[] TileStyles;

    private void Awake()
    {
        //根据从Tile脚本传进来的值 决定方块的显示类型
        Instance = this;
    }
}
