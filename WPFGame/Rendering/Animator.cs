using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace WPFGame.Rendering
{
    public class Animator
    {
        private Dictionary<string, Animation> _animations = new Dictionary<string, Animation>();
        private Animation _currentAnimation;
        private string _currentState = "";

        private double _timer = 0;
        private int _currentFrameIndex = 0;

        public void AddAnimation(string stateName, Animation animation)
        {
            _animations[stateName] = animation;
        }

        public void Play(string stateName)
        {
            // 如果已经在播这个动画了，就不重置
            if (_currentState == stateName) return;

            if (_animations.ContainsKey(stateName))
            {
                _currentState = stateName;
                _currentAnimation = _animations[stateName];
                _currentFrameIndex = 0;
                _timer = 0;
            }
        }

        // 放在 Player 的 Update 里调用
        public void Update(double deltaTime)
        {
            if (_currentAnimation == null) return;

            _timer += deltaTime;

            // 如果时间超过了一帧的停留时间，就切换到下一帧
            if (_timer >= _currentAnimation.FrameDuration)
            {
                _timer -= _currentAnimation.FrameDuration;
                _currentFrameIndex++;

                // 循环处理
                if (_currentFrameIndex >= _currentAnimation.Frames.Count)
                {
                    _currentFrameIndex = _currentAnimation.IsLooping ? 0 : _currentAnimation.Frames.Count - 1;
                }
            }
        }

        // 拿到当前应该画的那张图
        public CroppedBitmap GetCurrentFrame()
        {
            if (_currentAnimation == null) return null;
            return _currentAnimation.Frames[_currentFrameIndex];
        }
    }
}
