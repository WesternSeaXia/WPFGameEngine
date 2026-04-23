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

            // 【核心修复】：生成一个上下各收缩 1 像素的专用 X 轴检测框
            // 这样在地板上摩擦时，就不会把地板误认成墙壁了
            Rect pRectX = player.BoundingBox;
            pRectX.Inflate(0, -1);

            foreach (var platform in platforms)
            {
                Rect tRect = platform.BoundingBox;
                if (pRectX.IntersectsWith(tRect))
                {
                    if (player.VelocityX > 0)
                        player.X = tRect.Left - player.Width;
                    else if (player.VelocityX < 0)
                        player.X = tRect.Right;

                    player.VelocityX = 0;

                    // 修正后重新获取并收缩包围盒，用于下一次循环
                    pRectX = player.BoundingBox;
                    pRectX.Inflate(0, -1);
                }
            }

            // ==========================================
            // 第二步：只在 Y 轴移动并检测踩地/顶头
            // ==========================================
            player.IsGrounded = false;
            player.Y += player.VelocityY * deltaTime;

            // 【核心修复】：生成一个左右各收缩 1 像素的专用 Y 轴检测框
            // 防止贴着墙壁下落时，把墙壁的侧面误认成可以踩的地板
            Rect pRectY = player.BoundingBox;
            pRectY.Inflate(-1, 0);

            foreach (var platform in platforms)
            {
                Rect tRect = platform.BoundingBox;
                if (pRectY.IntersectsWith(tRect))
                {
                    // 【关键修改】：VelocityY < 0 才是受到重力往下掉
                    if (player.VelocityY < 0)
                    {
                        // 踩到了平台！
                        // 在新坐标系中，玩家的底部 (Y) 等于 平台的顶部 (平台Y + 平台高度)
                        player.Y = platform.Y + platform.Height;
                        player.VelocityY = 0;
                        player.IsGrounded = true; // 恢复跳跃权限
                    }
                    // 【关键修改】：VelocityY > 0 才是向上跳跃
                    else if (player.VelocityY > 0)
                    {
                        // 顶到了天花板！
                        // 在新坐标系中，玩家的顶部 (玩家Y + 玩家高度) 等于 平台的底部 (平台Y)
                        player.Y = platform.Y - player.Height;
                        player.VelocityY = 0;
                    }

                    pRectY = player.BoundingBox;
                    pRectY.Inflate(-1, 0);
                }
            }
        }

        public static void CheckTriggers(Player player, IEnumerable<GameTrigger> triggers)
        {
            // 获取玩家当前的碰撞盒 (不需要缩放 Inflate，碰到边缘也算触发)
            Rect pRect = player.BoundingBox;

            foreach (var trigger in triggers)
            {
                // 如果触发器已经被吃掉了，或者没碰到，就直接跳过s
                if (!trigger.IsActive) continue;

                if (pRect.IntersectsWith(trigger.BoundingBox))
                {
                    // 触发事件！
                    trigger.OnTriggered(player);
                }
            }
        }
    }
}
