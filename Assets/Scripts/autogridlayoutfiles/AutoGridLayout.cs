using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]     //在编辑模式下执行
[AddComponentMenu("Layout/Auto Grid Layout Group", 152)]        //[添加组件菜单（“布局/自动网格布局组”，152）

public class AutoGridLayout : GridLayoutGroup
{
    [SerializeField]
    private bool m_IsColumn;    //以行/列为单位基准
    [SerializeField]
    private int m_Column = 1, m_Row = 1;    //用户定义的行数与列数

    //计算水平/垂直布局输入
    public override void CalculateLayoutInputHorizontal()
    {
        //调用父类的同名方法
        base.CalculateLayoutInputHorizontal();      //如果没有这一行 那么只会生成一个方块

        float iColumn = -1; //列数
        float iRow = -1;    //行数
        if (m_IsColumn)
        {
            iColumn = m_Column;
            if (iColumn <= 0)
            {
                iColumn = 1;    //如果没有指定数字 则默认为1
            }
            iRow = Mathf.CeilToInt(this.transform.childCount / iColumn);    //计算行数 = 向下取整（方块数量 / 列数）
        }
        else
        {
            iRow = m_Row;
            if (iRow <= 0)
            {
                iRow = 1;
            }
            iColumn = Mathf.CeilToInt(this.transform.childCount / iRow);
        }

        //计算出可用于填充的空白部分的尺寸（从整体中减去方块间的间距和填充）
        float fHeight = (rectTransform.rect.height - ((iRow - 1) * (spacing.y))) - ((padding.top + padding.bottom));    //高 = （整体高 -（行数 * 纵向间距）） - （上/下 填充）
        float fWidth = (rectTransform.rect.width - ((iColumn - 1) * (spacing.x))) - ((padding.right + padding.left));   //宽 = （整体宽 -（列数 - 横向间距）） - （左/右 填充）

        //计算单个方块的尺寸 并将计算出来的尺寸赋值给方块
        Vector2 vSize = new Vector2(fWidth / iColumn, (fHeight) / iRow);    //方块宽 = 空余宽 / 列数， 方块高 = 空余高 / 行数
        cellSize = vSize;
    }
}
