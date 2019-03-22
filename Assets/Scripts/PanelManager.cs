using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    private GameManager gm;

    public GameObject OptionsPanel;     //菜单面板
    public GameObject ButtonList;       //默认的按钮面板
    public GameObject RuleText;         //规则说明
    public GameObject IntroductionText; //单位说明

    void Awake()
    {
        //获取到gamemanager
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
    }
    //介绍文字
    public void IntroductionOn()
    {
        IntroductionText.SetActive(true);
        ButtonList.SetActive(false);
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
}
