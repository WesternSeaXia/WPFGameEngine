using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using WPFGame.Core;

namespace WPFGame.Rendering
{
    public static class RenderPipeline
    {

        // 核心中间件：将世界坐标转换为 WPF 的屏幕绘制坐标
        public static Rect ToScreenSpace(Rect worldBounds)
        {
            // 【修改】：直接使用 GlobalConfig.TargetLogicalHeight
            double screenY = GlobalConfig.TargetLogicalHeight - worldBounds.Y - worldBounds.Height;

            return new Rect(worldBounds.X, screenY, worldBounds.Width, worldBounds.Height);
        }
    }
}
