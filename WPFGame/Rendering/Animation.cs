using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace WPFGame.Rendering
{
    public class Animation
    {
        public List<CroppedBitmap> Frames { get; private set; }
        public double FrameDuration { get; private set; } // 每帧停留多少秒 (例如 0.1s)
        public bool IsLooping { get; private set; }

        public Animation(List<CroppedBitmap> frames, double frameDuration, bool isLooping = true)
        {
            Frames = frames;
            FrameDuration = frameDuration;
            IsLooping = isLooping;
        }
    }
}
