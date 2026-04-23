using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WPFGame.Rendering
{
    public static class SpriteLoader
    {
        public static BitmapImage LoadImage(string packUri)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(packUri, UriKind.Absolute);
            bitmap.EndInit();
            return bitmap;
        }

        // ==========================================
        // 核心切割方法：支持从左下角测量裁剪，支持指定起始列
        // ==========================================
        public static List<CroppedBitmap> Slice(
            BitmapSource source,
            int cellWidth, int cellHeight, // 原始网格大小 (例如 98x64)
            int row, int frameCount,       // 行号和总帧数
            int startCol = 0,              // 【新增】：从第几列开始切，默认 0
            int cropX = 0, int cropY = 0,  // 【修改】：相对网格【左下角】的偏移
            int cropWidth = 0, int cropHeight = 0)
        {
            var frames = new List<CroppedBitmap>();

            // 如果没传裁剪尺寸，默认使用完整网格尺寸
            int finalWidth = cropWidth > 0 ? cropWidth : cellWidth;
            int finalHeight = cropHeight > 0 ? cropHeight : cellHeight;

            for (int i = 0; i < frameCount; i++)
            {
                // 1. 定位到当前网格的列号
                int col = startCol + i;

                // 2. 先定位到这个网格在整张大图上的【左上角】绝对坐标 (WPF原生坐标系)
                int cellStartX = col * cellWidth;
                int cellStartY = row * cellHeight;

                // 3. 【核心数学转换】：将基于左下角的测距，翻转为基于左上角的坐标

                // X 轴同向：网格左上角 X + 左侧向右偏移
                int actualX = cellStartX + cropX;

                // Y 轴反向：WPF 的 Y 是向下递增的。
                // 算法：网格高度(64) - 底部留白(cropY) - 真实切割高度(finalHeight) = 距离顶部的 Y 坐标
                int actualY = cellStartY + (cellHeight - cropY - finalHeight);

                // 4. 执行 WPF 原生切割
                var rect = new Int32Rect(actualX, actualY, finalWidth, finalHeight);
                var cropped = new CroppedBitmap(source, rect);
                frames.Add(cropped);
            }
            return frames;
        }
    }
}