using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WPFGame.Entities;
using WPFGame.Entities.Characters;
using WPFGame.Entities.Environment;
using WPFGame.Physics;
using WPFGame.Rendering;

namespace WPFGame.Core
{
    public class GameEngine
    {
        private readonly GameCanvas canvas;
        private readonly Camera camera;
        private long lastFrameTime;

        private Player player;
        private List<GameObject> gameObjects = new List<GameObject>();

        // 用于计算 FPS
        private int frameCount = 0;
        private double fpsTimer = 0;
        private int currentFPS = 0;

        public GameEngine(GameCanvas canvas, Camera camera)
        {
            this.canvas = canvas;
            this.camera = camera;
        }

        public void SetPlayer(Player player)
        {
            this.player = player;
            gameObjects.Add(player);
        }

        public void Start()
        {
            lastFrameTime = DateTime.Now.Ticks;
            CompositionTarget.Rendering += GameLoop;
        }

        public void AddObject(GameObject obj)
        {
            gameObjects.Add(obj);
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (canvas.ActualHeight <= 0 || canvas.ActualWidth <= 0) return;

            long currentTicks = DateTime.Now.Ticks;
            double deltaTime = (currentTicks - lastFrameTime) / 10000000.0;
            lastFrameTime = currentTicks;
            if (deltaTime > 0.1) deltaTime = 0.016;

            // ==========================================
            // FPS 计算逻辑
            // ==========================================
            frameCount++;
            fpsTimer += deltaTime;
            if (fpsTimer >= 1.0)
            {
                currentFPS = frameCount;
                frameCount = 0;
                fpsTimer -= 1.0;
            }

            // 计算缩放比例和视野宽度
            double scale = canvas.ActualHeight / GlobalConfig.TargetLogicalHeight;
            double logicalWidth = canvas.ActualWidth / scale;

            // ====== 执行 Update ======
            foreach (var obj in gameObjects)
            {
                obj.Update(deltaTime);
            }

            // ====== 执行 Physics ======
            var platforms = gameObjects.OfType<Platform>();
            if (player != null)
            {
                CollisionHelper.MoveAndCollide(player, platforms, deltaTime);
            }

            var triggers = gameObjects.OfType<GameTrigger>();
            if (player != null)
            {
                CollisionHelper.CheckTriggers(player, triggers);
            }

            // ====== 垃圾回收 ======
            gameObjects.RemoveAll(obj => obj.IsDestroyed);

            // ====== 执行 Camera ======
            if (player != null)
            {
                camera.SetTarget(player.X, player.Y, logicalWidth, GlobalConfig.TargetLogicalHeight);
                camera.Update(deltaTime);
            }

            // ====== 执行 Render ======
            using (DrawingContext dc = canvas.GetDrawingContext())
            {
                dc.DrawRectangle(Brushes.Black, null, new Rect(0, 0, canvas.ActualWidth, canvas.ActualHeight));

                dc.PushTransform(new ScaleTransform(scale, scale));
                dc.PushTransform(new MatrixTransform(camera.Transform));

                foreach (var obj in gameObjects)
                {
                    obj.Render(dc);
                }

                // C. 弹出摄像机矩阵！(回到屏幕逻辑空间)
                dc.Pop();

                // --- 绘制 UI (不受摄像机影响，永远固定在屏幕上) ---
                DrawUI(dc, logicalWidth);

                // D. 弹出全局缩放
                dc.Pop();
            }
        }

        // ==========================================
        // UI 绘制专门方法
        // ==========================================
        private void DrawUI(DrawingContext dc, double logicalWidth)
        {
            if (player == null) return;

            // UI 层的高度依然受全局配置控制
            double logicalHeight = GlobalConfig.TargetLogicalHeight;

            double dpi = VisualTreeHelper.GetDpi(canvas).PixelsPerDip;
            Typeface typeface = new Typeface(new FontFamily("Microsoft YaHei"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);

            // ----------------------------------------------------
            // 1. 画 FPS (锚点：左上角 TopLeft)
            // ----------------------------------------------------
            FormattedText fpsText = new FormattedText($"FPS: {currentFPS}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 24, Brushes.LimeGreen, dpi);

            // 使用布局引擎计算矩形，向内缩进 20 像素
            Rect fpsRect = UILayout.GetRect(UIAnchor.TopLeft, logicalWidth, logicalHeight, fpsText.Width, fpsText.Height, 20, 20);
            dc.DrawText(fpsText, fpsRect.TopLeft);

            // ----------------------------------------------------
            // 2. 画金币数 (锚点：左下角 BottomLeft)
            // ----------------------------------------------------
            FormattedText coinText = new FormattedText($"💰 金币: {player.Coins}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 36, Brushes.Gold, dpi);

            // 锚点设为左下角，距离左边 20，距离底部 40
            Rect coinRect = UILayout.GetRect(UIAnchor.BottomLeft, logicalWidth, logicalHeight, coinText.Width, coinText.Height, 20, 40);
            dc.DrawText(coinText, coinRect.TopLeft);

            // ----------------------------------------------------
            // 3. 画血条 (锚点：右上角 TopRight)
            // ----------------------------------------------------
            double barWidth = 300;
            double barHeight = 30;

            // 锚点设为右上角，距离右边 40，距离顶部 40
            Rect hpBarRect = UILayout.GetRect(UIAnchor.TopRight, logicalWidth, logicalHeight, barWidth, barHeight, 40, 40);

            double hpPercent = (double)player.CurrentHealth / player.MaxHealth;
            Rect currentHpRect = new Rect(hpBarRect.X, hpBarRect.Y, hpBarRect.Width * hpPercent, hpBarRect.Height);

            dc.DrawRectangle(Brushes.DarkGray, new Pen(Brushes.White, 3), hpBarRect);
            dc.DrawRectangle(Brushes.Red, null, currentHpRect);

            // 血量文字居中对齐到血条矩形
            FormattedText hpText = new FormattedText($"HP: {player.CurrentHealth} / {player.MaxHealth}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 20, Brushes.White, dpi);
            Point textPos = new Point(hpBarRect.X + (hpBarRect.Width - hpText.Width) / 2, hpBarRect.Y + 2);
            dc.DrawText(hpText, textPos);
        }
    }
}