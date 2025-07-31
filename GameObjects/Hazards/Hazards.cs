using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;
using DotTiled;

namespace Swinburne_OOP_HD
{
    public abstract class Hazards : InteractableObject
    {
        private ActionResource _hazard;

        public Hazards(string name, string type, string spriteFilePath, Point2D position)
        {
            Name = name;
            InitializeHazardResources(spriteFilePath);
            Type = type;
            Position = new Point2D()
            {
                X = position.X,
                Y = position.Y - _hazard.Sprite.Height
            };
        }

        private void InitializeHazardResources(string spriteFilePath)
        {
            _hazard = new ActionResource();
            _hazard.Name = Name;
            _hazard.Bitmap = new Bitmap(Name, spriteFilePath);
            _hazard.Bitmap.SetCellDetails(22, 13, 15, 1, 15);
            _hazard.Script = SplashKit.LoadAnimationScript($"{Name}Anim", "liquid_animation.txt");
            _hazard.Animation = SplashKit.CreateAnimation(_hazard.Script, $"start");
            _hazard.Options = SplashKit.OptionWithAnimation(_hazard.Animation);
            _hazard.Sprite = SplashKit.CreateSprite($"{Name}Sprite", _hazard.Bitmap, _hazard.Script);
            _hazard.Sprite.StartAnimation(0);
        }

        public override void Interact(Character character)
        {
            if (IsCharacterInRange(character))
            {
                if (!CanInteract(character))
                {
                    character.IsDead = true;
                }
            }
        }

        public override bool CanInteract(Character character)
        {
            return character.Type == Type;
        }

        public override bool IsCharacterInRange(Character character)
        {
            // Get character's collision box
            Rectangle characterAABB = character.GetAABB();

            // Get hazard's collision box
            Rectangle hazardAABB = GetAABB();
            
            // Check if the rectangles intersect
            return SplashKit.RectanglesIntersect(characterAABB, hazardAABB);
        }

        public override Rectangle GetAABB()
        {
            return SplashKit.RectangleFrom(
                Position.X,
                Position.Y,
                _hazard.Sprite.Width,
                _hazard.Sprite.Height
            );
        }

        public override void Draw()
        {
            _hazard.Sprite.Draw(Position.X, Position.Y);
            _hazard.Sprite.UpdateAnimation();
        }
    }
} 