using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//游戏状态 用于配合动画效果
public enum GameState
{
    Playing,                //可操作
    GameSuspension,         //不接受输入
    WaitingForMoveToEnd,     //等待移动结束
}

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
    Wall,       //9 防御塔
    Landmine,   //10 地雷
}

///主控流程
public class GameManager : MonoBehaviour
{
    public GameState State;     //游戏状态

    //public bool over = false;   //游戏是否结束
    public bool won = false;   //游戏是否已经胜利
    public int count = 0;     //记录本关内地雷的建造数量
    private bool move = false; //玩家是否发生过移动
    private int turn = 0;      //记录回合数

    #region 行列与方块列表

    //用于获取所有的方块 
    public Tile[,] AllTiles = new Tile[11, 8];

    //创建行和列的列表 用于移动
    private List<Tile[]> colums = new List<Tile[]>();
    private List<Tile[]> rows = new List<Tile[]>();

    //用于获取空方快 在产生新方块时用于标识
    private List<Tile> EmptyTiles = new List<Tile>();

    #endregion

    /// 1.新游戏开始时的初始化
    void Start()
    {
        //新游戏开始
        //over = true;    //游戏处于开启状态
        State = GameState.Playing;
        won = false;    //本关还未取得胜利
        move = false;
        turn = 3;       //从第3回合开始（为了在三回合后 产生第一个敌人
        count = 0;      //地雷还未建造过

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

        //开局时新建【1个主角】【2个建材】【x个墙壁】
        Generate(ElementType.Player);
        Generate(ElementType.Material);
        Generate(ElementType.Material);
        //Generate(ElementType.Enemy);

        int x = 3;
        while (x > 0)
        {
            Generate(ElementType.Wall);
            x--;
        }
    }

    #region 2.建造一个指定类型的单位
    //【随机位置建造】 参数是需要建造的单位类型
    public void Generate(ElementType type)
    {
        //若场上仍然存在空方快
        if (EmptyTiles.Count > 0)
        {
            //随机一个位置
            int PosIndex = Random.Range(0, EmptyTiles.Count);

            //如果这个位置已经有东西了 则不进行建造
            if (EmptyTiles[PosIndex].TileType != ElementType.Empty)
            {
                EmptyTiles.RemoveAt(PosIndex);  //删除这个位置
                Generate(type);                 //重新触发建造
            }
            else
            {
                //如果这是敌人或建材单位 赋予其等级
                if (type == ElementType.Enemy || type == ElementType.Material)
                {
                    EmptyTiles[PosIndex].TileLevel = 2;
                }
                //其他情况清空等级
                else
                {
                    EmptyTiles[PosIndex].TileLevel = 0;
                }

                //将指定位置处的方块标记为 指定元素
                EmptyTiles[PosIndex].TileType = type;

                //从空方快列表中删除该位置
                EmptyTiles.RemoveAt(PosIndex);
            }
        }
    }

    //【指定位置建造】 参数是需要建造的单位【类型】和【坐标】
    public bool Generate(ElementType type, int y, int x)
    {
        //若当前坐标为空
        if (AllTiles[x, y].TileType == ElementType.Empty)
        {
            //如果出现的是敌人或建材单位 赋予其等级
            if (type == ElementType.Enemy || type == ElementType.Material)
            {
                AllTiles[x, y].TileLevel = 2;
            }
            else
            {
                AllTiles[x, y].TileLevel = 0;
            }

            //将指定位置处的方块标记为 指定元素
            AllTiles[x, y].TileType = type;

            //从空方快列表中随便删一个位置
            //EmptyTiles.RemoveAt(Random.Range(0, EmptyTiles.Count));

            return true;
        }
        return false;
    }
    #endregion

