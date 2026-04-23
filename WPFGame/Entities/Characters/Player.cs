using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WPFGame.Core;
using WPFGame.Inputs;
using WPFGame.Rendering;

namespace WPFGame.Entities.Characters
{
    public class Player : GameObject
    {
        public double VelocityX { get; set; } // 水平速度
        public double VelocityY { get; set; } // 垂直速度

        private const double MoveSpeed = 300.0; // 移动速度
        private const double JumpForce = 600.0; // 跳跃力度

        // 【新增】：最大下落速度 (终端速度)
        private const double MaxFallSpeed = -1500.0;

        public bool IsGrounded { get; set; } // 是否在地面上

        public override void Update(double deltaTime)
        {
            // 左右移动
            if (KeyManager.IsKeyDown(Key.A)) VelocityX = -MoveSpeed;
            else if (KeyManager.IsKeyDown(Key.D)) VelocityX = MoveSpeed;
            else VelocityX = 0;

            // 跳跃 (只有在地上才能跳)
            if (KeyManager.IsKeyDown(Key.K) && IsGrounded)
            {
                VelocityY = JumpForce;
                IsGrounded = false;
            }

            // 应用重力自由落体
            VelocityY += GlobalConfig.Gravity * deltaTime;

            // ==========================================
            // 【新增】：限制最大下落速度
            // ==========================================
            if (VelocityY < MaxFallSpeed)
            {
                VelocityY = MaxFallSpeed;
            }
        }

        public override void OnDraw(DrawingContext dc, Rect screenBox)
        {
            dc.DrawRectangle(Brushes.Blue, null, screenBox);
        }
    }
}
