using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用一个【枚举】来对输入进分类
public enum MoveDirection
{
    Left, Right, Up, Down
}

///侦测输入
public class InputManager : MonoBehaviour
{

    private GameManager gm;     //用于调用gamemanager脚本的方法

    private void Awake()
    {
        //获取到gamemanager
        gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (gm.State == GameState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                gm.Move(MoveDirection.Right);   //右
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                gm.Move(MoveDirection.Left);    //左
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                gm.Move(MoveDirection.Up);      //上
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                gm.Move(MoveDirection.Down);    //下
            }
        }
    }
}
