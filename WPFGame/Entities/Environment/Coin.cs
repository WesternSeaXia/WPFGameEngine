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

        protected override void OnDraw(DrawingContext dc, Rect physicalBox, Rect visualBox)
        {
            // 画一个黄色的圆形代表金币 (使用 BoundingBox 的中心点)
            Point center = new Point(physicalBox.X + physicalBox.Width / 2, physicalBox.Y + physicalBox.Height / 2);
            dc.DrawEllipse(Brushes.Gold, null, center, physicalBox.Width / 2, physicalBox.Height / 2);
        }
    }
}
