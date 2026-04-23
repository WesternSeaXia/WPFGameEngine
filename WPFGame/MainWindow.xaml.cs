using System.Windows;
using System.Windows.Input;
using WPFGame.Core;
using WPFGame.Inputs;
using WPFGame.Rendering;
using WPFGame.Rendering.UI; // 引入 UI 命名空间

namespace WPFGame
{
    public partial class MainWindow : Window
    {
        private GameEngine _engine;
        private GameCanvas _canvas;

        public MainWindow()
        {
            InitializeComponent();
            this.Background = System.Windows.Media.Brushes.Black;

            _canvas = new GameCanvas();
            _canvas.ClipToBounds = true;

            _canvas.MouseLeftButtonDown += OnCanvasMouseClick;

            this.Content = _canvas;

            var camera = new Camera();
            _engine = new GameEngine(_canvas, camera);

            LoadLevel(1);
            _engine.Start();
        }

        // ==========================================
        // 鼠标点击事件路由
        // ==========================================
        private void OnCanvasMouseClick(object sender, MouseButtonEventArgs e)
        {
            // 获取鼠标在画布上的物理像素坐标
            Point physicalPos = e.GetPosition(_canvas);

            // ==========================================
            // 【核心修复 2】：物理坐标 -> 逻辑坐标 的降维打击
            // 算出当前的缩放比例，把鼠标坐标还原到 UI 的逻辑空间中
            // ==========================================
            double scale = _canvas.ActualHeight / GlobalConfig.TargetLogicalHeight;
            Point logicalPos = new Point(physicalPos.X / scale, physicalPos.Y / scale);

            // 把换算好的逻辑坐标丢给 UI 管理器
            _engine.UI.HandleClick(logicalPos);
        }

        // ==========================================
        // 关卡状态机 (Scene Manager 雏形)
        // ==========================================
        private void LoadLevel(int levelId)
        {
            // 1. 清空上一个关卡的所有数据
            _engine.ClearWorld();
            _engine.UI.ClearButtons();

            if (levelId == 1)
            {
                // 构建第一关地图
                string[] level1Map = {
                    "                                                  ",
                    "                                                  ",
                    " P       C   C   C                                ",
                    "###     ===========             C                 ",
                    "                               ###                ",
                    "                                                  ",
                    "##################################################"
                };
                LevelGenerator.LoadMap(_engine, level1Map);

                // 在第一关的【右下角】加一个“下一关”按钮
                var nextBtn = new UIButton
                {
                    Text = "前往第二关 ➡",
                    Anchor = UIAnchor.BottomRight, // 定位到右下角
                    Width = 150,
                    Height = 40,
                    OffsetX = 20,
                    OffsetY = 20,
                    // 【核心】：利用 Lambda 表达式注入回调函数
                    OnClick = () => LoadLevel(2)
                };
                _engine.UI.AddButton(nextBtn);
            }
            else if (levelId == 2)
            {
                // 构建第二关地图 (全是尖刺，高难度)
                string[] level2Map = {
                    "                                                  ",
                    "                                                  ",
                    "                                     C            ",
                    " P                                 #####          ",
                    "###         S S S        S S S                    ",
                    "           #######      #######                   ",
                    "##################################################"
                };
                LevelGenerator.LoadMap(_engine, level2Map);

                // 在第二关的【顶部居中】加一个“返回”按钮
                var backBtn = new UIButton
                {
                    Text = "⬅ 返回第一关",
                    Anchor = UIAnchor.TopCenter, // 定位到顶部居中
                    Width = 150,
                    Height = 40,
                    OffsetX = 0,
                    OffsetY = 20,
                    OnClick = () => LoadLevel(1)
                };
                _engine.UI.AddButton(backBtn);
            }
        }

        // 键盘事件保持不变...
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            KeyManager.OnKeyDown(e.Key);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            KeyManager.OnKeyUp(e.Key);
        }
    }
}