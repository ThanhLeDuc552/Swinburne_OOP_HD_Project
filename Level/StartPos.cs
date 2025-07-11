using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class StartPos
    {
        private string _type;
        private Point2D _pos;

        public StartPos(string type, Point2D pos) 
        {
            _type = type;
            _pos = pos;
        }

        public void SetStartingPoint(Character character) 
        {
            if (character.Type == _type) 
            {
                character.Position = SplashKit.VectorTo(_pos.X, _pos.Y);
            }
        }

        public string Type 
        {
            get { return _type; }
        }

        public Point2D Position
        {
            get { return _pos; }
        }
    }
}
