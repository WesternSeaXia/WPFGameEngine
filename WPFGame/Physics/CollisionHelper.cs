using System;
using System.Collections.Generic;
using System.Windows;
using WPFGame.Entities.Characters;
using WPFGame.Entities.Environment;

namespace WPFGame.Physics
{
    public static class CollisionHelper
    {
        public static void MoveAndCollide(Player player, IEnumerable<Platform> platforms, double deltaTime)
        {
            // ==========================================
            // 第一步：只在 X 轴移动并检测撞墙
            // ==========================================
            player.X += player.VelocityX * deltaTime;

            Rect pRectX = player.BoundingBox;
            pRectX.Inflate(0, -1);

            foreach (var platform in platforms)
            {
                Rect tRect = platform.BoundingBox;
                if (pRectX.IntersectsWith(tRect))
                {
                    if (player.VelocityX > 0)
                        // 向右撞墙，退回到墙的左侧，减去半个物理身位
                        player.X = tRect.Left - player.HitboxWidth / 2;
                    else if (player.VelocityX < 0)
                        // 向左撞墙，退回到墙的右侧，加上半个物理身位
                        player.X = tRect.Right + player.HitboxWidth / 2;

                    player.VelocityX = 0;

                    pRectX = player.BoundingBox;
                    pRectX.Inflate(0, -1);
                }
            }

            // ==========================================
            // 第二步：只在 Y 轴移动并检测踩地/顶头
            // ==========================================
            player.IsGrounded = false;
            player.Y += player.VelocityY * deltaTime;

            Rect pRectY = player.BoundingBox;
            pRectY.Inflate(-1, 0);

            foreach (var platform in platforms)
            {
                Rect tRect = platform.BoundingBox;
                if (pRectY.IntersectsWith(tRect))
                {
                    if (player.VelocityY < 0)
                    {
                        // 踩到了平台！Y 就是脚底，直接等于平台的顶部
                        player.Y = tRect.Y + tRect.Height;
                        player.VelocityY = 0;
                        player.IsGrounded = true;
                    }
                    else if (player.VelocityY > 0)
                    {
                        // 顶到了天花板！脚底 = 天花板底部 - 物理身高
                        player.Y = tRect.Y - player.HitboxHeight;
                        player.VelocityY = 0;
                    }

                    pRectY = player.BoundingBox;
                    pRectY.Inflate(-1, 0);
                }
            }
        }

        public static void CheckTriggers(Player player, IEnumerable<GameTrigger> triggers)
        {
            Rect pRect = player.BoundingBox;

            foreach (var trigger in triggers)
            {
                if (!trigger.IsActive) continue;

                if (pRect.IntersectsWith(trigger.BoundingBox))
                {
                    trigger.OnTriggered(player);
                }
            }
        }
    }
}