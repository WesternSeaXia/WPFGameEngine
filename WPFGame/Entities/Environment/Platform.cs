using System.Windows;
using System.Windows.Media;
using WPFGame.Rendering;

namespace WPFGame.Entities.Environment
{
    public class Platform : GameObject
    {
        public override void Update(double deltaTime) { /* 静态物体无需更新 */ }

        public override void OnDraw(DrawingContext dc, Rect screenBox)
        {
            dc.DrawRectangle(Brushes.Gray, null, screenBox);
        }
    }
}
