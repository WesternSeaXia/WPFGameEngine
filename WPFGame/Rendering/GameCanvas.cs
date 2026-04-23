using System.Windows;
using System.Windows.Media;

namespace WPFGame.Rendering
{
    // 继承 FrameworkElement：这是为了让这个类能够被放在 WPF 的窗口（Window）中。
    // FrameworkElement 提供了最基础的尺寸（Width/Height）、生命周期和参与可视化树的能力，
    // 但它像一张白纸，没有任何默认的 UI 外观，这意味着极低的性能开销。
    public class GameCanvas : FrameworkElement
    {
        // 维护一个可视对象集合：
        // 虽然只是绘制，但 WPF 的底层渲染引擎必须通过“树”形结构来管理渲染对象。
        // 这个集合负责建立 GameCanvas（父节点）和 DrawingVisual（子节点）之间的关系
        private readonly VisualCollection children;
        // 核心渲染载体：
        // DrawingVisual 是 WPF 中极为轻量级的绘图对象。它不具备布局、事件处理等能力，
        // 只负责记录绘图指令（画线、画图、写字等），非常适合游戏这种需要每秒重绘 60 次的场景。
        public readonly DrawingVisual MainVisual;

        public GameCanvas()
        {
            // 强制边缘像素对齐，关闭抗锯齿，这是像素级游戏必须的
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            // 强制使用高质量位图缩放算法（如果后面要用像素图）
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);

            // 初始化集合，并将当前实例 (this) 作为逻辑上的父级
            children = new VisualCollection(this);
            // 实例化轻量级绘图层
            MainVisual = new DrawingVisual();
            // 将绘图层加入集合。这一步建立了逻辑上的父子关系，但还不够让 WPF 渲染它。
            children.Add(MainVisual);
        }

        // --- 必须重写的两个方法 ---
        // WPF 的渲染引擎（底层是基于 DirectX 的 Composition Target）在绘制画面时，
        // 会主动“询问”每个 UI 元素：“你有几个孩子？”以及“把你的孩子交给我看看”。

        // 告诉 WPF 渲染引擎：我这里有几个子元素需要被渲染。
        // 这里返回 _children.Count（目前是 1 个，即 MainVisual）。
        protected override int VisualChildrenCount => children.Count;

        // 告诉 WPF 渲染引擎：根据索引把具体的子元素返回给你。
        // 如果不重写这两个方法，WPF 引擎就“看”不到 _children 里的 DrawingVisual，屏幕上就什么都没有。
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            return children[index];
        }

        // --- 对外暴露的绘图接口 ---
        // 提供给 GameEngine (游戏主循环) 调用的方法。
        // 游戏引擎在每一帧（Frame）更新时，都会调用这个方法获取“画笔”。
        public DrawingContext GetDrawingContext()
        {
            // RenderOpen() 会返回一个 DrawingContext（类似于 GDI+ 的 Graphics 对象或 HTML5 的 Canvas Context）。
            // 它是用来下达具体绘制指令（如 DrawRectangle, DrawImage）的。
            // 【重要机制】：每次调用 RenderOpen()，都会清空该 DrawingVisual 之前所有的绘制内容。
            // 调用者拿到 DrawingContext 后，必须在使用完毕后调用其 Close() 方法。
            // 只有在 Close() 被调用时，所有累积的绘制指令才会被统一打包发送给 GPU 进行渲染。
            return MainVisual.RenderOpen();
        }
    }
}
