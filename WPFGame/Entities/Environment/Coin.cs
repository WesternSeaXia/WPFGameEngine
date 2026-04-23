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
            player.Coins += 1;   // 玩家金币 +1
            this.IsDestroyed = true; // 标记为销毁，引擎的 Sweep 阶段会把它清理掉
            this.IsActive = false;   // 防止同一帧内触发两次
        }

        protected override void OnDraw(DrawingContext dc, Rect physicalBox, Rect visualBox)
        {
            // 画一个黄色的圆形代表金币 (使用 BoundingBox 的中心点)
            Point center = new Point(physicalBox.X + physicalBox.Width / 2, physicalBox.Y + physicalBox.Height / 2);
            dc.DrawEllipse(Brushes.Gold, null, center, physicalBox.Width / 2, physicalBox.Height / 2);
        }
    }
}
