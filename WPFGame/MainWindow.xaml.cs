using System.Windows;
using System.Windows.Input;
using WPFGame.Core;
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

            this.Background = System.Windows.Media.Brushes.Black;

            _canvas = new GameCanvas();
            _canvas.ClipToBounds = true;
            this.Content = _canvas;

            var camera = new Camera();
            _engine = new GameEngine(_canvas, camera);

            // ==========================================
            // 用 ASCII 艺术构建关卡
            // P = 玩家, # = 地块, = = 半高跳台, C = 金币, S = 尖刺
            // ==========================================
            string[] level1 = new string[]
            {
                "                                                  ",
                "                                                  ",
                "                   C C C                          ",
                "                  =======                         ",
                "                                            C     ",
                "           C                            ========= ",
                "         #####               C                    ",
                "                           #####   ########       ",
                "               C  S   C                           ",
                " P       #################                        ",
                "###                                            ###",
                "        ####                S S S S S S S S S S S ",
                "##################################################################################################"
            };

            // 一键加载地图！
            LevelGenerator.LoadMap(_engine, level1);

            // 启动游戏主循环
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