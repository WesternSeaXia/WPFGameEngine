using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WPFGame.Inputs
{
    public static class KeyManager
    {
        // 存储当前所有被按下的键
        private static readonly HashSet<Key> _pressedKeys = new HashSet<Key>();

        // 在 App.xaml.cs 或 MainWindow 中绑定这两个事件
        public static void OnKeyDown(Key key) => _pressedKeys.Add(key);
        public static void OnKeyUp(Key key) => _pressedKeys.Remove(key);

        public static bool IsKeyDown(Key key) => _pressedKeys.Contains(key);
    }
}
