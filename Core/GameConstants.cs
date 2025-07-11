namespace Swinburne_OOP_HD 
{
    public struct GameConstants
    {
        // Sprite scaling factor and calculation
        public const float SPRITE_SCALE = 0.1f;
        public const double HEAD_TO_LEG_ORIGINAL_DISTANCE = 50.0; // Original distance between head and leg in moveRight mode
        public const double FOOT_TO_FACE_SCALED_DISTANCE = HEAD_TO_LEG_ORIGINAL_DISTANCE * SPRITE_SCALE; // Scaled distance (= 5.0)
        public const int HEAD_TO_LEG_PIXELS = 280; // Character's sprite original height excluding hair (scaling and AABB)

        // Physics constants - these control how the character moves and feels
        public const double TILE_SIZE = 16.0;
        public const float GRAVITY_STRENGTH = 0.001f;    // How fast objects accelerate downward (pixels/frame²) - much slower gravity
        public const float JUMP_FORCE = -0.31f;          // Initial upward velocity when jumping (negative = up) - increased from -0.31f for higher jumps
        public const float MOVE_SPEED = 0.2f;            // Horizontal movement speed (pixels/frame) - very slow and precise
        public const float FRICTION = 0.85f;             // How quickly horizontal movement slows down (0-1, closer to 1 = less friction)
        public const float TERMINAL_VELOCITY = 15.0f;    // Maximum falling speed to prevent infinite acceleration
    }
}