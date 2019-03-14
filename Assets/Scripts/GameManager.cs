using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//定义一个枚举来储存方块元素类型
public enum ElementType
{
    Empty,      //空

    Player = 1, //1 主角
    Enemy,      //2 敌人
    Material,   //3 建材

    Tower = 6,  //6 攻击塔
    Power,      //7 发电站
    Mall,       //8 商场
    Wall        //9 防御塔
}

///主控流程
public class GameManager : MonoBehaviour {

    private int turn = 0;       //记录回合数
    private bool over = false;  //游戏是否结束

    # region 行列与方块列表

    //用于获取所有的方块 
    private Tile[,] AllTiles = new Tile[11, 8];

    //创建行和列的列表 用于移动
    private List<Tile[]> colums = new List<Tile[]>();
    private List<Tile[]> rows = new List<Tile[]>();

    //用于获取空方快 在产生新方块时用于标识
    private List<Tile> EmptyTiles = new List<Tile>();

    #endregion

    # region 胜利或失败面板
    public GameObject WinText;          //胜利文字
    public GameObject LostText;         //失败文字
    public Text MessageText;            //失败理由/胜利积分
    public GameObject GameOverPanel;    //结束面板
    #endregion

    /// 1.新游戏开始时的初始化
    void Start()
    {
        //新游戏开始
        over = true;    //游戏处于开启状态
        turn = 1;       //第0回合开始

        //游戏开始时清除场地
        Tile[] AllTilesOneDim = GameObject.FindObjectsOfType<Tile>();   //获取到所有的方块
        foreach (Tile t in AllTilesOneDim)
        {
            t.TileType = ElementType.Empty;     //游戏开始时清除场地 ，清除所有方块上的信息
            AllTiles[t.indRow, t.indCol] = t;   //在游戏管理器中存储关于所有图块的信息 ，在每一个方块上都储存其行号和列号
            EmptyTiles.Add(t);                  //将当前位置存入空图块列表

            //if(t.indRow == 0 && t.indCol == 0)
            //{
            //    Generate(ElementType.Player, t.indRow, t.indCol);   //在指定位置创建主角
            //}
            //else
            //{

            //}
        }

        # region 初始化 行和列 的列表
        colums.Add(new Tile[] { AllTiles[0, 0], AllTiles[1, 0], AllTiles[2, 0], AllTiles[3, 0], AllTiles[4, 0], AllTiles[5, 0], AllTiles[6, 0], AllTiles[7, 0], AllTiles[8, 0], AllTiles[9, 0], AllTiles[10, 0] });
        colums.Add(new Tile[] { AllTiles[0, 1], AllTiles[1, 1], AllTiles[2, 1], AllTiles[3, 1], AllTiles[4, 1], AllTiles[5, 1], AllTiles[6, 1], AllTiles[7, 1], AllTiles[8, 1], AllTiles[9, 1], AllTiles[10, 1] });
        colums.Add(new Tile[] { AllTiles[0, 2], AllTiles[1, 2], AllTiles[2, 2], AllTiles[3, 2], AllTiles[4, 2], AllTiles[5, 2], AllTiles[6, 2], AllTiles[7, 2], AllTiles[8, 2], AllTiles[9, 2], AllTiles[10, 2] });
        colums.Add(new Tile[] { AllTiles[0, 3], AllTiles[1, 3], AllTiles[2, 3], AllTiles[3, 3], AllTiles[4, 3], AllTiles[5, 3], AllTiles[6, 3], AllTiles[7, 3], AllTiles[8, 3], AllTiles[9, 3], AllTiles[10, 3] });
        colums.Add(new Tile[] { AllTiles[0, 4], AllTiles[1, 4], AllTiles[2, 4], AllTiles[3, 4], AllTiles[4, 4], AllTiles[5, 4], AllTiles[6, 4], AllTiles[7, 4], AllTiles[8, 4], AllTiles[9, 4], AllTiles[10, 4] });
        colums.Add(new Tile[] { AllTiles[0, 5], AllTiles[1, 5], AllTiles[2, 5], AllTiles[3, 5], AllTiles[4, 5], AllTiles[5, 5], AllTiles[6, 5], AllTiles[7, 5], AllTiles[8, 5], AllTiles[9, 5], AllTiles[10, 5] });
        colums.Add(new Tile[] { AllTiles[0, 6], AllTiles[1, 6], AllTiles[2, 6], AllTiles[3, 6], AllTiles[4, 6], AllTiles[5, 6], AllTiles[6, 6], AllTiles[7, 6], AllTiles[8, 6], AllTiles[9, 6], AllTiles[10, 6] });
        colums.Add(new Tile[] { AllTiles[0, 7], AllTiles[1, 7], AllTiles[2, 7], AllTiles[3, 7], AllTiles[4, 7], AllTiles[5, 7], AllTiles[6, 7], AllTiles[7, 7], AllTiles[8, 7], AllTiles[9, 7], AllTiles[10, 7] });

        rows.Add(new Tile[] { AllTiles[0, 0], AllTiles[0, 1], AllTiles[0, 2], AllTiles[0, 3], AllTiles[0, 4], AllTiles[0, 5], AllTiles[0, 6], AllTiles[0, 7] });
        rows.Add(new Tile[] { AllTiles[1, 0], AllTiles[1, 1], AllTiles[1, 2], AllTiles[1, 3], AllTiles[1, 4], AllTiles[1, 5], AllTiles[1, 6], AllTiles[1, 7] });
        rows.Add(new Tile[] { AllTiles[2, 0], AllTiles[2, 1], AllTiles[2, 2], AllTiles[2, 3], AllTiles[2, 4], AllTiles[2, 5], AllTiles[2, 6], AllTiles[2, 7] });
        rows.Add(new Tile[] { AllTiles[3, 0], AllTiles[3, 1], AllTiles[3, 2], AllTiles[3, 3], AllTiles[3, 4], AllTiles[3, 5], AllTiles[3, 6], AllTiles[3, 7] });
        rows.Add(new Tile[] { AllTiles[4, 0], AllTiles[4, 1], AllTiles[4, 2], AllTiles[4, 3], AllTiles[4, 4], AllTiles[4, 5], AllTiles[4, 6], AllTiles[4, 7] });
        rows.Add(new Tile[] { AllTiles[5, 0], AllTiles[5, 1], AllTiles[5, 2], AllTiles[5, 3], AllTiles[5, 4], AllTiles[5, 5], AllTiles[5, 6], AllTiles[5, 7] });
        rows.Add(new Tile[] { AllTiles[6, 0], AllTiles[6, 1], AllTiles[6, 2], AllTiles[6, 3], AllTiles[6, 4], AllTiles[6, 5], AllTiles[6, 6], AllTiles[6, 7] });
        rows.Add(new Tile[] { AllTiles[7, 0], AllTiles[7, 1], AllTiles[7, 2], AllTiles[7, 3], AllTiles[7, 4], AllTiles[7, 5], AllTiles[7, 6], AllTiles[7, 7] });
        rows.Add(new Tile[] { AllTiles[8, 0], AllTiles[8, 1], AllTiles[8, 2], AllTiles[8, 3], AllTiles[8, 4], AllTiles[8, 5], AllTiles[8, 6], AllTiles[8, 7] });
        rows.Add(new Tile[] { AllTiles[9, 0], AllTiles[9, 1], AllTiles[9, 2], AllTiles[9, 3], AllTiles[9, 4], AllTiles[9, 5], AllTiles[9, 6], AllTiles[9, 7] });
        rows.Add(new Tile[] { AllTiles[10, 0], AllTiles[10, 1], AllTiles[10, 2], AllTiles[10, 3], AllTiles[10, 4], AllTiles[10, 5], AllTiles[10, 6], AllTiles[10, 7] });
        #endregion

        //开局时新建【一个主角】【一个建材】【x个墙壁】【一个敌人】
        Generate(ElementType.Material);
        Generate(ElementType.Material);
        Generate(ElementType.Player);
        Generate(ElementType.Enemy);

        int x = 3;
        while (x > 0)
        {
            Generate(ElementType.Wall);
            x--;
        }
    }

