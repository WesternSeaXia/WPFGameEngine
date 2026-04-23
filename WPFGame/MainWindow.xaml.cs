using System.Windows;
using System.Windows.Input;
using WPFGame.Core;
using WPFGame.Entities.Characters;
using WPFGame.Entities.Environment;
using WPFGame.Inputs;
using WPFGame.Rendering;

namespace WPFGame
{
    public partial class MainWindow : Window
    {
        private GameEngine _engine;
        private GameCanvas _canvas;

        public MainWindow()
        {
            InitializeComponent();

            // 1. 设置窗体背景色为黑色（这样等比例缩放留出的边框就是黑边）
            this.Background = System.Windows.Media.Brushes.Black;

            // 2. 初始化画布，并【强制指定逻辑分辨率】
            _canvas = new GameCanvas();
            _canvas.ClipToBounds = true;

            this.Content = _canvas;

            // 2. 初始化引擎和摄像机
            var camera = new Camera();
            _engine = new GameEngine(_canvas, camera);

            // 3. 创建玩家和地图
            var player = new Player { X = 50, Y = 500, Width = 40, Height = 40 };
            _engine.SetPlayer(player);

            // 添加几个测试用的地板和跳台
            _engine.AddObject(new Platform { X = 0, Y = 100, Width = 800, Height = 50 });
            _engine.AddObject(new Platform { X = 300, Y = 300, Width = 150, Height = 20 });

            _engine.AddObject(new Coin { X = 350, Y = 450, Width = 20, Height = 20 });
            _engine.AddObject(new Spike { X = 500, Y = 150, Width = 20, Height = 20 });

            // 4. 启动主循环
            _engine.Start();
        }

        // ====== 就在这里绑按键！======
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