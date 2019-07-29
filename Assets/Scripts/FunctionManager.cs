using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunctionManager : MonoBehaviour
{
    private GameManager gm;     //用于调用gamemanager脚本的方法  

    void Awake()
    {
        //获取到gamemanager
        gm = FindObjectOfType<GameManager>();
    }

    #region 2.建造一个指定类型的单位
    //【随机位置建造】 参数是需要建造的单位类型
    public void Generate(ElementType type)
    {
        //若场上仍然存在空方快
        if (gm.EmptyTiles.Count > 0)
        {
            //随机一个位置
            int PosIndex = Random.Range(0, gm.EmptyTiles.Count);

            //如果这个位置已经有东西了 则不进行建造
            if (gm.EmptyTiles[PosIndex].TileType != ElementType.Empty)
            {
                gm.EmptyTiles.RemoveAt(PosIndex);  //删除这个位置
                Generate(type);                 //重新触发建造
            }
            else
            {
                //如果这是敌人或建材单位 赋予其等级
                if (type == ElementType.Enemy || type == ElementType.Material)
                {
                    gm.EmptyTiles[PosIndex].TileLevel = 2;
                }
                //其他情况清空等级
                else
                {
                    gm.EmptyTiles[PosIndex].TileLevel = 0;
                }

                //将指定位置处的方块标记为 指定元素
                gm.EmptyTiles[PosIndex].TileType = type;

                //从空方快列表中删除该位置
                gm.EmptyTiles.RemoveAt(PosIndex);
            }
        }
    }

    //【指定位置建造】 参数是需要建造的单位【类型】和【坐标】
    public bool Generate(ElementType type, int y, int x)
    {
        //若当前坐标为空
        if (gm.AllTiles[x, y].TileType == ElementType.Empty)
        {
            //如果出现的是敌人或建材单位 赋予其等级
            if (type == ElementType.Enemy || type == ElementType.Material)
            {
                gm.AllTiles[x, y].TileLevel = 2;
            }
            else
            {
                gm.AllTiles[x, y].TileLevel = 0;
            }

            //将指定位置处的方块标记为 指定元素
            gm.AllTiles[x, y].TileType = type;

            //从空方快列表中随便删一个位置
            //gm.EmptyTiles.RemoveAt(Random.Range(0, gm.EmptyTiles.Count));

            return true;
        }
        return false;
    }
    #endregion

    //计算指定类型的【建筑数量】
    public int CountOff(ElementType type)
    {
        int num = 0;
        foreach (Tile t in gm.AllTiles)
        {
            if (t.TileType == type)
            {
                num++;
            }

            //顺便遍历所有按钮 如果有处于禁用状态的按钮 将其恢复
            t.GetComponent<Button>().interactable = true;
        }
        return num;
    }

    //【远程攻击】 参数是塔的x,y坐标，和要攻击的单位类型
    public bool TowerAttack(int x, int y, ElementType type)
    {
        bool attack = false;    //【T】本回合已进行过攻击 【F】未进行过攻击

        //横向 先左后右 (x-3,y) -> (x+3,y)
        for (int j = x - 3; j <= x + 3; j++)
        {
            #region 攻击前的判断
            if (attack)
            {
                Debug.Log("已进行过攻击");
                break;
            }
            if (j < 0 || j > 7)
            {
                continue;
            }
            #endregion

            #region 发动攻击
            if (gm.AllTiles[y, j].TileType == type)
            {
                int consume = gm.AllTiles[y, j].TileLevel * 2;
                int surplus = Power.Instance.Numerical - consume;
                if (surplus > 20)
                {
                    if (type != ElementType.Player && type != ElementType.Access)
                    {
                        gm.AllTiles[y, j].TileLevel = 0;
                        gm.AllTiles[y, j].TileType = ElementType.Empty;
                        gm.AllTiles[y, j].SlowBuff = false;
                    }

                    attack = true;

                    Power.Instance.Numerical = surplus;
                    //string str = "攻击消耗" + consume;
                    //Debug.Log(str);

                    return true;
                }
            }
            #endregion
        }

        //纵向 先下后上 (x,y-3) -> (x,y+3)
        for (int i = y - 3; i <= y + 3; i++)
        {
            #region 攻击前的判断
            //每回合只攻击一个单位
            if (attack)
            {
                Debug.Log("已进行过攻击");
                break;
            }
            //防止数组下标越界
            if (i < 0 || i > 10)
            {
                continue;
            }
            #endregion

            #region 发动攻击
            //定位到攻击范围内的格子 判断其是否为攻击目标
            if (gm.AllTiles[i, x].TileType == type)
            {
                int consume = gm.AllTiles[i, x].TileLevel * 2;         //攻击消耗
                int surplus = Power.Instance.Numerical - consume;   //攻击后剩余电力

                //判断是否有足够的资源进行攻击 且不销毁主角
                if (surplus > 20)
                {
                    if (type != ElementType.Player && type != ElementType.Access)
                    {
                        //清空目标的等级和样式
                        gm.AllTiles[i, x].TileLevel = 0;
                        gm.AllTiles[i, x].TileType = ElementType.Empty;
                        //解除减速buff
                        gm.AllTiles[i, x].SlowBuff = false;
                    }

                    //关闭攻击开关
                    attack = true;

                    //消耗相应资源
                    Power.Instance.Numerical = surplus;
                    //string str = "攻击消耗" + consume;
                    //Debug.Log(str);

                    return true;
                }
            }
            #endregion
        }

        return false;
    }

    //敌方支援
    public void Assisted()
    {
        foreach (Tile t in gm.AllTiles)
        {
            if (t.TileType == ElementType.Enemy)
            {
                t.TileLevel *= 2;
                t.UpdateTile();
            }
        }
    }

    //磁场事件 目前定义在回合结束时触发
    public void MagneticEvent(int x, int y)
    {
        //左 x = -3 -2 -1
        for (int i = x - 3; i < x; i++)
        {
            //防止数组下标越界
            if (i - 1 < 0)
            {
                continue;
            }

            //如果是个可移动的单位 且其身后为空或是可移动单位
            if (gm.AllTiles[y, i].TileType < ElementType.Tower && gm.AllTiles[y, i].TileType != ElementType.Empty
                && gm.AllTiles[y, i - 1].TileType == ElementType.Empty)
            {
                //传值给后一个格子
                gm.AllTiles[y, i - 1].TileLevel = gm.AllTiles[y, i].TileLevel;
                gm.AllTiles[y, i - 1].TileType = gm.AllTiles[y, i].TileType;

                //清空自身
                gm.AllTiles[y, i].TileLevel = 0;
                gm.AllTiles[y, i].TileType = ElementType.Empty;
            }
        }

        //右 x = 3 2 1
        for (int i = x + 3; i > x; i--)
        {
            if (i + 1 > 7)
            {
                continue;
            }

            if (gm.AllTiles[y, i].TileType < ElementType.Tower && gm.AllTiles[y, i].TileType != ElementType.Empty
                && gm.AllTiles[y, i + 1].TileType == ElementType.Empty)
            {
                gm.AllTiles[y, i + 1].TileLevel = gm.AllTiles[y, i].TileLevel;
                gm.AllTiles[y, i + 1].TileType = gm.AllTiles[y, i].TileType;

                gm.AllTiles[y, i].TileLevel = 0;
                gm.AllTiles[y, i].TileType = ElementType.Empty;
            }
        }

        //上 y = 3 2 1
        for (int i = y + 3; i > y; i--)
        {
            //防止数组下标越界
            if (i + 1 > 10)
            {
                continue;
            }

            if (gm.AllTiles[i, x].TileType < ElementType.Tower && gm.AllTiles[i, x].TileType != ElementType.Empty
                && gm.AllTiles[i + 1, x].TileType == ElementType.Empty)
            {
                gm.AllTiles[i + 1, x].TileLevel = gm.AllTiles[i, x].TileLevel;
                gm.AllTiles[i + 1, x].TileType = gm.AllTiles[i, x].TileType;

                gm.AllTiles[i, x].TileLevel = 0;
                gm.AllTiles[i, x].TileType = ElementType.Empty;
            }
        }

        //下y = -3 -2 -1
        for (int i = y - 3; i < y; i++)
        {
            if (i - 1 < 0)
            {
                continue;
            }

            if (gm.AllTiles[i, x].TileType < ElementType.Tower && gm.AllTiles[i, x].TileType != ElementType.Empty
                && gm.AllTiles[i - 1, x].TileType == ElementType.Empty)
            {
                gm.AllTiles[i - 1, x].TileLevel = gm.AllTiles[i, x].TileLevel;
                gm.AllTiles[i - 1, x].TileType = gm.AllTiles[i, x].TileType;

                gm.AllTiles[i, x].TileLevel = 0;
                gm.AllTiles[i, x].TileType = ElementType.Empty;
            }
        }
    }

}
