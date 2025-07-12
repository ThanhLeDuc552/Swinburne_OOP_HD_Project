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
        
        /// <summary>
        /// Handles interaction logic when a character enters the interaction zone
        /// </summary>
        /// <param name="character">The character attempting to interact</param>
        public abstract void Interact(Character character);
        
        /// <summary>
        /// Checks if a character can interact with this object
        /// </summary>
        /// <param name="character">The character to check</param>
        /// <returns>True if interaction is possible</returns>
        public abstract bool CanInteract(Character character);
        
        /// <summary>
        /// Checks if a character is within the interaction range
        /// </summary>
        /// <param name="character">The character to check</param>
        /// <returns>True if character is in range</returns>
        public abstract bool IsCharacterInRange(Character character);
        
        /// <summary>
        /// Updates the object state and handles range checking
        /// </summary>
        public override void Update()
        {
            // Override in derived classes for specific update logic
        }
    }
} 