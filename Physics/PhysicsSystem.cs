using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swinburne_OOP_HD
{
    public class PhysicsSystem
    {
        private readonly GravitySystem _gravitySystem;
        private readonly MovementSystem _movementSystem;

        public PhysicsSystem()
        {
            var collisionDetector = new CollisionDetector();
            _gravitySystem = new GravitySystem();
            _movementSystem = new MovementSystem(collisionDetector);
        }

        public void UpdateCharacter(Character character, Level level)
        {
            // Handle input-based movement
            HandleMovementInput(character);

            // Handle jumping
            bool jumpPressed = character.IsJumping();
            _movementSystem.HandleJump(character, jumpPressed);

            // Reset grounded status
            character.IsGrounded = false;

            // Apply gravity
            _gravitySystem.ApplyGravity(character);

            // Move with collision checking (for tiles collision checking)
            _movementSystem.MoveWithCollisionCheck(character, level, character.Velocity.X, 0);
            _movementSystem.MoveWithCollisionCheck(character, level, 0, character.Velocity.Y);
        }

        private void HandleMovementInput(Character character)
        {
            Vector2D currentVelocity = character.Velocity;

            if (character.IsMovingLeft())
            {
                character.Velocity = SplashKit.VectorTo(-GameConstants.MOVE_SPEED, currentVelocity.Y);
            }
            else if (character.IsMovingRight())
            {
                character.Velocity = SplashKit.VectorTo(GameConstants.MOVE_SPEED, currentVelocity.Y);
            }
            else
            {
                // Apply friction
                double newVelocityX = currentVelocity.X * GameConstants.FRICTION;
                if (Math.Abs(newVelocityX) < 0.1f)
                {
                    newVelocityX = 0;
                }
                character.Velocity = SplashKit.VectorTo(newVelocityX, currentVelocity.Y);
            }
        }
    }
}
