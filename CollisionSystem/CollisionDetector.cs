using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class CollisionDetector : ICollisionDetector
    {
        // Object collision
        public bool HasCollision(SolidObject obj1, SolidObject obj2)
        {
            // Check if the bounding boxes of the two objects intersect
            return SplashKit.RectanglesIntersect(obj1.GetAABB(), obj2.GetAABB());
        }

        // Tile collision 
        public bool HasCollision(SolidObject movingObject, Level level)
        {
            Rectangle movingObjectAABB = movingObject.GetAABB();
            return CheckTileCollision(movingObjectAABB, level);
        }

        public CollisionInfo GetCollisionInfo(SolidObject obj1, SolidObject obj2)
        {
            if (!HasCollision(obj1, obj2))
            {
                return new CollisionInfo { HasCollision = false, Side = CollisionSide.None };
            }

            Rectangle rect1 = obj1.GetAABB();
            Rectangle rect2 = obj2.GetAABB();

            double overlapLeft = SplashKit.RectangleRight(rect1) - SplashKit.RectangleLeft(rect2);
            double overlapRight = SplashKit.RectangleRight(rect2) - SplashKit.RectangleLeft(rect1);
            double overlapTop = SplashKit.RectangleBottom(rect1) - SplashKit.RectangleTop(rect2);
            double overlapBottom = SplashKit.RectangleBottom(rect2) - SplashKit.RectangleTop(rect1);

            double minOverlap = Math.Min(Math.Min(overlapLeft, overlapRight), Math.Min(overlapTop, overlapBottom)); // Check the side with the min overlap (which actually is the max overlap)

            CollisionSide side = CollisionSide.None;
            Vector2D normal = SplashKit.VectorTo(0, 0);

            if (minOverlap == overlapTop)
            {
                side = CollisionSide.Top;
                normal = SplashKit.VectorTo(0, -1); // normalized vector for velocity
            }
            else if (minOverlap == overlapBottom)
            {
                side = CollisionSide.Bottom;
                normal = SplashKit.VectorTo(0, 1);
            }
            else if (minOverlap == overlapLeft)
            {
                side = CollisionSide.Left;
                normal = SplashKit.VectorTo(-1, 0);
            }
            else if (minOverlap == overlapRight)
            {
                side = CollisionSide.Right;
                normal = SplashKit.VectorTo(1, 0);
            }

            return new CollisionInfo
            {
                HasCollision = true,
                CollisionNormal = normal,
                PenetrationDepth = minOverlap,
                Side = side
            };
        }

        private bool CheckTileCollision(Rectangle movingObjectAABB, Level level)
        {
            int leftTile = (int)(SplashKit.RectangleLeft(movingObjectAABB) / GameConstants.TILE_SIZE);
            int rightTile = (int)(SplashKit.RectangleRight(movingObjectAABB) / GameConstants.TILE_SIZE);
            int topTile = (int)(SplashKit.RectangleTop(movingObjectAABB) / GameConstants.TILE_SIZE);
            int bottomTile = (int)(SplashKit.RectangleBottom(movingObjectAABB) / GameConstants.TILE_SIZE);

            for (int y = topTile; y <= bottomTile; y++)
            {
                for (int x = leftTile; x <= rightTile; x++)
                {
                    if (IsValidTilePosition(x, y, level) && IsSolidTile(x, y, level))
                    {
                        Rectangle tileBounds = GetTileBounds(x, y);

                        if (SplashKit.RectanglesIntersect(movingObjectAABB, tileBounds))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool IsValidTilePosition(int x, int y, Level level)
        {
            return x >= 0 && x < level.MapWidth && y >= 0 && y < level.MapHeight;
        }

        private bool IsSolidTile(int x, int y, Level level)
        {
            int tileIndex = y * (int)level.MapWidth + x;

            if (tileIndex >= 0 && tileIndex < level.MapData.Length)
            {
                uint tileGID = level.MapData[tileIndex];
                return tileGID >= 1 && tileGID <= 7; // 1-7 are solid tiles
            }

            return false;
        }

        private Rectangle GetTileBounds(int x, int y)
        {
            return SplashKit.RectangleFrom(
                x * GameConstants.TILE_SIZE,
                y * GameConstants.TILE_SIZE,
                GameConstants.TILE_SIZE,
                GameConstants.TILE_SIZE
            );
        }
    }
}
