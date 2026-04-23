using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using WPFGame.Entities.Characters;
using WPFGame.Rendering;

namespace WPFGame.Entities.Environment
{
    public class Coin : GameTrigger
    {
        public override void OnTriggered(Player player)
        {
            // 给玩家加分 (假设你在 Player 类里加了 Score 属性)
            // player.Score += 10;

            this.IsDestroyed = true;
        }

        public override void OnDraw(DrawingContext dc, Rect screenBox)
        {
            // 画一个黄色的圆形代表金币 (使用 BoundingBox 的中心点)
            Point center = new Point(screenBox.X + screenBox.Width / 2, screenBox.Y + screenBox.Height / 2);
            dc.DrawEllipse(Brushes.Gold, null, center, screenBox.Width / 2, screenBox.Height / 2);
        }
    }
}
