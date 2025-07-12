using SplashKitSDK;
using DotTiled;

namespace Swinburne_OOP_HD
{
    public class Button : InteractableObject
    {
        private Bitmap _button;

        public Button(DotTiled.Object obj)
        {
            Name = obj.Name;
            Type = (obj.Properties[0] as StringProperty).Value;
            Position = new Point2D()
            {
                X = obj.X,
                Y = obj.Y - obj.Height
            };

            IsActivated = false;
            _button = SplashKit.LoadBitmap(Name, @"tiles\objects\other\button.png");
        }

        public override void Draw() 
        {
            if (!IsActivated)
            {
                _button.Draw(Position.X, Position.Y);
            }
        }
        
        public override bool IsCharacterInRange(Character character) 
        {
            Rectangle characterAABB = character.GetAABB();
            Rectangle buttonAABB = GetAABB();
            return SplashKit.RectanglesIntersect(characterAABB, buttonAABB);
        }

        public override bool CanInteract(Character character) 
        {
            return true;
        }

        public override void Interact(Character character) 
        {
            if (IsCharacterInRange(character))
            {
                IsActivated = true;
            }
            else 
            {
                IsActivated = false;
            }
        }

        public override Rectangle GetAABB()
        {
            return SplashKit.RectangleFrom(
                Position.X,
                Position.Y,
                _button.Width,
                _button.Height
            );
        }
    }
}