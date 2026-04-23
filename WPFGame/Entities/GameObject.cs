using System.Windows;
using System.Windows.Media;
using WPFGame.Rendering;

namespace WPFGame.Entities
{
    public abstract class GameObject
    {
        // ==========================================
        // 灵魂锚点 (Anchor)：绝对真实的世界坐标，永远在脚底中心
        // ==========================================
        public double X { get; set; }
        public double Y { get; set; }

        public bool IsDestroyed { get; set; } = false;

        // ==========================================
        // 物理组件 (Hitbox)
        // ==========================================
        public double HitboxWidth { get; set; }
        public double HitboxHeight { get; set; }

        // 生成物理包围盒：X向左偏移一半，Y贴地
        public Rect BoundingBox => new Rect(X - HitboxWidth / 2, Y, HitboxWidth, HitboxHeight);

        // ==========================================
        // 视觉组件 (Visuals)
        // ==========================================
        public double VisualWidth { get; set; }
        public double VisualHeight { get; set; }
        public double VisualOffsetX { get; set; } = 0;
        public double VisualOffsetY { get; set; } = 0;

        public abstract void Update(double deltaTime);
        protected abstract void OnDraw(DrawingContext dc, Rect physicalBox, Rect visualBox);

        public void Render(DrawingContext dc)
        {
            // 1. 计算物理框屏幕坐标
            Rect physicalScreenBox = RenderPipeline.ToScreenSpace(this.BoundingBox);

            // 2. 生成视觉包围盒：同样以脚底中心 X 为基准向左偏移一半
            Rect visualWorldBox = new Rect(
                (X - VisualWidth / 2) + VisualOffsetX,
                Y + VisualOffsetY,
                VisualWidth,
                VisualHeight
            );

            // 3. 将视觉框转换为屏幕坐标
            Rect visualScreenBox = RenderPipeline.ToScreenSpace(visualWorldBox);

            OnDraw(dc, physicalScreenBox, visualScreenBox);
        }
    }
}