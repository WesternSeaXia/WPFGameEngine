using System;
using System.Collections.Generic;
using System.Text;

namespace WPFGame.Core
{
    public static class GlobalConfig
    {
        // ==========================================
        // 渲染与视口设置
        // ==========================================

        // 锁定游戏的基准逻辑高度
        public const double TargetLogicalHeight = 1000.0;

        // 瓦片地图的基础大小 (为后续做准备)
        public const double TileSize = 50.0;

        // ==========================================
        // 全局物理规则 (可以逐渐把实体里的硬编码移过来)
        // ==========================================

        // 真实的物理重力 (向下为负)
        public const double Gravity = -980.0;
    }
}
