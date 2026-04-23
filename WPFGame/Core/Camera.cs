using System;
using System.Windows.Media;

namespace WPFGame.Core
{
    public class Camera
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        private double _targetX;
        private double _targetY;

        private double _idleTimer = 0;
        private const double FollowDelay = 0.1; // 0.1秒延迟起步

        // 基础平滑系数 (如果觉得跟不上玩家，就把这个值调大，比如 8.0 或 10.0)
        private const double BaseSmoothing = 5.0;

        private double _lastPlayerX;
        private double _lastPlayerY;

        public void SetTarget(double playerX, double playerY, double screenWidth, double screenHeight)
        {
            double goalX = playerX - screenWidth / 2;
            double goalY = playerY - screenHeight / 2.8;

            // 防止看到地图左边界外的黑边
            if (goalX < 0) goalX = 0;

            if (Math.Abs(playerX - _lastPlayerX) > 0.1 || Math.Abs(playerY - _lastPlayerY) > 0.1)
            {
                _targetX = goalX;
                _targetY = goalY;
                _lastPlayerX = playerX;
                _lastPlayerY = playerY;
            }
        }

        public void Update(double deltaTime)
        {
            bool needsToMoveX = Math.Abs(X - _targetX) > 1.0;
            bool needsToMoveY = Math.Abs(Y - _targetY) > 1.0;

            if (needsToMoveX || needsToMoveY)
            {
                _idleTimer += deltaTime;
            }
            else
            {
                _idleTimer = 0;
            }

            if (_idleTimer >= FollowDelay)
            {
                // 最纯粹、最丝滑的线性插值跟随
                X += (_targetX - X) * BaseSmoothing * deltaTime;
                Y += (_targetY - Y) * BaseSmoothing * deltaTime;

                // 地图左边界兜底
                if (X < 0) X = 0;
            }
        }

        public Matrix Transform => new TranslateTransform(-X, Y).Value;
    }
}