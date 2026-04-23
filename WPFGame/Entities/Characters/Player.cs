using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WPFGame.Core;
using WPFGame.Inputs;
using WPFGame.Rendering;

namespace WPFGame.Entities.Characters
{
    // Player 类继承自 GameObject，代表游戏中的玩家角色
    public class Player : GameObject
    {
        // ==========================================
        // 动态物理状态
        // ==========================================
        // X 轴（水平方向）的当前瞬时速度。正数向右，负数向左。
        public double VelocityX { get; set; }
        
        // Y 轴（垂直方向）的当前瞬时速度。正数向上，负数向下（因为重力是负数）。
        public double VelocityY { get; set; }

        // ==========================================
        // 玩家能力常量 (魔术数字的收束)
        // ==========================================
        // 按下方向键时，赋予玩家的绝对移动速度。
        private const double MoveSpeed = 300.0;
        
        // 按下跳跃键时，赋予玩家向上的瞬时初速度。
        private const double JumpForce = 600.0;
        
        // 玩家在空中自由落体时能达到的最大向下速度（防穿模的终端速度）。
        private const double MaxFallSpeed = -1500.0;

        // ==========================================
        // 状态标记
        // ==========================================
        // 核心标记：玩家当前是否踩在坚固的平台上（由 CollisionHelper 在碰撞检测后设置）。
        public bool IsGrounded { get; set; }

        // ==========================================
        // 动画与视觉组件
        // ==========================================
        // 负责管理和播放帧序列的动画控制器。
        private Animator _animator;
        
        // 记录玩家当前的朝向。1 代表面向右（默认），-1 代表面向左。用于渲染时的贴图翻转。
        private int _facingDirection = 1;

        // 构造函数：在创建 Player 实例时执行，负责初始化所有属性、加载资源。
        public Player()
        {
            // 实例化动画控制器
            _animator = new Animator();
            
            // 从项目的 Assets 文件夹加载整张大贴图（Sprite Sheet）到内存中。
            var sheet = SpriteLoader.LoadImage("pack://application:,,,/Assets/Sprites/player_sheet.png");

            // ==========================================
            // 定义二次裁剪参数 (将多余的透明像素切掉)
            // ==========================================
            // 角色肉体比 98 窄，靠左边 29 像素
            int actualCropX = 29;

            // 角色脚底紧贴着格子的底线，所以相对底部的偏移是 0！(这比以前算距离头顶多少像素爽多了)
            int actualCropY = 15;

            int actualCropWidth = 40;
            int actualCropHeight = 49;

            // 加载 Idle 动画：第 0 行，从第 0 列开始，切 6 帧
            var idleFrames = SpriteLoader.Slice(
                sheet,
                cellWidth: 98, cellHeight: 64,
                row: 0, frameCount: 6,
                startCol: 0, // 新增的参数，指定从第 0 列开始
                cropX: actualCropX, cropY: actualCropY,
                cropWidth: actualCropWidth, cropHeight: actualCropHeight);

            _animator.AddAnimation("Idle", new Animation(idleFrames, 1.0 / 6.0));

            // 加载 Walk 动画：假设在第 1 行，且前面有 2 帧废弃不用的起步动作，我们想从第 2 列开始切 8 帧
            var walkFrames = SpriteLoader.Slice(
                sheet,
                cellWidth: 98, cellHeight: 64,
                row: 1, frameCount: 16,
                startCol: 2, // 比如这里你可以直接指定从第 2 列开始切了！
                cropX: actualCropX, cropY: actualCropY,
                cropWidth: actualCropWidth, cropHeight: actualCropHeight);

            _animator.AddAnimation("Walk", new Animation(walkFrames, 0.8 / 16.0));

            // 初始化后，默认播放站立动画。
            _animator.Play("Idle");

            // ==========================================
            // 架构核心：初始化物理体积与视觉大小 (完全解耦)
            // 此时角色的 X, Y 锚点永远固定在脚底中心。
            // ==========================================
            
            // 1. 设置物理判定框 (Hitbox) - 决定你能不能穿过墙、会不会踩空
            // 宽度设为 30。比视觉图像 (40) 还要瘦一点，这样即使角色的肩膀碰到墙，也不会卡住。
            this.HitboxWidth = 30;         
            // 高度设为 45。比视觉图像 (50) 矮一点，防止头顶的几根毛碰到天花板就被判定为撞头。
            this.HitboxHeight = 45;        

            // 2. 设置视觉贴图框 (VisualBox) - 决定画出来的图像有多大
            // 将视觉宽度设为裁剪图的 2 倍 (40 * 2 = 80)。这会在渲染时把切出来的原图放大一倍。
            this.VisualWidth = actualCropWidth * 2;   
            // 将视觉高度设为裁剪图的 2 倍 (50 * 2 = 100)。
            this.VisualHeight = actualCropHeight * 2; 

            // 3. 设置视觉偏移 (Offset)
            // 由于锚点在脚底中心，而视觉框的生成也是基于这个点居中、对齐底部的，
            // 所以即使你把图放大了两倍，它的脚底依然会完美踩在锚点上，水平也是完美居中的。
            // 因此偏移量设为 0 即可。
            this.VisualOffsetX = 0;
            this.VisualOffsetY = 0;
        }

