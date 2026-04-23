using System.Globalization;
using System.Windows;
using System.Windows.Media;
using WPFGame.Core;
using WPFGame.Entities.Characters;
using WPFGame.Rendering.UI;

namespace WPFGame.Rendering
{
    public class UIManager
    {
        // 缓存字体对象，避免每帧重复创建引发 GC (垃圾回收) 压力
        private readonly Typeface _typeface;

        // 按钮列表
        private List<UIButton> _buttons = new List<UIButton>();

        public UIManager()
        {
            _typeface = new Typeface(new FontFamily("Microsoft YaHei"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
        }

        // 添加按钮
        public void AddButton(UIButton button)
        {
            _buttons.Add(button);
        }

        // 清空按钮 (切换关卡时很有用)
        public void ClearButtons()
        {
            _buttons.Clear();
        }

        // ==========================================
        // 【新增】：鼠标点击检测
        // ==========================================
        public bool HandleClick(Point mousePosition)
        {
            foreach (var btn in _buttons)
            {
                // 如果鼠标点击坐标在按钮的矩形范围内
                if (btn.CurrentScreenRect.Contains(mousePosition))
                {
                    btn.OnClick?.Invoke(); // 执行绑定的事件
                    return true; // 拦截点击，表示点到 UI 了，游戏世界不要响应
                }
            }
            return false;
        }

        /// <summary>
        /// 绘制抬头显示界面 (HUD)
        /// </summary>
        public void DrawHUD(DrawingContext dc, Player player, double logicalWidth, double logicalHeight, int currentFPS, double dpi)
        {
            if (player == null) return;

            // ----------------------------------------------------
            // 1. 画 FPS (左上角)
            // ----------------------------------------------------
            FormattedText fpsText = new FormattedText($"FPS: {currentFPS}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, _typeface, 24, Brushes.LimeGreen, dpi);
            Rect fpsRect = UILayout.GetRect(UIAnchor.TopLeft, logicalWidth, logicalHeight, fpsText.Width, fpsText.Height, 20, 20);
            dc.DrawText(fpsText, fpsRect.TopLeft);

            // ----------------------------------------------------
            // 2. 画金币数 (左下角)
            // ----------------------------------------------------
            FormattedText coinText = new FormattedText($"💰 金币: {player.Coins}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, _typeface, 36, Brushes.Gold, dpi);
            Rect coinRect = UILayout.GetRect(UIAnchor.BottomLeft, logicalWidth, logicalHeight, coinText.Width, coinText.Height, 20, 40);
            dc.DrawText(coinText, coinRect.TopLeft);

            // ----------------------------------------------------
            // 3. 画血条 (右上角)
            // ----------------------------------------------------
            double barWidth = 300;
            double barHeight = 30;

            Rect hpBarRect = UILayout.GetRect(UIAnchor.TopRight, logicalWidth, logicalHeight, barWidth, barHeight, 40, 40);

            double hpPercent = (double)player.CurrentHealth / player.MaxHealth;
            Rect currentHpRect = new Rect(hpBarRect.X, hpBarRect.Y, hpBarRect.Width * hpPercent, hpBarRect.Height);

            dc.DrawRectangle(Brushes.DarkGray, new Pen(Brushes.White, 3), hpBarRect);
            dc.DrawRectangle(Brushes.Red, null, currentHpRect);

            FormattedText hpText = new FormattedText($"HP: {player.CurrentHealth} / {player.MaxHealth}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, _typeface, 20, Brushes.White, dpi);
            Point textPos = new Point(hpBarRect.X + (hpBarRect.Width - hpText.Width) / 2, hpBarRect.Y + 2);
            dc.DrawText(hpText, textPos);

            // ==========================================
            // 【新增】：绘制所有交互按钮
            // ==========================================
            foreach (var btn in _buttons)
            {
                // 1. 计算按钮的绝对屏幕位置
                btn.CurrentScreenRect = UILayout.GetRect(btn.Anchor, logicalWidth, logicalHeight, btn.Width, btn.Height, btn.OffsetX, btn.OffsetY);

                // 2. 画按钮底色
                dc.DrawRectangle(Brushes.DarkSlateBlue, new Pen(Brushes.White, 2), btn.CurrentScreenRect);

                // 3. 画按钮文字
                FormattedText btnText = new FormattedText(btn.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, _typeface, 20, Brushes.White, dpi);

                // 文字居中算法
                textPos = new Point(
                    btn.CurrentScreenRect.X + (btn.Width - btnText.Width) / 2,
                    btn.CurrentScreenRect.Y + (btn.Height - btnText.Height) / 2
                );
                dc.DrawText(btnText, textPos);
            }
        }

    }
}