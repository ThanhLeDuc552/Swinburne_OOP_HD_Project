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
        private readonly JumpSystem _jumpSystem;
        private readonly MovementSystem _movementSystem;
        private readonly CollisionResolver _collisionResolver;

        public PhysicsSystem()
        {
            var collisionDetector = new CollisionDetector();
            _gravitySystem = new GravitySystem();
            _jumpSystem = new JumpSystem();
            _movementSystem = new MovementSystem(collisionDetector);
            _collisionResolver = new CollisionResolver(collisionDetector);
        }

        public void UpdateCharacter(Character character, Level level)
        {
            // Handle input-based movement
            HandleMovementInput(character);

            // Handle jumping
            bool jumpPressed = character.IsJumping();
            _jumpSystem.HandleJump(character, jumpPressed);

            // Reset grounded status
            bool wasGrounded = character.IsGrounded;
            character.IsGrounded = false;

            // Apply gravity
            _gravitySystem.ApplyGravity(character);

            // Move with collision checking
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
