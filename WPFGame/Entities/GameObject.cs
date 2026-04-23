using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WPFGame.Rendering;

namespace WPFGame.Entities
{
    public abstract class GameObject
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsDestroyed { get; set; } = false;
        public Rect BoundingBox => new Rect(X, Y, Width, Height);
        public abstract void Update(double deltaTime);
        public abstract void OnDraw(DrawingContext dc, Rect screenBox);

        public void Render(DrawingContext dc)
        {
            // 在基类中统一计算屏幕坐标
            Rect screenBox = RenderPipeline.ToScreenSpace(this.BoundingBox);

            // 将计算好的屏幕坐标传递给具体的子类去画
            OnDraw(dc, screenBox);
        }
    }
}
