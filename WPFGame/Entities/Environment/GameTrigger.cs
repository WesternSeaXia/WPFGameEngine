using System;
using System.Collections.Generic;
using System.Text;
using WPFGame.Entities.Characters;

namespace WPFGame.Entities.Environment
{
    // 触发器基类
    public abstract class GameTrigger : GameObject
    {
        // 标记这个触发器是否还有效（比如金币吃完一次就失效了）
        public bool IsActive { get; set; } = true;

        // 当玩家碰到这个区域时，会调用这个方法
        public abstract void OnTriggered(Player player);

        // 触发器默认不需要 Update 逻辑，子类有需要可以重写
        public override void Update(double deltaTime) { }
    }
}