        // ==========================================
        // 游戏主循环每帧调用的逻辑更新方法
        // deltaTime: 距离上一帧经过的时间（秒）
        // ==========================================
        public override void Update(double deltaTime)
        {
            // --- 处理键盘输入与水平移动 ---
            if (KeyManager.IsKeyDown(Key.A))
            {
                VelocityX = -MoveSpeed; // 向左的速度
                _facingDirection = -1;  // 更新朝向状态为左
                // 只有在踩着地板时才播放走路动画（防止在空中播放太空步）
                if (IsGrounded) _animator.Play("Walk");
            }
            else if (KeyManager.IsKeyDown(Key.D))
            {
                VelocityX = MoveSpeed;  // 向右的速度
                _facingDirection = 1;   // 更新朝向状态为右
                if (IsGrounded) _animator.Play("Walk");
            }
            else
            {
                // 如果既没按左也没按右，速度归零，停下
                VelocityX = 0;
                // 在地上停住时，播放呼吸动画
                if (IsGrounded) _animator.Play("Idle");
            }

            // --- 处理跳跃 ---
            // 只有按下跳跃键 K 且 当前站在地板上时，才允许起跳
            if (KeyManager.IsKeyDown(Key.K) && IsGrounded)
            {
                VelocityY = JumpForce; // 赋予一个巨大的向上初速度
                IsGrounded = false;    // 状态立刻变为悬空
            }

            // --- 处理重力 ---
            // 速度 = 速度 + 加速度 * 时间。这里将全局配置的重力加到 Y 轴速度上
            VelocityY += GlobalConfig.Gravity * deltaTime;

            // 限制最大下落速度，防止因为下落太快导致穿过地板
            if (VelocityY < MaxFallSpeed)
            {
                VelocityY = MaxFallSpeed;
            }

            // --- 更新动画状态 ---
            // 推进动画时间轴，决定当前应该显示第几帧
            _animator.Update(deltaTime);
        }

        // ==========================================
        // 渲染管线调用的具体绘制方法
        // physicalBox: 转换到屏幕坐标后的物理碰撞框
        // visualBox: 转换到屏幕坐标后的视觉贴图框
        // ==========================================
        protected override void OnDraw(DrawingContext dc, Rect physicalBox, Rect visualBox)
        {
            // 向 Animator 索要当前时间点应该画出的那一张图像
            var currentFrame = _animator.GetCurrentFrame();

            // 如果成功拿到了图像（资源加载正常）
            if (currentFrame != null)
            {
                // 1. 保存 WPF 绘图上下文当前的“干净”矩阵状态
                dc.PushTransform(new MatrixTransform(Matrix.Identity));

                // 2. 处理转身（翻转）逻辑
                if (_facingDirection == -1) // 如果玩家面朝左
                {
                    // 获取玩家脚底锚点(X, Y)转换到屏幕上的真实 X 坐标。
                    // 为什么要用这个？因为我们希望角色围绕它的“脊柱中心”原地转身，而不是围绕图片中心。
                    double screenCenterX = RenderPipeline.ToScreenSpace(new Rect(this.X, this.Y, 0, 0)).X;
                    
                    // 推入一个缩放变换矩阵：
                    // -1 代表 X 轴完全镜像翻转
                    // 1 代表 Y 轴保持原样
                    // screenCenterX 和 visualBox.Y 是翻转的中心轴点（即锚点在屏幕上的位置）
                    dc.PushTransform(new ScaleTransform(-1, 1, screenCenterX, visualBox.Y));
                }

                // 3. 执行最终绘制
                // 将那一小块图像，严丝合缝地填入我们算好的视觉框(visualBox)中。
                // 此时，因为前面推入了变换矩阵，如果朝左，画出来就是反的。
                dc.DrawImage(currentFrame, visualBox);

                // 4. 清理现场
                // 如果刚刚执行了翻转，把翻转矩阵弹出去
                if (_facingDirection == -1) dc.Pop();
                // 把之前保存的干净矩阵弹出去，恢复初始状态，保证不影响下一个物体的绘制
                dc.Pop();

                // 调试代码（注释状态）：可以取消注释来看看红色的真实碰撞箱在哪
                // dc.DrawRectangle(null, new Pen(Brushes.Red, 1), physicalBox);
                // dc.DrawRectangle(null, new Pen(Brushes.Blue, 1), visualBox);
            }
            else
            {
                // 容错处理：如果图片文件找不到或者读取失败，画一个蓝色方块代替，防止游戏黑屏。
                // 这里用 physicalBox 画，方便看出实际的碰撞体积。
                dc.DrawRectangle(Brushes.Blue, null, physicalBox);
            }
        }
    }
}