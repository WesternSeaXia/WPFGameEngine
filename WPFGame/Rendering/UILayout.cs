using System.Windows;

namespace WPFGame.Rendering
{
    // 定义经典的 9 宫格锚点
    public enum UIAnchor
    {
        TopLeft, TopCenter, TopRight,
        MiddleLeft, Center, MiddleRight,
        BottomLeft, BottomCenter, BottomRight
    }

    public static class UILayout
    {
        /// <summary>
        /// 根据锚点计算出 UI 元素的绝对屏幕矩形
        /// </summary>
        /// <param name="anchor">锚点位置</param>
        /// <param name="screenWidth">当前屏幕逻辑宽度</param>
        /// <param name="screenHeight">当前屏幕逻辑高度</param>
        /// <param name="elementWidth">UI 控件的宽度</param>
        /// <param name="elementHeight">UI 控件的高度</param>
        /// <param name="offsetX">向内的水平偏移（外边距）</param>
        /// <param name="offsetY">向内的垂直偏移（外边距）</param>
        /// <returns>最终绘制时使用的 Rect</returns>
        public static Rect GetRect(UIAnchor anchor, double screenWidth, double screenHeight, double elementWidth, double elementHeight, double offsetX = 0, double offsetY = 0)
        {
            double x = 0;
            double y = 0;

            // ==========================================
            // X 轴定位
            // ==========================================
            switch (anchor)
            {
                case UIAnchor.TopLeft:
                case UIAnchor.MiddleLeft:
                case UIAnchor.BottomLeft:
                    x = offsetX; // 靠左，往右偏移
                    break;
                case UIAnchor.TopCenter:
                case UIAnchor.Center:
                case UIAnchor.BottomCenter:
                    x = (screenWidth - elementWidth) / 2 + offsetX; // 居中
                    break;
                case UIAnchor.TopRight:
                case UIAnchor.MiddleRight:
                case UIAnchor.BottomRight:
                    x = screenWidth - elementWidth - offsetX; // 靠右，往左推（缩进）
                    break;
            }

            // ==========================================
            // Y 轴定位
            // ==========================================
            switch (anchor)
            {
                case UIAnchor.TopLeft:
                case UIAnchor.TopCenter:
                case UIAnchor.TopRight:
                    y = offsetY; // 靠上，往下偏移
                    break;
                case UIAnchor.MiddleLeft:
                case UIAnchor.Center:
                case UIAnchor.MiddleRight:
                    y = (screenHeight - elementHeight) / 2 + offsetY; // 居中
                    break;
                case UIAnchor.BottomLeft:
                case UIAnchor.BottomCenter:
                case UIAnchor.BottomRight:
                    y = screenHeight - elementHeight - offsetY; // 靠下，往上推（缩进）
                    break;
            }

            return new Rect(x, y, elementWidth, elementHeight);
        }
    }
}