    /// 2.建造一个指定类型的单位 参数是需要建造的单位类型
    public void Generate(ElementType Type)
    {
        //若场上仍然存在空方快
        if (EmptyTiles.Count > 0)
        {
            //随机一个位置
            int PosIndex = Random.Range(0, EmptyTiles.Count);

            //如果这是敌人或建材单位 赋予其等级
            if (Type == ElementType.Enemy || Type == ElementType.Material)
            {
                EmptyTiles[PosIndex].TileLevel = 2;
            }
            //其他情况清空等级
            else
            {
                EmptyTiles[PosIndex].TileLevel = 0;
            }

            //将指定位置处的方块标记为 指定元素
            EmptyTiles[PosIndex].TileType = Type;

            //从空方快列表中删除该位置
            EmptyTiles.RemoveAt(PosIndex);
        }

        #region 指定坐标建造方案
        //if (EmptyTiles.Count > 0)
        //{
        //    //如果这是敌人或建材单位 赋予其等级
        //    if (Type == ElementType.Enemy || Type == ElementType.Material)
        //    {
        //        AllTiles[x][y].TileLevel = 2;
        //    }
        //    //将指定位置处的方块标记为 指定元素
        //    AllTiles[x][y].TileType = Type;
        //}
        #endregion
    }

    //检查是否还有可移动的方块
    bool CanMove()
    {
        //无视剩余空方快大于零的情况
        if (EmptyTiles.Count > 0)
            return true;

        //无视方块周围还有相同数字方块的情况
        else
        {
            //check columns
            //for (int i = 0; i < colums.Count; i++)
            //    for (int j = 0; j < rows.Count - 1; j++)
            //        if (AllTiles[j, i].Number == AllTiles[j + 1, i].Number)
            //            return true;

            ////check rows
            //for (int i = 0; i < rows.Count; i++)
            //    for (int j = 0; j < colums.Count - 1; j++)
            //        if (AllTiles[i, j].Number == AllTiles[i, j + 1].Number)
            //            return true;
        }

        //既没有空方快，也没有可以合并的方块时，游戏失败
        return false;
    }

