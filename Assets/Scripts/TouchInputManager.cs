using UnityEngine;
using System.Collections;

// 触屏支持
public class TouchInputManager : MonoBehaviour
{

    // https://pfonseca.com/swipe-detection-on-unity

    private float fingerStartTime = 0.0f;          //手指开始时间
    private Vector2 fingerStartPos = Vector2.zero;  //手指开始坐标

    private bool isSwipe = false;                   //是否发生了移动
    private float minSwipeDist = 50.0f;            //最小滑动区域
    private float maxSwipeTime = 1.5f;              //最大滑动时间

    private GameManager gm; //调用主控脚本

    void Awake()
    {
        //获取到组主控脚本
        gm = GameObject.FindObjectOfType<GameManager>();
    }

    void Update()
    {
        //若 此时处于可操作状态 且 触点数大于0个
        if (gm.State == GameState.Playing && Input.touchCount > 0)
        {
            //遍历每一个touches 接触
            foreach (Touch touch in Input.touches)
            {
                switch (touch.phase)    //phase 阶段、时期
                {
                    //1.触碰开始
                    case TouchPhase.Began:
                        isSwipe = true;                     //发生了移动
                        fingerStartTime = Time.time;        //记录起始时间
                        fingerStartPos = touch.position;    //记录起始坐标
                        break;

                    //2.触碰取消
                    case TouchPhase.Canceled:
                        isSwipe = false;    //没有发生移动（无视本次记录
                        break;

                    //3.触碰结束
                    case TouchPhase.Ended:
                        float gestureTime = Time.time - fingerStartTime;                    //手势时间 = 当前时间 - 起始时间
                        float gestureDist = (touch.position - fingerStartPos).magnitude;    //手势距离 = (当前坐标 - 起始坐标).大小

                        // 发生移动 and 手势时间不超过1.5s的定时 and 手势距离大于指定的最小距离
                        if (isSwipe && gestureTime < maxSwipeTime && gestureDist > minSwipeDist)
                        {
                            Vector2 direction = touch.position - fingerStartPos;    //方向 = 触摸坐标 - 起始坐标
                            Vector2 swipeType = Vector2.zero;                       //滑动类型 = Vector2(0, 0)

                            // 若 direction方向 的x值大于y值
                            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                            {
                                //滑动类型 = Vector2(1, 0) * ±1(x的变动方向)
                                swipeType = Vector2.right * Mathf.Sign(direction.x);
                            }
                            else
                            {
                                //滑动类型 = Vector2(0, 1) * ±1
                                swipeType = Vector2.up * Mathf.Sign(direction.y);
                            }

                            //在确定移动方向后，若 滑动类型的x 不等于0【被判定为水平移动
                            if (swipeType.x != 0.0f)
                            {
                                //若 滑动类型的x 大于零则向右移动
                                if (swipeType.x > 0.0f)
                                {
                                    // MOVE RIGHT
                                    gm.Move(MoveDirection.Right);
                                }
                                else
                                {
                                    // MOVE LEFT
                                    gm.Move(MoveDirection.Left);
                                }
                            }

                            //在确定移动方向后，若 滑动类型的y 不等于0【被判定为垂直移动
                            if (swipeType.y != 0.0f)
                            {
                                //若 滑动类型的y 大于零则向上移动
                                if (swipeType.y > 0.0f)
                                {
                                    // MOVE UP
                                    gm.Move(MoveDirection.Up);
                                }
                                else
                                {
                                    // MOVE DOWN
                                    gm.Move(MoveDirection.Down);
                                }
                            }

                        }
                        break;
                }
            }
        }

    }
}