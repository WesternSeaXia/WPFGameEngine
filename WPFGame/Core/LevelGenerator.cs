using System;
using System.Collections.Generic;
using System.Text;
using WPFGame.Entities.Characters;
using WPFGame.Entities.Environment;

namespace WPFGame.Core
{
    public static class LevelGenerator
    {
        public static void LoadMap(GameEngine engine, string[] mapData)
        {
            int totalRows = mapData.Length;

            // 遍历每一行文本 (Y 轴方向)
            for (int row = 0; row < totalRows; row++)
            {
                string line = mapData[row];

                // 遍历每一个字符 (X 轴方向)
                for (int col = 0; col < line.Length; col++)
                {
                    char tile = line[col];

                    // 【核心推导】：
                    // X 坐标：直接按列号乘以格子大小
                    double xPos = col * GlobalConfig.TileSize;

                    // Y 坐标：数组最后一行(totalRows - 1)对应真实世界的 Y=0
                    // 行号越小（在文本里越靠上），Y坐标应该越大
                    double yPos = ((totalRows - 1) - row) * GlobalConfig.TileSize;

                    switch (tile)
                    {
                        case '#': // 实心地板 (50x50)
                            engine.AddObject(new Platform
                            {
                                X = xPos,
                                Y = yPos,
                                HitboxWidth = GlobalConfig.TileSize,
                                HitboxHeight = GlobalConfig.TileSize
                            });
                            break;

                        case '=': // 半高跳台 (悬浮的薄木板，高度20)
                            // 为了让跳台与方格顶部对齐，Y 轴需要加上 (50 - 20)
                            engine.AddObject(new Platform
                            {
                                X = xPos,
                                Y = yPos + (GlobalConfig.TileSize - 20),
                                HitboxWidth = GlobalConfig.TileSize,
                                HitboxHeight = 20
                            });
                            break;

                        case 'P': // 玩家出生点
                            var player = new Player
                            {
                                X = xPos,
                                Y = yPos,
                                HitboxWidth = 40,
                                HitboxHeight = 50
                            };
                            engine.SetPlayer(player);
                            break;

                        case 'C': // 金币 (20x20，居中放置)
                            double coinOffset = (GlobalConfig.TileSize - 20) / 2;
                            engine.AddObject(new Coin
                            {
                                X = xPos + coinOffset,
                                Y = yPos + coinOffset,
                                HitboxWidth = 20,
                                HitboxHeight = 20
                            });
                            break;

                        case 'S': // 尖刺 (放在当前格子地面的底部，半高)
                            engine.AddObject(new Spike
                            {
                                X = xPos,
                                Y = yPos,
                                HitboxWidth = GlobalConfig.TileSize,
                                HitboxHeight = GlobalConfig.TileSize / 2
                            });
                            break;

                        case ' ': // 空气，不处理
                        default:
                            break;
                    }
                }
            }
        }
    }
}