    //获取到输入后的【回合行为】
    public void Move(MoveDirection md)
    {
        //if (over)   //若游戏已经结束 不执行回合的行为
        if (State == GameState.Playing)
        {
            /// 3.获取输入并处理
            bool moveMade = false;  //用于判断本次输入是否有效
            ResetMergedFlags();     //清空所有方块上的开关

            #region 触发【4.移动】【5.合并】【6.消耗】方法
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
            #endregion

            //移动发生后的行为
            if (moveMade)
            {
                /// 7. 建筑行为 并更新空方快列表
                UpdateEmptyTiles();     //触发建筑行为 并且更新空方快列表 防止已有方块被覆盖
                //Material.Instance.Numerical += 50;  //获得建材
                //Power.Instance.Numerical += 50;     //获得电力
                //Money.Instance.Numerical += 50;     //获得金钱

                /// 8.产生新的建材与敌人
                Generate(ElementType.Material);     //回合结束后 新建一个建材
                if (turn % 5 == 0)
                {
                    Generate(ElementType.Enemy);    //每五回合产生一个敌人
                }

                /// 9.游戏胜负的判定
                //【30回合后】场上【不存在敌人】 则游戏胜利 但仅限于第一次获得胜利的情况
                if (!won && turn > 32 && NotEnemy())    //turn从3开始
                {
                    PanelManager.Instance.YouWon();
                }

                //如果电力不足
                if (Power.Instance.Numerical <= 0)
                {
                    //GameOver("电力不足");    //显示游戏结束消息
                    PanelManager.Instance.GameOver("電力が足りない");
                }
                //或是没有可移动的方块
                else if (!CanMove())
                {
                    //GameOver("没有可移动的方块");
                    PanelManager.Instance.GameOver("移動できるコマがない");
                }

                /// 0.回合结束
                turn++;
                move = false;
            }
        }
    }