    //更新空方块（在每次发生过移动之后）
    private void UpdateEmptyTiles()
    {
        //清空空方快列表
        EmptyTiles.Clear();
        //遍历所有方块
        foreach (Tile t in AllTiles)
        {
            /// 7.建筑行为与空方快列表的更新
            switch (t.TileType)
            {
                case ElementType.Empty:
                    //更新空方块列表
                    EmptyTiles.Add(t);
                    break;
                case ElementType.Tower:
                    //攻击塔【】
                    TowerAttack(t.indRow, t.indCol);
                    break;
                case ElementType.Power:
                    //获得电力 （根据建筑等级？
                    Power.Instance.Numerical += 10;
                    break;
                case ElementType.Mall:
                    //获得金钱 （根据建筑等级？
                    Money.Instance.Numerical += 10;
                    break;
            }
        }
    }

    //攻击塔行为 需要获取到自身的x,y坐标【】
    private void TowerAttack(int x, int y)
    {

    }

    //合并图块-关闭已合并开关
    private void ResetMergedFlags()
    {
        //遍历所有的方块
        foreach (Tile t in AllTiles)
            //将他们标记为可合并，用于下回合的行动
            t.mergedThisTurn = false;
    }

    //移动图块 参数是【指定行号的行/列】
    bool MakeOneMoveDownIndex(Tile[] LineOfTiles)
    {
        //在逐行获得行/列后，逐个判断方块，进行移动与合并
        for (int i = 0; i< LineOfTiles.Length - 1; i++)
        {
            /// 4.Move Block 移动方块
            //若方块【自身为空】，且后方有一个【非空非建筑的方块】
            if (LineOfTiles[i].TileType == ElementType.Empty && 
                LineOfTiles[i + 1].TileType != ElementType.Empty && (int)LineOfTiles[i + 1].TileType < 5)
            {
                LineOfTiles[i].TileLevel = LineOfTiles[i + 1].TileLevel;    //将后方方块的等级转移到自己身上
                LineOfTiles[i].TileType = LineOfTiles[i + 1].TileType;      //将其的类型也进行转移
                LineOfTiles[i + 1].TileType = ElementType.Empty;            //清空后方方块的数值
                LineOfTiles[i + 1].TileLevel = 0;                           //清除遗留的数字

                return true;    //用于控制循环，直至没有可合并的方块
            }

            /// 5.Merge Block 合并方块
            #region 碰撞规则
            //若方块【自身不为空】
            if (LineOfTiles[i].TileType != ElementType.Empty)
            {
                //根据自身类型选择合并规则（自身是被合并的一方
                switch (LineOfTiles[i].TileType)
                {
                    #region 主角
                    case ElementType.Player:
                        //遇到敌人时被破坏 但前提是对方本回合并未发生过碰撞
                        if (LineOfTiles[i + 1].TileType == ElementType.Enemy && LineOfTiles[i + 1].mergedThisTurn == false)
                        {
                            GameOver("玩家受到攻击"); //游戏失败
                            return false; ;
                        }
                        //不与其他单位发生事件
                        break;
                    #endregion

                    #region 建材
                    case ElementType.Material:
                        //同等级同类 合并升级 且自身与后方【本回合都未发生过合并】
                        if (LineOfTiles[i + 1].TileType == ElementType.Material && LineOfTiles[i].TileLevel == LineOfTiles[i + 1].TileLevel &&
                            LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i + 1].mergedThisTurn == false)
                        {
                            LineOfTiles[i].TileLevel *= 2;      //等级翻倍
                            LineOfTiles[i].UpdateTile();        //更新方块内容
                            LineOfTiles[i + 1].TileLevel = 0;   //清空前一个方块的等级
                            LineOfTiles[i + 1].TileType = ElementType.Empty;    //清空前一个方块的样式
                            LineOfTiles[i].mergedThisTurn = true;   //该方块不再合并
                            return true;
                        }
                        //主角 被吸收并提供相应资源 且自身与后方【本回合都未发生过合并】
                        else if (LineOfTiles[i + 1].TileType == ElementType.Player &&
                            LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i + 1].mergedThisTurn == false)
                        {
                            Material.Instance.Numerical += LineOfTiles[i].TileLevel;    //获取到相应资源
                            LineOfTiles[i].TileType = ElementType.Player;       //将自身销毁 改为主角
                            LineOfTiles[i + 1].TileLevel = 0;                   //清空前一个方块的数字
                            LineOfTiles[i + 1].TileType = ElementType.Empty;    //清空前一个方块的样式
                            LineOfTiles[i].mergedThisTurn = true;               //不再合并
                            return true;
                        }
                        //敌人 被破坏
                        else if (LineOfTiles[i + 1].TileType == ElementType.Enemy && LineOfTiles[i + 1].mergedThisTurn == false)
                        {
                            LineOfTiles[i].TileLevel = LineOfTiles[i + 1].TileLevel;    //获取到敌人的等级
                            LineOfTiles[i].TileType = ElementType.Enemy;                //销毁自身 变为敌人
                            //清空上一个方块
                            LineOfTiles[i + 1].TileLevel = 0;
                            LineOfTiles[i + 1].TileType = ElementType.Empty;
                            //不再合并
                            LineOfTiles[i].mergedThisTurn = true;
                            return true;
                        }
                        //其他情况不发生事件
                        break;
                    #endregion

                    #region 敌人
                    case ElementType.Enemy:
                        //同等级同类 合并升级 且自身与后方【本回合都未发生过合并】
                        if (LineOfTiles[i + 1].TileType == ElementType.Enemy && LineOfTiles[i].TileLevel == LineOfTiles[i + 1].TileLevel &&
                            LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i + 1].mergedThisTurn == false)
                        {
                            LineOfTiles[i].TileLevel *= 2;      //等级翻倍
                            LineOfTiles[i].UpdateTile();        //更新样式
                            LineOfTiles[i + 1].TileLevel = 0;   //清空上一个方块
                            LineOfTiles[i + 1].TileType = ElementType.Empty;
                            LineOfTiles[i].mergedThisTurn = true;
                            return true;
                        }
                        //被其他单位碰撞不发生事件
                        break;
                    #endregion

                    #region 建筑
                    //墙壁
                    case ElementType.Wall:
                        //不发生任何事件
                        break;

                    //其他建筑（攻击塔、发电站、商场
                    default:
                        //会被敌人破坏
                        if (LineOfTiles[i + 1].TileType == ElementType.Enemy && LineOfTiles[i + 1].mergedThisTurn == false)
                        {
                            LineOfTiles[i].TileLevel = LineOfTiles[i + 1].TileLevel;    //获取到敌人的等级
                            LineOfTiles[i].TileType = ElementType.Enemy;                //自身变为敌人
                            LineOfTiles[i].UpdateTile();                    //更新方块的样式
                            return true;
                        }
                        break;
                        #endregion
                }
                //switch结束
                //return true;    //用于控制循环，直至没有可合并的方块
            }
            #endregion

            #region 源码保存
            //若方块【自身不为空】，且与后方【方块的类型】和【数字】一样，以及双方的【本回合都未发生过合并】
            //if (LineOfTiles[i].TileType != ElementType.Empty &&
            //        LineOfTiles[i].TileType == LineOfTiles[i + 1].TileType && LineOfTiles[i].TileLevel == LineOfTiles[i + 1].TileLevel &&
            //        LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i + 1].mergedThisTurn == false)
            //{
            //    LineOfTiles[i].TileLevel *= 2;                      //方块自身的等级翻倍
            //    LineOfTiles[i].UpdateTile();                        //更新方块的样式
            //    LineOfTiles[i + 1].TileLevel = 0;                   //后方方块的数字被清零
            //    LineOfTiles[i + 1].TileType = ElementType.Empty;    //消除后方的方块
            //    LineOfTiles[i].mergedThisTurn = true;               //将方块标记为不再合并
            //    return true;
            //}
            #endregion
        }

        //若检测不到可移动/合并的方块，则终止循环
        return false;
    }
    bool MakeOneMoveUpIndex(Tile[] LineOfTiles)
    {
        for (int i = LineOfTiles.Length - 1; i > 0; i--)
        {
            ///Move Block 移动方块
            if (LineOfTiles[i].TileType == ElementType.Empty && 
                LineOfTiles[i - 1].TileType != ElementType.Empty && (int)LineOfTiles[i - 1].TileType < 5)
            {
                LineOfTiles[i].TileLevel = LineOfTiles[i - 1].TileLevel;
                LineOfTiles[i].TileType = LineOfTiles[i - 1].TileType;
                LineOfTiles[i - 1].TileType = ElementType.Empty;
                LineOfTiles[i - 1].TileLevel = 0;
                return true;
            }

            ///Merge Block 合并方块
            #region 碰撞规则
            if (LineOfTiles[i].TileType != ElementType.Empty)
            {
                switch (LineOfTiles[i].TileType)
                {
                    #region 主角
                    case ElementType.Player:
                        //遇到敌人时被破坏
                        if (LineOfTiles[i - 1].TileType == ElementType.Enemy && LineOfTiles[i - 1].mergedThisTurn == false)
                        {
                            GameOver("玩家受到攻击");
                            return false; ;
                        }
                        break;
                    #endregion

                    #region 建材
                    case ElementType.Material:
                        //同等级同类 合并升级
                        if (LineOfTiles[i - 1].TileType == ElementType.Material && LineOfTiles[i].TileLevel == LineOfTiles[i - 1].TileLevel &&
                            LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i - 1].mergedThisTurn == false)
                        {
                            LineOfTiles[i].TileLevel *= 2;
                            LineOfTiles[i].UpdateTile();
                            LineOfTiles[i - 1].TileLevel = 0;
                            LineOfTiles[i - 1].TileType = ElementType.Empty;
                            LineOfTiles[i].mergedThisTurn = true;
                            return true;
                        }
                        //主角 被吸收并提供相应资源
                        else if (LineOfTiles[i - 1].TileType == ElementType.Player &&
                            LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i - 1].mergedThisTurn == false)
                        {
                            Material.Instance.Numerical += LineOfTiles[i].TileLevel;
                            LineOfTiles[i].TileType = ElementType.Player;
                            LineOfTiles[i - 1].TileLevel = 0;
                            LineOfTiles[i - 1].TileType = ElementType.Empty;
                            LineOfTiles[i].mergedThisTurn = true;
                            return true;
                        }
                        //敌人 被破坏
                        else if (LineOfTiles[i - 1].TileType == ElementType.Enemy && LineOfTiles[i - 1].mergedThisTurn == false)
                        {
                            LineOfTiles[i].TileLevel = LineOfTiles[i - 1].TileLevel;
                            LineOfTiles[i].TileType = ElementType.Enemy;
                            LineOfTiles[i - 1].TileLevel = 0;
                            LineOfTiles[i - 1].TileType = ElementType.Empty;
                            LineOfTiles[i].mergedThisTurn = true;
                            return true;
                        }
                        break;
                    #endregion

                    #region 敌人
                    case ElementType.Enemy:
                        //同等级同类 合并升级
                        if (LineOfTiles[i - 1].TileType == ElementType.Enemy && LineOfTiles[i].TileLevel == LineOfTiles[i - 1].TileLevel &&
                            LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i - 1].mergedThisTurn == false)
                        {
                            LineOfTiles[i].TileLevel *= 2;
                            LineOfTiles[i].UpdateTile();
                            LineOfTiles[i - 1].TileLevel = 0;
                            LineOfTiles[i - 1].TileType = ElementType.Empty;
                            LineOfTiles[i].mergedThisTurn = true;
                            return true;
                        }
                        break;
                    #endregion

                    #region 建筑
                    //墙壁
                    case ElementType.Wall:
                        //不发生任何事件
                        break;

                    //其他建筑（攻击塔、发电站、商场
                    default:
                        //会被敌人破坏
                        if (LineOfTiles[i - 1].TileType == ElementType.Enemy && LineOfTiles[i - 1].mergedThisTurn == false)
                        {
                            LineOfTiles[i].TileLevel = LineOfTiles[i + 1].TileLevel;
                            LineOfTiles[i].TileType = ElementType.Enemy;
                            LineOfTiles[i].UpdateTile();
                            return true;
                        }
                        break;
                    #endregion
                }
                //switch结束
                //return true;    //用于控制循环，直至没有可合并的方块
            }
            #endregion

            #region 源码保存
            //if (LineOfTiles[i].TileType != ElementType.Empty &&
            //    LineOfTiles[i].TileType == LineOfTiles[i - 1].TileType && LineOfTiles[i].TileLevel == LineOfTiles[i - 1].TileLevel &&
            //    LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i - 1].mergedThisTurn == false)
            //{
            //    LineOfTiles[i].TileLevel *= 2;
            //    LineOfTiles[i].UpdateTile();
            //    LineOfTiles[i - 1].TileLevel = 0;
            //    LineOfTiles[i - 1].TileType = ElementType.Empty;
            //    LineOfTiles[i].mergedThisTurn = true;
            //    return true;
            //}
            #endregion
        }
        return false;
    }

    //移动方法 参数为获取到的移动方向 根据方向进行相应的移动
    public void Move(MoveDirection md)
    {
        if (over)   //若游戏已经结束 不执行回合的行为
        {
            /// 3.获取输入并处理
            bool moveMade = false;  //用于判断本次输入是否有效
            ResetMergedFlags();     //打开合并开关

            //处理【4.移动】【5.合并】【6.消耗】
            switch (md)
            {
                case MoveDirection.Down:    //下
                    for (int i = 0; i < colums.Count; i++)
                    {
                        while (MakeOneMoveDownIndex(colums[i]))   //i为行/列号 逐行传递给移动函数
                        {
                            moveMade = true;
                        }
                    }
                    break;
                case MoveDirection.Left:    //左
                    for (int i = 0; i < rows.Count; i++)
                    {
                        while (MakeOneMoveDownIndex(rows[i]))
                        {
                            moveMade = true;
                        }
                    }
                    break;
                case MoveDirection.Right:   //右
                    for (int i = 0; i < rows.Count; i++)
                    {
                        while (MakeOneMoveUpIndex(rows[i]))
                        {
                            moveMade = true;
                        }
                    }
                    break;
                case MoveDirection.Up:     //上
                    for (int i = 0; i < colums.Count; i++)
                    {
                        while (MakeOneMoveUpIndex(colums[i]))
                        {
                            moveMade = true;
                        }
                    }
                    break;
            }

            //移动发生后的行为
            if (moveMade)
            {
                /// 7. 建筑行为 并更新空方快列表
                //CanMove();
                //Material.Instance.Numerical += 8;   //获得建材

                /// 8.产生新的建材与敌人
                UpdateEmptyTiles();     //更新空方快列表 防止已有方块被覆盖

                Generate(ElementType.Material);     //回合结束后 新建一个建材
                if (turn % 5 == 0)
                {
                    Generate(ElementType.Enemy);    //每两回合产生一个敌人
                }

                //检测是否还有可移动的方块
                if (!CanMove())
                {
                    GameOver("没有可移动的方块"); //否则显示游戏结束消息
                }

                /// 0.回合结束
                turn++;
            }
        }
    }

    #region 游戏结束后的行为
    //游戏胜利界面
    public void YouWon()
    {
        over = false;                   //禁止接受输入
        LostText.SetActive(false);      //关闭失败字幕
        WinText.SetActive(true);        //开启胜利字幕
        GameOverPanel.SetActive(true);  //开启提示面板

        MessageText.text = "你赚到了100金钱";
    }
    //失败提示界面
    public void GameOver(string s)
    {
        over = false;
        //激活提示面板 覆盖游戏面板
        GameOverPanel.SetActive(true);

        MessageText.text = s;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            YouWon();
        }
    }
    #endregion
}
