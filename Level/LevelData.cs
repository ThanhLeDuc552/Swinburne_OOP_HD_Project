using DotTiled;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class LevelData
    {
        public uint TileWidth { get; set; }
        public uint TileHeight { get; set; }
        public uint MapWidth { get; set; }
        public uint MapHeight { get; set; }
        public ObjectLayer? ObjectLayer { get; set; }
        public TileLayer? TileLayer { get; set; }
        public string? BackgroundImagePath { get; set; }
        public uint[]? MapData { get; set; }
        public FlippingFlags[]? FlippingFlags { get; set; }
        public Dictionary<uint, Bitmap> Tiles { get; set; } = new Dictionary<uint, Bitmap>();
    }
}