using SplashKitSDK;
using DotTiled;

namespace Swinburne_OOP_HD
{
    public class Lever : InteractableObject
    {
        private ActionResource _lever;
        private SplashKitSDK.Timer _timer;

        public Lever(DotTiled.Object obj, string leverSpritePath)
        {
            Type = (obj.Properties[0] as StringProperty).Value;
            Name = obj.Name;

            Position = new Point2D() 
            {
                X = obj.X,
                Y = obj.Y - obj.Height
            };

            IsActivated = false;
            _timer = new SplashKitSDK.Timer($"{Name}Timer");

            InitializeLever(leverSpritePath);
        }

        private void InitializeLever(string leverSpritePath) 
        {
            _lever.Name =  Name;
            _lever.Bitmap = new Bitmap(Name, leverSpritePath);
            _lever.Bitmap.SetCellDetails(18, 18, 2, 1, 2);
            _lever.Script = SplashKit.LoadAnimationScript("LeverAnimation", "lever_animation.txt");
            _lever.Sprite = SplashKit.CreateSprite($"{Name}Sprite", _lever.Bitmap, _lever.Script);
            _lever.Sprite.StartAnimation("Deactivate");
        }

        public override void Draw() 
        {
            _lever.Sprite.Draw(Position.X, Position.Y);
            _lever.Sprite.UpdateAnimation();
        }

        public override bool IsCharacterInRange(Character character) 
        {
            Rectangle characterAABB = character.GetAABB();
            Rectangle leverAABB = GetAABB();
            return SplashKit.RectanglesIntersect(characterAABB, leverAABB);
        }

        public override bool CanInteract(Character character) 
        {
            return true;
        }

        public override void Interact(Character character) 
        {
            // If timer hasn't started, start it and toggle lever
            if (!_timer.IsStarted) 
            {
                _timer.Start();
                ToggleLever();
            }
            // If timer has exceeded threshold, reset and toggle lever
            else if (_timer.Ticks > 10000u) 
            {
                _timer.Reset();
                ToggleLever();
            }
        }

        public override Rectangle GetAABB()
        {
            return SplashKit.RectangleFrom(
                Position.X,
                Position.Y,
                _lever.Sprite.Width,
                _lever.Sprite.Height
            );
        }

        public override void ClearResource()
        {
            // Implement later
        }

        public void ResetTimer() 
        {
            _timer.Stop();
            _timer.Reset();
        }

        public SplashKitSDK.Timer Timer
        {
            get { return _timer; }
        }

        private void ToggleLever()
        {
            if (!IsActivated)
            {
                IsActivated = true;
                _lever.Sprite.StartAnimation(1);
            }
            else 
            {
                IsActivated = false;
                _lever.Sprite.StartAnimation(0);
            }
        }
    }
}