    //【移动】与【合并】方块 参数是【指定行号的行/列】
    bool MakeOneMoveDownIndex(Tile[] LineOfTiles)   //下 左
    {
        //在逐行获得行/列后，逐个判断方块，进行移动与合并
        for (int i = 0; i < LineOfTiles.Length - 1; i++)
        {
            #region 4.Move Block 移动方块
            //若方块【自身为空】，且后方有一个【非空】&【非建筑】&【没有发生过碰撞】的方块
            if (LineOfTiles[i].TileType == ElementType.Empty && LineOfTiles[i + 1].TileType != ElementType.Empty &&
                (int)LineOfTiles[i + 1].TileType < 5 && LineOfTiles[i + 1].mergedThisTurn == false)
            {
                //移动
                LineOfTiles[i].TileLevel = LineOfTiles[i + 1].TileLevel;    //将后方方块的等级转移到自己身上
                LineOfTiles[i].TileType = LineOfTiles[i + 1].TileType;      //将其的类型也进行转移
                LineOfTiles[i + 1].TileType = ElementType.Empty;            //清空后方方块的数值
                LineOfTiles[i + 1].TileLevel = 0;                           //清除遗留的数字

                //主角或敌人移动时进行消耗
                if (LineOfTiles[i].TileType == ElementType.Player || LineOfTiles[i].TileType == ElementType.Enemy)
                {
                    Power.Instance.Numerical -= 1;
                }
                LineOfTiles[i].moveThisTurn = true;  //本格（该单位）发生过移动
                return true;    //用于控制循环，直至没有可合并的方块
            }
            #endregion

            #region 5.Merge Block 合并方块
            //碰撞规则
            if (LineOfTiles[i].TileType != ElementType.Empty)   //若方块【自身不为空】
            {
                //根据自身类型选择合并规则（自身是被合并的一方
                switch (LineOfTiles[i].TileType)
                {
                    #region 主角
                    case ElementType.Player:
                        //检查自身是否发生过移动
                        if (LineOfTiles[i].moveThisTurn)
                        {
                            move = true;
                        }
                        //遇到敌人时被破坏 但前提是对方本回合并【未发生过碰撞 & 发生过移动】
                        if (LineOfTiles[i + 1].TileType == ElementType.Enemy &&
                            LineOfTiles[i + 1].mergedThisTurn == false && LineOfTiles[i + 1].moveThisTurn == true)
                        {
                            //GameOver("玩家受到攻击"); //游戏失败
                            PanelManager.Instance.GameOver("プレーヤーが攻撃された");
                            return false; ;
                        }
                        //不与其他单位发生事件
                        break;
                    #endregion

                    #region 建材
                    case ElementType.Material:
                        //同等级同类 且自身与后方【本回合都未发生过合并】 合并升级
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
                        //敌人 被破坏 但是敌人必须发生过移动 且在自身发生过合并后可以免疫敌人的攻击
                        else if (LineOfTiles[i + 1].TileType == ElementType.Enemy && LineOfTiles[i + 1].moveThisTurn == true &&
                            LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i + 1].mergedThisTurn == false)
                        {
                            //改变样式
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
                            LineOfTiles[i + 1].TileLevel = 0;   //清空上一个方块的等级
                            LineOfTiles[i + 1].TileType = ElementType.Empty;    //清空上一个方块的样式
                            LineOfTiles[i].mergedThisTurn = true;
                            return true;
                        }
                        //被其他单位碰撞不发生事件
                        break;
                    #endregion

                    #region 建筑
                    //地雷
                    case ElementType.Landmine:
                        //仅与敌人发生事件 【不管敌人处于何种状态】
                        if (LineOfTiles[i + 1].TileType == ElementType.Enemy)
                        {
                            //被8级以下敌人碰撞 毁灭敌人和自身
                            if (LineOfTiles[i + 1].TileLevel <= 8)
                            {
                                LineOfTiles[i].TileType = ElementType.Empty;    //销毁自身
                                LineOfTiles[i + 1].TileLevel = 0;               //清空敌人的等级
                                LineOfTiles[i + 1].TileType = ElementType.Empty;//清空敌人的样式
                            }
                            //8级以上敌人等级减半
                            else
                            {
                                LineOfTiles[i].TileLevel = LineOfTiles[i + 1].TileLevel / 2;    //等级减半
                                LineOfTiles[i].TileType = ElementType.Enemy;                    //销毁变为敌人
                                LineOfTiles[i + 1].TileLevel = 0;                               //清空上一个方块的等级
                                LineOfTiles[i + 1].TileType = ElementType.Empty;                //清空上一个方块的样式
                                LineOfTiles[i].mergedThisTurn = true;   //敌人不再碰撞
                            }
                            //不再移动
                            return true;
                        }
                        break;

                    //墙壁
                    case ElementType.Wall:
                        //不发生任何事件
                        break;

                        //可以被主角推动 但每回合【只移动一格】&【主角未发生过合并】&【当前不处于边缘】
                        if (LineOfTiles[i + 1].TileType == ElementType.Player && i - 1 >= 0 &&
                            LineOfTiles[i].moveThisTurn == false && LineOfTiles[i + 1].mergedThisTurn == false && LineOfTiles[i - 1].TileType == ElementType.Empty)
                        {
                            Debug.Log("推");
                            //将自身传递到前方一格

                            //将主角移动到身后

                            //清空主角身后的位置

                            //关闭自身的移动开关
                            LineOfTiles[i + 1].moveThisTurn = true;
                        }
                        break;

                    //其他建筑（攻击塔、发电站、商场
                    default:
                        //会被敌人破坏 但敌人本回合必须【进行过移动】&【未发生过合并】
                        if (LineOfTiles[i + 1].TileType == ElementType.Enemy &&
                            LineOfTiles[i + 1].mergedThisTurn == false && LineOfTiles[i + 1].moveThisTurn == true)
                        {
                            //改变样式
                            LineOfTiles[i].TileLevel = LineOfTiles[i + 1].TileLevel;    //获取到敌人的等级
                            LineOfTiles[i].TileType = ElementType.Enemy;                //自身变为敌人
                            //LineOfTiles[i].UpdateTile();                                //更新方块的样式
                            //清空上一个方块
                            LineOfTiles[i + 1].TileLevel = 0;
                            LineOfTiles[i + 1].TileType = ElementType.Empty;
                            //不再合并与移动
                            LineOfTiles[i].mergedThisTurn = true;
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
    bool MakeOneMoveUpIndex(Tile[] LineOfTiles)     //上 右
    {
        for (int i = LineOfTiles.Length - 1; i > 0; i--)
        {
            #region 4.移动方块
            if (LineOfTiles[i].TileType == ElementType.Empty && LineOfTiles[i - 1].TileType != ElementType.Empty &&
                (int)LineOfTiles[i - 1].TileType < 5 && LineOfTiles[i - 1].mergedThisTurn == false)
            {
                LineOfTiles[i].TileLevel = LineOfTiles[i - 1].TileLevel;
                LineOfTiles[i].TileType = LineOfTiles[i - 1].TileType;
                LineOfTiles[i - 1].TileType = ElementType.Empty;
                LineOfTiles[i - 1].TileLevel = 0;

                if (LineOfTiles[i].TileType == ElementType.Player || LineOfTiles[i].TileType == ElementType.Enemy)
                {
                    Power.Instance.Numerical -= 1;
                }
                LineOfTiles[i].moveThisTurn = true;
                return true;
            }
            #endregion

            #region 5.合并方块
            //碰撞规则
            if (LineOfTiles[i].TileType != ElementType.Empty)
            {
                switch (LineOfTiles[i].TileType)
                {
                    #region 主角
                    case ElementType.Player:
                        //检查自身是否发生移动
                        if (LineOfTiles[i].moveThisTurn)
                        {
                            move = true;
                        }
                        //遇到敌人时被破坏
                        if (LineOfTiles[i - 1].TileType == ElementType.Enemy &&
                            LineOfTiles[i - 1].mergedThisTurn == false && LineOfTiles[i - 1].moveThisTurn == true)
                        {
                            //GameOver("玩家受到攻击");
                            PanelManager.Instance.GameOver("プレーヤーが攻撃された");
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
                        else if (LineOfTiles[i - 1].TileType == ElementType.Enemy && LineOfTiles[i - 1].moveThisTurn == true &&
                            LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i - 1].mergedThisTurn == false)
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
                    //地雷
                    case ElementType.Landmine:
                        if (LineOfTiles[i - 1].TileType == ElementType.Enemy)
                        {
                            if (LineOfTiles[i + 1].TileLevel <= 8)
                            {
                                LineOfTiles[i].TileType = ElementType.Empty;
                                LineOfTiles[i - 1].TileLevel = 0;
                                LineOfTiles[i - 1].TileType = ElementType.Empty;
                            }
                            else
                            {
                                LineOfTiles[i].TileLevel = LineOfTiles[i - 1].TileLevel / 2;
                                LineOfTiles[i].TileType = ElementType.Enemy;
                                LineOfTiles[i - 1].TileLevel = 0;
                                LineOfTiles[i - 1].TileType = ElementType.Empty;
                                LineOfTiles[i].mergedThisTurn = true;
                            }
                            return true;
                        }
                        break;

                    //墙壁
                    case ElementType.Wall:
                        //不发生任何事件
                        break;

                        //可以被主角推动 但每回合【当前不处于边缘】&【只移动一格】&【主角未发生过合并】&【后方为空】
                        if (LineOfTiles[i - 1].TileType == ElementType.Player && i + 1 <= (LineOfTiles.Length - 1) &&
                            LineOfTiles[i].moveThisTurn == false && LineOfTiles[i - 1].mergedThisTurn == false && LineOfTiles[1 + 1].TileType == ElementType.Empty)
                        {
                            Debug.Log("推");
                            //将自身传递到前方一格

                            //将主角移动到身后

                            //清空主角身后的位置

                            //关闭自身的移动开关
                            LineOfTiles[i - 1].moveThisTurn = true;
                        }
                        break;

                    //其他建筑（攻击塔、发电站、商场
                    default:
                        //会被敌人破坏
                        if (LineOfTiles[i - 1].TileType == ElementType.Enemy &&
                            LineOfTiles[i - 1].mergedThisTurn == false && LineOfTiles[i - 1].moveThisTurn == true)
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
                }
            }
            #endregion
        }
        return false;
    }

    //计算指定类型的【建筑数量】 顺便更新空方快列表
    public int CountOff(ElementType type)
    {
        //EmptyTiles.Clear();
        int num = 0;

        foreach (Tile t in AllTiles)
        {
            if (t.TileType == type)
            {
                num++;
            }
            //else if (t.TileType == ElementType.Empty)
            //{
            //    EmptyTiles.Add(t);
            //}
        }
        return num;
    }

    //更新空方块列表与【建筑行为】
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
                    TowerAttack(t.indCol, t.indRow, ElementType.Enemy); //攻击塔
                    break;

                case ElementType.Power:
                    //仅在玩家发生过移动时产生资源
                    if (move)
                    {
                        Power.Instance.Numerical += 10; //获得电力 （根据建筑等级？
                    }
                    break;

                case ElementType.Mall:
                    if (move)
                    {
                        Money.Instance.Numerical += 5; //获得金钱 （根据建筑等级？
                    }
                    break;
            }
        }
    }

    //【远程攻击】 参数是塔的x,y坐标，和要攻击的单位类型
    private void TowerAttack(int x, int y, ElementType type)
    {
        //int x = t.indCol;   //max 7
        //int y = t.indRow;   //max 10

        //string s = "攻击塔坐标:" + y + "，" + x;
        //Debug.Log(s);

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
            if (AllTiles[y, j].TileType == type)
            {
                int consume = AllTiles[y, j].TileLevel * 2;
                int surplus = Power.Instance.Numerical - consume;
                if (surplus > 20)
                {
                    AllTiles[y, j].TileLevel = 0;
                    AllTiles[y, j].TileType = ElementType.Empty;

                    attack = true;

                    Power.Instance.Numerical = surplus;
                    string str = "攻击消耗" + consume;
                    Debug.Log(str);
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
            if (AllTiles[i, x].TileType == type)
            {
                int consume = AllTiles[i, x].TileLevel * 2;    //攻击消耗
                int surplus = Power.Instance.Numerical - consume;   //攻击后剩余电力
                                                                    //判断是否有足够的资源进行攻击
                if (surplus > 20)
                {
                    //清空敌人的等级和样式
                    AllTiles[i, x].TileLevel = 0;
                    AllTiles[i, x].TileType = ElementType.Empty;

                    attack = true;      //关闭攻击开关
                                        //消耗相应资源
                    Power.Instance.Numerical = surplus;
                    string str = "攻击消耗" + consume;
                    Debug.Log(str);
                }
            }
            #endregion
        }
    }

    //管理合并开关（回合结束后
    private void ResetMergedFlags()
    {
        //遍历所有的方块
        foreach (Tile t in AllTiles)
        {
            t.mergedThisTurn = false;   //将他们标记为可合并，用于下回合的行动
            t.moveThisTurn = false;           //将所有的方块标记为未移动
        }
    }

    #region 游戏胜负的判定
    //检查场上是否还存在敌人
    bool NotEnemy()
    {
        foreach (Tile t in AllTiles)
        {
            //如果还存在敌人
            if (t.TileType == ElementType.Enemy)
            {
                return false;
            }
        }
        //不存在敌人
        return true;
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
    #endregion

    //debug用
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PanelManager.Instance.YouWon();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            PanelManager.Instance.GameOver("手动结束");
        }
        //清空所有可移动的单位
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            foreach (Tile t in AllTiles)
            {
                if (t.TileType == ElementType.Material || t.TileType == ElementType.Enemy)
                {
                    t.TileType = ElementType.Empty;
                }
            }
        }
        //清空主角外的所有单位
        else if (Input.GetKeyDown(KeyCode.X))
        {
            foreach (Tile t in AllTiles)
            {
                if (t.TileType != ElementType.Player)
                {
                    t.TileType = ElementType.Empty;
                }
            }
        }
        //制造一个敌人
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Generate(ElementType.Enemy);
        }
        //获得大量资源
        else if (Input.GetKeyDown(KeyCode.L))
        {
            Material.Instance.Numerical += 3000;  //获得建材
            Power.Instance.Numerical += 3000;     //获得电力
            Money.Instance.Numerical += 3000;     //获得金钱
        }
    }

}
