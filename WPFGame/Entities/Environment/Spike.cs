using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WPFGame.Entities.Characters;
using WPFGame.Rendering;

namespace WPFGame.Entities.Environment
{
    public class Spike : GameTrigger
    {
        public override void OnTriggered(Player player)
        {
            // 陷阱碰到不会消失，所以不改变 IsActive

            // 惩罚机制：把玩家强制传送回起点，并清空速度
            player.X = 50;
            player.Y = 500;
            player.VelocityX = 0;
            player.VelocityY = 0;
        }

        public override void OnDraw(DrawingContext dc, Rect screenBox)
        {
            dc.DrawRectangle(Brushes.DarkRed, null, screenBox);
        }
    }
}
