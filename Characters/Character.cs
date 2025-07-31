using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    abstract public class Character : SolidObject // for future object extension
    {
        // Action resources for different character actions
        private ActionResource _moveRight;
        private ActionResource _moveLeft;
        private ActionResource _jump;
        private ActionResource _fall;
        private ActionResource _idle;
        private ActionResource _currAction;

        // Vectors - physics
        private bool _canJump;

        // AABB properties for collision detection
        private double _hairHeight;
        private double _collisionWidth;
        private double _collisionHeight;
        private double _halfWidth;
        private double _halfHeight;

        // In game properties
        private bool _isDead;

        public Character(string bundleFile, string type, string name)
        {
            base.Position = new Vector2D(); // The original position (Position is the center of the character, which used for AABB)
            Type = type;
            Name = name;
            Velocity = new Vector2D() { X = 0, Y = 0 };
            IsGrounded = false;
            _canJump = true;
            _isDead = false;

            SplashKit.LoadResourceBundle(name, bundleFile);
            InitActions(name);
            _currAction = _idle;
            SetScale();

            _halfWidth = (_idle.Sprite.Width * GameConstants.SPRITE_SCALE) / 2.0;
            _halfHeight = (_idle.Sprite.Height * GameConstants.SPRITE_SCALE) / 2.0;
            _hairHeight = (_idle.Sprite.Height - GameConstants.HEAD_TO_LEG_PIXELS) * GameConstants.SPRITE_SCALE;
            _collisionWidth = _idle.Sprite.Width * GameConstants.SPRITE_SCALE;
            _collisionHeight = _idle.Sprite.Height * GameConstants.SPRITE_SCALE - _hairHeight;
        }

        // Draw the character
        public override void Draw()
        {
            (double offsetX, double offsetY) = CalculateSpriteOffset(_currAction);

            // Draw with calculated offsets (feet aligned for sprites)
            _currAction.Sprite.Draw(offsetX + base.Position.X, offsetY + base.Position.Y);
            _currAction.Sprite.UpdateAnimation();
        }

        // Get character's AABB bounding box for collision detection
        public override Rectangle GetAABB()
        {
            return SplashKit.RectangleFrom(
                Position.X - _halfWidth,                 // left
                Position.Y - _halfHeight + _hairHeight,   // top - hair height
                _collisionWidth,
                _collisionHeight
            );
        }

        public void SetCurrentAction(ActionResource action)
        {
            if (_currAction.Name != action.Name)
            {
                _currAction = action;
            }
        }

        private void SetScale()
        {
            _idle.Sprite.Scale = GameConstants.SPRITE_SCALE;
            _fall.Sprite.Scale = GameConstants.SPRITE_SCALE;
            _jump.Sprite.Scale = GameConstants.SPRITE_SCALE;
            _moveRight.Sprite.Scale = GameConstants.SPRITE_SCALE;
            _moveLeft.Sprite.Scale = GameConstants.SPRITE_SCALE;
        }

        // Update animations based on current physics state and input
        public override void Update()
        {
            // Determine animation based on physics state and input
            if (!IsGrounded && Velocity.Y > 0)
            {
                // Falling - use fall animation or movement animation based on horizontal input
                if (IsMovingLeft())
                    SetCurrentAction(_moveLeft);
                else if (IsMovingRight())
                    SetCurrentAction(_moveRight);
                else
                    SetCurrentAction(_fall);
            }
            else if (!IsGrounded && Velocity.Y < 0)
            {
                // Jumping up - use jump animation or movement animation based on horizontal input
                if (IsMovingLeft())
                    SetCurrentAction(_moveLeft);
                else if (IsMovingRight())
                    SetCurrentAction(_moveRight);
                else
                    SetCurrentAction(_jump);
            }
            else if (IsGrounded)
            {
                // On ground - use movement or idle animation
                if (IsMovingLeft())
                    SetCurrentAction(_moveLeft);
                else if (IsMovingRight())
                    SetCurrentAction(_moveRight);
                else
                    SetCurrentAction(_idle);
            }
        }

        public override Vector2D Position // new to override the base Position property
        {
            get 
            {
                Vector2D currPos = new Vector2D();
                currPos.X = base.Position.X + _idle.Sprite.SpriteCenterPoint.X;
                currPos.Y = base.Position.Y + _idle.Sprite.SpriteCenterPoint.Y;
                return currPos;
            }
            set
            {
                base.Position = SplashKit.VectorTo
                (
                    value.X - _idle.Sprite.SpriteCenterPoint.X,
                    value.Y - _idle.Sprite.SpriteCenterPoint.Y
                );
            }
        }

        public bool CanJump 
        {
            get { return _canJump; }
            set { _canJump = value; }
        }

        public double HalfWidth 
        {
            get { return _halfWidth; }
        }

        public double HalfHeight 
        {
            get { return _halfHeight; }
        }

        public double HairHeight 
        {
            get { return _hairHeight; }
        }

        public bool IsDead
        {
            get { return _isDead; }
            set { _isDead = value; }
        }

        // Physics properties for external access
        public abstract bool IsMovingLeft();
        public abstract bool IsMovingRight();
        public abstract bool IsJumping();
        public abstract bool IsFalling();
        public abstract bool IsIdle();

        public ActionResource Idle // for main menu
        {
            get { return _idle; }
        }

        private ActionResource CreateActionResource(string actionName, string charName)
        {
            ActionResource action = new ActionResource();
            action.Name = actionName;
            action.Bitmap = SplashKit.BitmapNamed(charName + actionName);
            action.Script = SplashKit.AnimationScriptNamed(charName + actionName);
            action.Animation = SplashKit.CreateAnimation(action.Script, actionName);
            action.Options = SplashKit.OptionWithAnimation(action.Animation);
            action.Sprite = SplashKit.CreateSprite(charName + actionName, action.Bitmap, action.Script);

            return action;
        }

        // Initialize actions for the character
        private void InitActions(string name)
        {
            // MoveRight initialization
            _moveRight = CreateActionResource("MoveRight", name);
            _moveRight.Sprite.StartAnimation(0);

            // MoveLeft initialization
            _moveLeft = CreateActionResource("MoveLeft", name);
            _moveLeft.Sprite.StartAnimation(0);

            // Jump initialization
            _jump = CreateActionResource("Jump", name);
            _jump.Sprite.StartAnimation(0);

            // Fall initialization
            _fall = CreateActionResource("Fall", name);
            _fall.Sprite.StartAnimation(0);

            // Idle initialization
            _idle = CreateActionResource("Idle", name);
            _idle.Sprite.StartAnimation(0);
        }

        // Method to calculate sprite offsets based on action
        private (double offsetX, double offsetY) CalculateSpriteOffset(ActionResource action)
        {
            // Extract center points
            double idleCenterX = _idle.Sprite.SpriteCenterPoint.X;
            double idleCenterY = _idle.Sprite.SpriteCenterPoint.Y;
            double currCenterX = action.Sprite.SpriteCenterPoint.X;
            double currCenterY = action.Sprite.SpriteCenterPoint.Y;

            // Calculate height adjustments
            double idleHeightAdj = _idle.Sprite.Height * GameConstants.SPRITE_SCALE;
            double currHeightAdj = action.Sprite.Height * GameConstants.SPRITE_SCALE;

            // Calculate width adjustments
            double idleWidthAdj = _idle.Sprite.Width * GameConstants.SPRITE_SCALE;
            double currWidthAdj = action.Sprite.Width * GameConstants.SPRITE_SCALE;

            // Calculate base offsets
            double offsetX = idleCenterX - currCenterX;
            double offsetY = idleCenterY - currCenterY + (idleHeightAdj - currHeightAdj) / 2.0;

            // Apply action-specific X adjustments
            if (action.Name == _moveLeft.Name)
            {
                offsetX += (currWidthAdj - idleWidthAdj) / 2.0;
            }
            else if (action.Name == _moveRight.Name)
            {
                offsetX += (currWidthAdj - idleWidthAdj) / 2.0 - (currWidthAdj / 2.0 - GameConstants.FOOT_TO_FACE_SCALED_DISTANCE);
            }

            return (offsetX, offsetY);
        }
    }
}

