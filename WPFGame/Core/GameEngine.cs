using System;
using System.Collections.Generic;
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

                dc.Pop();
                dc.Pop();
            }
        }
    }
}