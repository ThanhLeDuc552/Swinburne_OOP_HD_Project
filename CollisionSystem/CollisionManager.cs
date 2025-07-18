using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class CollisionManager
    {
        public CollisionManager() { }

        public void CheckHazardCollisions(Character character, List<Hazards> hazards, ref bool gameOver) // currently violates OOP principles
        {
            foreach (Hazards hazard in hazards)
            {
                if (hazard.IsCharacterInRange(character))
                {
                    if (!hazard.CanInteract(character)) // Character is NOT immune
                    {
                        gameOver = true;
                        break; // Character died
                    }
                }
            }
        }

        public void CheckExitDoorInteractions(Character fireboy, Character watergirl, List<ExitDoor> exitDoors, ref bool fireExitReached, ref bool waterExitReached) // currently violates OOP principles
        {
            foreach (ExitDoor door in exitDoors)
            {
                // Check FireBoy interactions
                if (door.Type == "fire" && door.IsCharacterInRange(fireboy))
                {
                    door.Interact(fireboy);
                    if (door.IsActivated)
                    {
                        fireExitReached = true;
                    }
                }
                else if (door.Type == "fire" && !door.IsCharacterInRange(fireboy) && door.IsActivated)
                {
                    door.Interact(fireboy); // This will deactivate the door
                    fireExitReached = false;
                }

                // Check WaterGirl interactions
                if (door.Type == "water" && door.IsCharacterInRange(watergirl))
                {
                    door.Interact(watergirl);
                    if (door.IsActivated)
                    {
                        waterExitReached = true;
                    }
                }
                else if (door.Type == "water" && !door.IsCharacterInRange(watergirl) && door.IsActivated)
                {
                    door.Interact(watergirl); // This will deactivate the door
                    waterExitReached = false;
                }
            }
        }

        public List<Diamond> CheckDiamondInteraction(Character character, List<Diamond> diamonds) // currently violates OOP principles
        {
            List<Diamond> diamondsToRemove = new List<Diamond>();
            
            foreach (Diamond diamond in diamonds) 
            {
                if (diamond.IsCharacterInRange(character)) 
                {
                    if (diamond.CanInteract(character)) 
                    {
                        diamondsToRemove.Add(diamond); // Mark the actual diamond object for removal
                    }
                }
            }

            return diamondsToRemove;
        }

        public void CheckPlatformInteractions(Character character, List<Platform> platforms, PhysicsSystem physicsSystem) 
        {
            foreach (Platform platform in platforms)
            {
                Physics.FixStandingOnObject(character, platform);
                Physics.HandleObjectCollision(character, platform);
            }
        }

        public void CheckLeverInteractions(Character character, List<Lever> levers, List<Platform> platforms) 
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