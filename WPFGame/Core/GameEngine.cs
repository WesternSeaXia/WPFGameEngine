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

        private readonly UIManager uiManager;
        public UIManager UI => uiManager;

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
            this.uiManager = new UIManager();
        }

        // 清空当前世界的所有物体 (切关卡必备)
        public void ClearWorld()
        {
            gameObjects.Clear();
            player = null;
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
                double dpi = VisualTreeHelper.GetDpi(canvas).PixelsPerDip;
                uiManager.DrawHUD(dc, player, logicalWidth, GlobalConfig.TargetLogicalHeight, currentFPS, dpi);

                // D. 弹出全局缩放
                dc.Pop();
            }
        }
    }
}