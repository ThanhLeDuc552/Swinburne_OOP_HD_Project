using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;
using DotTiled;

namespace Swinburne_OOP_HD
{
    public abstract class ExitDoor : InteractableObject
    {
        protected ActionResource _doorInteracted;
        protected ActionResource _doorNotInteracted;
        protected bool _interacted;
        double _doorWidth;
        double _doorHeight;

        public ExitDoor(DotTiled.Object obj, string spriteFilePath)
        {
            InitializeDoorResources(obj, spriteFilePath);

            Type = (obj.Properties[0] as StringProperty).Value;
            Name = obj.Name;

            _doorWidth = _doorInteracted.Sprite.Width * 0.8f;
            _doorHeight = _doorInteracted.Sprite.Height * 0.8f;
            _interacted = false;

            Position = new Point2D()
            {
                X = obj.X,
                Y = obj.Y - _doorHeight
            };

        }

        private void InitializeDoorResources(DotTiled.Object obj, string spriteFilePath)
        {
            // Initialize interacted door resource
            _doorInteracted = new ActionResource();
            _doorInteracted.Name = obj.Name;
            _doorInteracted.Bitmap = new Bitmap($"{obj.Name}", spriteFilePath);
            _doorInteracted.Bitmap.SetCellDetails(59, 62, 22, 1, 22);
            _doorInteracted.Script = SplashKit.LoadAnimationScript("DoorAnim", "door_animation_interacted.txt");
            _doorInteracted.Animation = SplashKit.CreateAnimation(_doorInteracted.Script, $"Interacted");
            _doorInteracted.Options = SplashKit.OptionWithAnimation(_doorInteracted.Animation);
            _doorInteracted.Sprite = SplashKit.CreateSprite($"{obj.Name}Sprite", _doorInteracted.Bitmap, _doorInteracted.Script);
            _doorInteracted.Sprite.Scale = 0.8f; // Scale the sprite to fit the game world

            // Initialize not interacted door resource
            _doorNotInteracted = new ActionResource();
            _doorNotInteracted.Name = obj.Name;
            _doorNotInteracted.Bitmap = new Bitmap($"{obj.Name}", spriteFilePath);
            _doorNotInteracted.Bitmap.SetCellDetails(59, 62, 22, 1, 22);
            _doorNotInteracted.Script = SplashKit.LoadAnimationScript("DoorAnim", "door_animation_not_interacted.txt");
            _doorNotInteracted.Animation = SplashKit.CreateAnimation(_doorNotInteracted.Script, $"NotInteracted");
            _doorNotInteracted.Options = SplashKit.OptionWithAnimation(_doorNotInteracted.Animation);
            _doorNotInteracted.Sprite = SplashKit.CreateSprite($"{obj.Name}Sprite", _doorNotInteracted.Bitmap, _doorNotInteracted.Script);
            _doorNotInteracted.Sprite.Scale = 0.8f; // Scale the sprite to fit the game world
            _doorNotInteracted.Sprite.StartAnimation("NotInteracted");
        }

        public override void Draw()
        {
            if (_interacted)
            {
                _doorInteracted.Sprite.Draw(Position.X, Position.Y);
                _doorInteracted.Sprite.UpdateAnimation();
            }
            else
            {
                _doorNotInteracted.Sprite.Draw(Position.X, Position.Y);
                _doorNotInteracted.Sprite.UpdateAnimation();
            }
        }

        public override bool IsCharacterInRange(Character character)
        {
            // Get character's collision box
            Rectangle characterAABB = character.GetAABB();

            // Get door's collision box
            Rectangle doorAABB = GetAABB();
            
            // Check if the rectangles intersect
            return SplashKit.RectanglesIntersect(characterAABB, doorAABB);
        }

        public override bool CanInteract(Character character)
        {
            return character.Type == Type && IsCharacterInRange(character);
        }

        public override void Interact(Character character)
        {
            if (IsCharacterInRange(character))
            {
                if (character.Type == Type && !_interacted)
                {
                    _interacted = true;
                    IsActivated = true;
                    // Trigger the door opening animation
                    _doorInteracted.Sprite.StartAnimation("Interacted");
                }
            }
            else if (_interacted && !IsCharacterInRange(character))
            {
                if (character.Type == Type)
                {
                    _interacted = false;
                    IsActivated = false;
                    // Trigger the door closing animation
                    _doorNotInteracted.Sprite.StartAnimation(0);
                }
            }
        }

        public override Rectangle GetAABB()
        {
            return SplashKit.RectangleFrom(
                Position.X,
                Position.Y,
                _doorWidth,
                _doorHeight
            );
        }

        public override void Update()
        {
            // Override in derived classes if needed for specific door update logic
        }
    }
} 