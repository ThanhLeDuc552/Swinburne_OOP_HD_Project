using SplashKitSDK;

namespace Swinburne_OOP_HD 
{
    public abstract class Diamond : InteractableObject
    {
        private ActionResource _diamond;
        private bool _isSpecial;
        private bool _isCollected;

        public Diamond(string name, string type, string spriteFilePath, Point2D position, bool isSpecial = false)
        {
            Type = type;
            Name = name;
            _isSpecial = isSpecial;
            _isCollected = false; // Initialize as not collected

            InitializeDiamondResources(spriteFilePath);
            Position = new Point2D()
            {
                X = position.X,
                Y = position.Y - _diamond.Sprite.Height
            };
        }

        private void InitializeDiamondResources(string spriteFilePath)
        {
            _diamond = new ActionResource();
            _diamond.Name = Name;
            _diamond.Bitmap = new Bitmap(Name, spriteFilePath);
            if (_isSpecial)
            {
                _diamond.Bitmap.SetCellDetails(34, 29, 40, 1, 40);
            }
            else
            {
                _diamond.Bitmap.SetCellDetails(27, 23, 40, 1, 40);
            }
            _diamond.Script = SplashKit.LoadAnimationScript($"{Name}Anim", "diamond_animation.txt");
            _diamond.Animation = SplashKit.CreateAnimation(_diamond.Script, $"start");
            _diamond.Options = SplashKit.OptionWithAnimation(_diamond.Animation);
            _diamond.Sprite = SplashKit.CreateSprite($"{Name}Sprite", _diamond.Bitmap, _diamond.Script);
            _diamond.Sprite.StartAnimation(0);
        }

        public override void Interact(Character character)
        {
            if (IsCharacterInRange(character) && CanInteract(character))
            {
                SoundCollections.Instance.PlaySound("DiamondCollect");
                _isCollected = true;
            }
        }

        public override bool CanInteract(Character character)
        {
            if (_isSpecial) return true;
            return character.Type == Type;
        }

        public override bool IsCharacterInRange(Character character)
        {
            Rectangle characterAABB = character.GetAABB();
            Rectangle diamondAABB = GetAABB();
            return SplashKit.RectanglesIntersect(characterAABB, diamondAABB);
        }

        public override void Draw()
        {
            _diamond.Sprite.Draw(Position.X, Position.Y);
            _diamond.Sprite.UpdateAnimation();
        }

        public override Rectangle GetAABB()
        {
            return SplashKit.RectangleFrom(
                Position.X, 
                Position.Y, 
                _diamond.Sprite.Width, 
                _diamond.Sprite.Height
            );
        }

        public bool IsCollected
        {
            get { return _isCollected; }
        }
    }
}