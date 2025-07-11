using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;
using DotTiled;

namespace Swinburne_OOP_HD
{
    public class Platform : SolidObject
    {
        private Bitmap _platform;
        private Direction _direction; // direction of movement
        private float _movedDistance; // track moved distance for smooth transition and appropriate logic
        private Dictionary<Direction, Vector2D> _directionVectors; // store direction vectors for each direction
        private string _activatorClass; // store the class of the activator
        private int _tileToMove; // number of tiles to move
        private bool _isActivated; // whether the platform is activated or not

        public Platform(DotTiled.Object obj, string platformSpritePath)
        {
            Type = (obj.Properties[3] as StringProperty).Value;
            Name = obj.Name;
            Position = SplashKit.VectorTo(obj.X, obj.Y - obj.Height);

            _activatorClass = (obj.Properties[0] as StringProperty).Value;
            _direction = (Direction)(obj.Properties[1] as IntProperty).Value;
            _tileToMove = (obj.Properties[2] as IntProperty).Value;
            _isActivated = false;
            _movedDistance = 0;
            _directionVectors = new Dictionary<Direction, Vector2D>
            {
                {Direction.Left, SplashKit.VectorTo(-GameConstants.MOVE_SPEED, 0)},
                {Direction.Right, SplashKit.VectorTo(GameConstants.MOVE_SPEED, 0)},
                {Direction.Up, SplashKit.VectorTo(0, -GameConstants.MOVE_SPEED)},
                {Direction.Down, SplashKit.VectorTo(0, GameConstants.MOVE_SPEED)}
            };

            InitializePlatform(platformSpritePath);
        }

        private void InitializePlatform(string platformSpritePath)
        {
            _platform = SplashKit.LoadBitmap(Name, platformSpritePath);
        }

        public override void Update()
        {
            if (_isActivated)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        public override void Draw()
        {
            _platform.Draw(Position.X, Position.Y);
        }

        public override Rectangle GetAABB()
        {
            return SplashKit.RectangleFrom(Position.X, Position.Y, _platform.Width, _platform.Height);
        }

        public override void ClearResource()
        {
            // Implement later
        }

        public void Activate()
        {
            if (_movedDistance < _tileToMove * GameConstants.TILE_SIZE)
            {
                _movedDistance += GameConstants.MOVE_SPEED;
                Position = SplashKit.VectorAdd(Position, _directionVectors[_direction]);
            }
        }

        public void Deactivate()
        {
            if (_movedDistance > 0)
            {
                _movedDistance -= GameConstants.MOVE_SPEED;
                Position = SplashKit.VectorSubtract(Position, _directionVectors[_direction]);
            }
        }

        public string ActivatorClass
        {
            get { return _activatorClass; }
        }

        public bool IsActivated
        {
            get { return _isActivated; }
            set { _isActivated = value; }
        }
    }
}