using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public abstract class InteractableObject : GameObject
    {
        private bool _isActivated;
        private Point2D _position;
        
        public bool IsActivated 
        { 
            get { return _isActivated; } 
            set { _isActivated = value; } 
        }

        public Point2D Position
        {
            get { return _position; } 
            set { _position = value; }
        }
        
        // Handles interaction logic when a character enters the interaction zone
        public abstract void Interact(Character character);
        
        // Checks if a character can interact with this object
        public abstract bool CanInteract(Character character);
        
        // Checks if a character is within the interaction range
        public abstract bool IsCharacterInRange(Character character);
        
        // Updates the object state and handles range checking
        public override void Update()
        {
            // Override in derived classes for specific update logic
        }
    }
} 