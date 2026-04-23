using System;
using System.Windows;
using System.Windows.Media;

namespace WPFGame.Rendering.UI
{
    public class UIButton
    {
        public string Text { get; set; }
        public UIAnchor Anchor { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }

        // 【核心】：C# 的 Action 委托，相当于函数指针。外界把方法塞进来，按钮被点时执行。
        public Action OnClick { get; set; }

        // 缓存本帧渲染时的实际屏幕位置，用于做鼠标碰撞检测
        public Rect CurrentScreenRect { get; set; }
    }
}