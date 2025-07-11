using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public abstract class SolidObject : GameObject
    {
        private Vector2D _position;
        private bool _isGrounded;
        private Vector2D _velocity;

        public bool IsGrounded
        {
            get { return _isGrounded; }
            set { _isGrounded = value; }
        }

        public Vector2D Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public virtual Vector2D Position
        {
            get { return _position; }
            set { _position = value; }
        }
    }
}