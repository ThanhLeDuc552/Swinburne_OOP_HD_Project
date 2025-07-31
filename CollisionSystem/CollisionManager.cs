using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class CollisionManager
    {
        private readonly CollisionResolver _collisionResolver;

        public CollisionManager() 
        {
            var collisionDetector = new CollisionDetector();
            _collisionResolver = new CollisionResolver(collisionDetector);
        }

        public void CheckHazardCollisions(List<Character> characters, List<Hazards> hazards)
        {
            foreach (Character character in characters)
            {
                foreach (Hazards hazard in hazards)
                {
                    hazard.Interact(character);
                }
            }
        }

        public void CheckExitDoorInteractions(Character fireboy, Character watergirl, List<ExitDoor> exitDoors, Level level)
        {
            foreach (ExitDoor door in exitDoors)
            {
                // Check FireBoy interactions
                if (door.Type == "fire" && door.IsCharacterInRange(fireboy))
                {
                    door.Interact(fireboy);
                    if (door.IsActivated)
                    {
                        level.FireExitReached = true;
                    }
                }
                else if (door.Type == "fire" && !door.IsCharacterInRange(fireboy) && door.IsActivated)
                {
                    door.Interact(fireboy); // This will deactivate the door
                    level.FireExitReached = false;
                }

                // Check WaterGirl interactions
                if (door.Type == "water" && door.IsCharacterInRange(watergirl))
                {
                    door.Interact(watergirl);
                    if (door.IsActivated)
                    {
                        level.WaterExitReached = true;
                    }
                }
                else if (door.Type == "water" && !door.IsCharacterInRange(watergirl) && door.IsActivated)
                {
                    door.Interact(watergirl); // This will deactivate the door
                    level.WaterExitReached = false;
                }
            }
        }

        public List<Diamond> CheckDiamondInteraction(List<Character> characters, List<Diamond> diamonds)
        {
            List<Diamond> collectedDiamonds = new List<Diamond>();
            foreach (Character character in characters)
            {
                foreach (Diamond diamond in diamonds)
                {
                    diamond.Interact(character);
                    if (diamond.IsCollected)
                    {
                        collectedDiamonds.Add(diamond);
                    }
                }
            }
            return collectedDiamonds;
        }

        public void CheckPlatformInteractions(List<Character> characters, List<Platform> platforms, PhysicsSystem physicsSystem) 
        {
            foreach (Character character in characters)
            {
                foreach (Platform platform in platforms)
                {
                    _collisionResolver.ResolveCollision(character, platform);
                }
            }
        }

        public void CheckLeverInteractions(List<Character> characters, List<Lever> levers, List<Platform> platforms) 
        {
            foreach (Character character in characters)
            {
                foreach (Lever lever in levers)
                {
                    if (lever.IsCharacterInRange(character))
                    {
                        lever.Interact(character);
                    }

                    else if (lever.Timer.IsStarted && lever.Timer.Ticks > 10000u)
                    {
                        lever.ResetTimer();
                    }

                    foreach (Platform platform in platforms)
                    {
                        if (platform.ActivatorClass == "Lever" && platform.Type == lever.Type)
                        {
                            if (lever.IsActivated)
                            {
                                platform.IsActivated = true;
                            }
                            else
                            {
                                platform.IsActivated = false;
                            }
                        }
                    }
                }
            }
        }

        public void CheckButtonInteractions(List<Character> characters, List<Button> buttons, List<Platform> platforms)
        {
            foreach (Button button in buttons)
            {
                bool anyCharacterInRange = false;

                foreach (Character character in characters)
                {
                    if (button.IsCharacterInRange(character))
                    {
                        anyCharacterInRange = true;
                        break;
                    }
                }

                // Update button state based on whether any character is in range
                if (anyCharacterInRange)
                {
                    button.IsActivated = true;
                }
                else
                {
                    button.IsActivated = false;
                }
            }

            // Update platform states based on button groups (only once)
            foreach (Platform platform in platforms)
            {
                if (platform.ActivatorClass == "Button")
                {
                    bool anyButtonActivated = false;

                    foreach (Button button in buttons)
                    {
                        if (platform.Type == button.Type)
                        {
                            if (button.IsActivated)
                            {
                                anyButtonActivated = true;
                                break;
                            }
                        }
                    }

                    platform.IsActivated = anyButtonActivated;
                }
            }
        }

        public void CheckBoxInteractions() { }
    }
} 