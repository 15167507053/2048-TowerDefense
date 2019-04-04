using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    private GameManager gm;
    public static PanelManager Instance;

    #region 胜利或失败面板
    public Text TitleText;              //胜利或失败的文字
    public Text MessageText;            //失败理由 & 胜利积分
    public GameObject GameOverPanel;    //结束面板
    public GameObject ContinueBtn;      //继续游戏按钮 仅在游戏胜利时显示
    #endregion

    #region 菜单面板
    public GameObject OptionsPanel;     //菜单面板
    public GameObject ButtonList;       //默认的按钮面板
    public GameObject RuleText;         //规则说明
    public GameObject IntroductionText; //单位说明
    #endregion

    void Awake()
    {
        Instance = this;

        gm = FindObjectOfType<GameManager>();
    }

    #region 菜单功能
    //悔棋
    public void RegretBtn()
    {
        Debug.Log("悔棋");
    }

    //重新开始
    public void RestartBtn()
    {
        Debug.Log("重新开始");

        //如果已经获得了胜利 则保存分数
        if (gm.won)
        {
            PlayerPrefs.SetInt("Account", Money.Instance.Numerical);
        }

        Application.LoadLevel(Application.loadedLevel); //重新开始
    }

    //重置本关
    public void ResetBtn()
    {
        Debug.Log("重置");

        //清空一切资产
        PlayerPrefs.SetInt("Account", 100);

        Application.LoadLevel(Application.loadedLevel);
    }
    #endregion

    #region 菜单
    //菜单界面
    public void OptionsOn()
    {
        defaultStyle();                 //恢复菜单的默认状态

        OptionsPanel.SetActive(true);   //将菜单设为可见

        gm.State = GameState.GameSuspension;    //暂停接受输入
        //gm.over = false;
    }
    //关闭菜单
    public void Close()
    {
        OptionsPanel.SetActive(false);  //关闭菜单面板

        gm.State = GameState.Playing;   //重新接受输入
        //gm.over = true;
    }

    //菜单的默认样式
    private void defaultStyle()
    {
        RuleText.SetActive(false);          //关闭规则文字
        IntroductionText.SetActive(false);  //关闭说明文字
        ButtonList.SetActive(true);         //恢复菜单面板上按键的显示
    }

    //说明文字
    public void RuleOn()
    {
        RuleText.SetActive(true);       //显示说明文字
        ButtonList.SetActive(false);    //隐藏菜单面板上的按键
        gm.State = GameState.GameSuspension;    //暂停接受输入
    }
    //介绍文字
    public void IntroductionOn()
    {
        IntroductionText.SetActive(true);
        ButtonList.SetActive(false);
        gm.State = GameState.GameSuspension;
    }

    //显示玩法
    public void TempRule()
    {
        defaultStyle();
        RuleOn();
        OptionsPanel.SetActive(true);
    }
    //单位说明
    public void TempIntro()
    {
        defaultStyle();
        IntroductionOn();
        OptionsPanel.SetActive(true);
    }
    #endregion

    #region 游戏结束后的行为
    //失败提示界面
    public void GameOver(string s1, string s2)
    {
        //over = false;
        gm.State = GameState.GameSuspension;

        //如果失败时资产小于0 则将其归为100
        //if(Money.Instance.Numerical < 0)
        //{
        //    PlayerPrefs.SetInt("Account", 100);
        //}

        //关闭其他一切面板
        OptionsPanel.SetActive(false);

        TitleText.text = "Game Over";
        ContinueBtn.SetActive(false);
        GameOverPanel.SetActive(true);

        PlayerPrefs.SetInt("Account", Money.Instance.Numerical);    //记录游戏失败时的资源

        MessageText.text = s1 + s2;
    }
    //游戏胜利界面
    public void YouWon()
    {
        //over = false;                   //禁止接受输入
        gm.State = GameState.GameSuspension;

        OptionsPanel.SetActive(false);  //关闭其他面板

        TitleText.text = "You Won";     //标题文字改为胜利
        ContinueBtn.SetActive(true);    //显示继续按钮
        GameOverPanel.SetActive(true);  //开启提示面板

        int score = Money.Instance.Numerical - PlayerPrefs.GetInt("Account");   //分数 = 过关后金钱 - 上一局剩余金钱
        PlayerPrefs.SetInt("Account", Money.Instance.Numerical);                //胜利后更新剩余金钱
        //MessageText.text = "你赚到了\n" + score + "\n的金钱";                   //显示分数
        MessageText.text = "あなたは\n" + score + "\nのお金を手に入れた";
    }

    //继续游戏
    public void ContinueGame()
    {
        Debug.Log("继续游戏");
        gm.won = true;    //本关内不再显示胜利面板
        //over = true;   //游戏继续
        gm.State = GameState.Playing;
        GameOverPanel.SetActive(false);
    }
    #endregion

}
